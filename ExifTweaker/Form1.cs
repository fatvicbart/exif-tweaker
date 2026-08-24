using System.Collections.Concurrent;
using System.ComponentModel;
using ExifTweaker.Infrastructure;
using ExifTweaker.Controls;
using ExifTweaker.Forms;
using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker;

public partial class Form1 : Form
{
    private readonly ImportSession _session = new();
    private readonly EditHistory _history = new();
    private readonly SessionController _sessionController;
    private readonly ExifToolService _exifTool;
    private readonly ThumbnailService _thumbnails;
    private readonly LocationEditorService _locations = new();
    private MapControl _map => mapControl;
    private readonly BindingSource _bindingSource = new();
    private readonly BindingList<PhotoItem> _view = new();
    private readonly ConcurrentDictionary<string, Image> _gridThumbnails = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, byte> _thumbnailLoads = new(StringComparer.OrdinalIgnoreCase);
    private Func<PhotoItem, bool> _activeFilter = _ => true;
    private BindingList<PhotoItem> _files => _session.Media;
    private readonly AppSettings _settings = AppSettings.Load();
    private readonly FileDiscoveryService _discovery = new();
    private readonly MetadataService _metadata;
    private readonly IGeocodingService _geocoding;
    private readonly UpdateService _updates;
    private bool _isBusy;
    private string _activeFilterName = "Tous";
    private GpsCoordinate? _gpsClipboard;
    private readonly System.Windows.Forms.Timer _gpsSearchTimer = new() { Interval = 450 };
    private CancellationTokenSource? _gpsSearchCts;
    private bool _gpsSearchInProgress;
    private bool _updatingGpsSuggestions;
    private readonly ListBox _gpsSuggestions = new() { DisplayMember = nameof(Coordinates.Name), IntegralHeight = false };
    private readonly ToolStripDropDown _gpsSuggestionsPopup = new() { AutoClose = false, Padding = System.Windows.Forms.Padding.Empty };
    private GpsCoordinate? _currentLocation;
    private string _currentLocationName = string.Empty;
    private CancellationTokenSource? _currentLocationLookupCts;
    private readonly CancellationTokenSource _locationResolutionCts = new();
    private readonly SemaphoreSlim _locationResolutionLock = new(1, 1);
    private readonly ConcurrentDictionary<string, string> _locationAddressCache = new();
    private CancellationTokenSource? _operationCts;
    private readonly InformationControl _informationView = new();
    private readonly ConcurrentDictionary<string, (DateTime LastWriteUtc, IReadOnlyList<ExifTagInfo> Tags)> _informationCache = new(StringComparer.OrdinalIgnoreCase);
    private CancellationTokenSource? _informationCts;
    private PhotoItem? _activeItem;

    private List<PhotoItem> SelectedItems => dgv.SelectedRows.Cast<DataGridViewRow>()
        .Select(row => row.DataBoundItem as PhotoItem)
        .Concat(_session.Media.Where(item => item.IsSelected))
        .Where(item => item is not null)
        .Cast<PhotoItem>()
        .Distinct()
        .ToList();

    public Form1()
    {
        _exifTool = new ExifToolService(_settings.ExifToolPath);
        _metadata = new MetadataService(_exifTool, _settings);
        _thumbnails = new ThumbnailService(_exifTool, _settings);
        _geocoding = new GeocodingService(_settings);
        _updates = new UpdateService(_settings);
        _sessionController = new SessionController(_session, _history);
        InitializeComponent();
        splitContainer1.Panel2.Controls.Add(_informationView);
        _informationView.Visible = false;
        InitializeGpsSuggestionsPopup();
        InitializeNavigation();
        _bindingSource.DataSource = _view;
        dgv.DataSource = _bindingSource;
        bChange.Text = "PRÉPARER";
        bChange.AccessibleDescription = "Prépare la date et les coordonnées GPS affichées";
        bOpen.Text = "FICHIERS…";
        bGPS.Text = "RECHERCHER";
        WireCommands();
        UpdateMapChecks();
        _map.BringToFront();
        _map.MapLocationChanged += async (_, point) => await SetLocationFromMapAsync(point.Latitude, point.Longitude);
        Shown += async (_, _) =>
        {
            try { await _map.InitializeAsync(_settings.MapTileUrl, _settings.MapAttribution); }
            catch (Exception ex) { AppLogger.Error("Map initialization failed.", ex); MessageBox.Show(ex.Message, "Map unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            if (_settings.CheckForUpdatesAutomatically)
                await _updates.CheckAndPromptAsync(this, manual: false);
        };
        _session.PropertyChanged += (_, _) => { UpdateSessionCaption(); RefreshFilter(); };
        RefreshFilter();
        UpdateSessionCaption();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        var selected = SelectedItems;
        if (selected.Count == 0)
        {
            MessageBox.Show("Sélectionnez au moins une image.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _sessionController.StageVisibleValues(selected, dateTimePicker1.Value, _currentLocation, _locations);
        if (_currentLocation is { } location)
        {
            if (!string.IsNullOrWhiteSpace(_currentLocationName))
                foreach (var item in selected)
                    item.SetResolvedLocation(location.Latitude, location.Longitude, _currentLocationName);
            else
                QueueLocationResolution(selected);
        }
        RefreshMapMarkers();
        operationStatus.Text = _currentLocation is null
            ? $"Date préparée pour {selected.Count} fichier(s)."
            : $"Date et localisation préparées pour {selected.Count} fichier(s).";
    }

    private async void button2_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            InitialDirectory = Environment.CurrentDirectory,
            Filter = _discovery.DialogFilter,
            FilterIndex = 1,
            RestoreDirectory = true,
            Multiselect = true,
            Title = "Select photos and videos..."
        };
        if (dialog.ShowDialog() == DialogResult.OK) await AddFilesAsync(dialog.FileNames);
    }

    private async Task OpenFolderAsync()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select a folder containing photos or videos",
            ShowNewFolderButton = false,
            UseDescriptionForTitle = true
        };
        if (dialog.ShowDialog(this) == DialogResult.OK) await AddFilesAsync(new[] { dialog.SelectedPath });
    }

    private void Form1_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true) e.Effect = DragDropEffects.Copy;
    }

    private async void Form1_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] paths) await AddFilesAsync(paths);
    }

    private async Task AddFilesAsync(IEnumerable<string> paths)
    {
        try
        {
            SetBusy(true);
            var ct = StartOperation();
            var discovery = await _discovery.DiscoverAsync(paths, _settings.RecursiveImport, ct);
            foreach (var error in discovery.Errors) AppLogger.Info(error);
            if (discovery.Errors.Count > 0)
            {
                var details = string.Join(Environment.NewLine, discovery.Errors.Take(8));
                if (discovery.Errors.Count > 8) details += $"{Environment.NewLine}… and {discovery.Errors.Count - 8} more.";
                MessageBox.Show(details, "Some paths could not be imported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            var existing = _files.Select(item => item.FilePath).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var newPaths = discovery.Files.Where(path => !existing.Contains(path)).ToList();
            if (newPaths.Count == 0) return;
            pgb.Value = 5;
            var progress = new Progress<int>(value => pgb.Value = Math.Clamp(5 + value * 95 / 100, 0, 100));
            var items = await _metadata.LoadAsync(newPaths, progress, ct);
            _session.AddRange(items);
            QueueLocationResolution(items);
        }
        catch (OperationCanceledException) { AppLogger.Info("Import cancelled."); }
        catch (Exception ex) { AppLogger.Error("Import failed.", ex); MessageBox.Show(ex.Message, "Import error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { SetBusy(false); }
    }

    private async Task ApplyPendingChangesAsync(IReadOnlyList<PhotoItem> photos)
    {
        var preview = _metadata.Preview(photos);
        if (preview.FileCount == 0) return;
        using (var previewDialog = new ApplyPreviewForm(preview))
        {
            if (previewDialog.ShowDialog(this) != DialogResult.OK || !previewDialog.Confirmed) return;
        }

        try
        {
            SetBusy(true);
            var ct = StartOperation();
            var progress = new Progress<int>(value => pgb.Value = value);
            var result = await _metadata.ApplyPendingChangesAsync(photos, progress, ct);
            var succeeded = photos.Where(photo => result.Files.Any(file => file.Succeeded && file.FilePath.Equals(photo.FilePath, StringComparison.OrdinalIgnoreCase))).ToList();
            _history.Forget(succeeded);
            foreach (var item in succeeded)
            {
                _thumbnails.Invalidate(item.FilePath);
                _informationCache.TryRemove(item.FilePath, out _);
            }
            _session.NotifyChanged();
            QueueLocationResolution(succeeded);
            using var report = new ApplyReportForm(result);
            report.ShowDialog(this);
        }
        catch (OperationCanceledException) { AppLogger.Info("Apply cancelled."); }
        catch (Exception ex)
        {
            AppLogger.Error("Apply failed.", ex);
            MessageBox.Show(ex.Message, "Apply error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally { SetBusy(false); }
    }

    private async void bGPS_Click(object? sender, EventArgs e)
    {
        await SearchGpsSuggestionsAsync(showNoResultMessage: true);
    }

    private void ScheduleGpsSearch()
    {
        if (_updatingGpsSuggestions) return;
        _currentLocation = null;
        _currentLocationName = string.Empty;
        _currentLocationLookupCts?.Cancel();
        tName.Clear();
        _gpsSearchTimer.Stop();
        _gpsSearchCts?.Cancel();
        ClearGpsSuggestions();
        if (tGPS.Text.Trim().Length < 2)
        {
            UpdateCommandState();
            return;
        }
        _gpsSearchTimer.Start();
        UpdateCommandState();
    }

    private async Task SearchGpsSuggestionsAsync(bool showNoResultMessage)
    {
        _gpsSearchTimer.Stop();
        var query = tGPS.Text.Trim();
        if (query.Length < 2)
        {
            if (showNoResultMessage)
                MessageBox.Show("Saisissez au moins deux caractères pour rechercher un lieu.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _gpsSearchCts?.Cancel();
        _gpsSearchCts?.Dispose();
        var cts = new CancellationTokenSource();
        _gpsSearchCts = cts;
        _gpsSearchInProgress = true;
        operationStatus.Text = "Recherche du lieu…";
        UpdateCommandState();
        try
        {
            var results = await _geocoding.SearchAsync(query, cts.Token);
            if (cts.IsCancellationRequested || !query.Equals(tGPS.Text.Trim(), StringComparison.Ordinal)) return;
            PopulateGpsSuggestions(results, query);
            operationStatus.Text = results.Count == 0 ? "Aucun lieu trouvé." : $"{results.Count} lieu(x) proposé(s).";
            if (results.Count == 0 && showNoResultMessage)
                MessageBox.Show("Aucun lieu trouvé.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (OperationCanceledException) { AppLogger.Info("Geocoding cancelled."); }
        catch (Exception ex)
        {
            AppLogger.Error("Geocoding failed.", ex);
            operationStatus.Text = "Échec de la recherche du lieu.";
            if (showNoResultMessage)
                MessageBox.Show(ex.Message, "Geocoding error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            if (ReferenceEquals(_gpsSearchCts, cts))
            {
                _gpsSearchInProgress = false;
                UpdateCommandState();
            }
        }
    }

    private void InitializeGpsSuggestionsPopup()
    {
        var host = new ToolStripControlHost(_gpsSuggestions) { AutoSize = false, Margin = Padding.Empty, Padding = System.Windows.Forms.Padding.Empty };
        _gpsSuggestionsPopup.Items.Add(host);
        _gpsSuggestions.MouseClick += (_, _) => SelectGpsSuggestion();
    }

    private void PopulateGpsSuggestions(IReadOnlyList<Coordinates> results, string query)
    {
        if (!tGPS.Text.Trim().Equals(query, StringComparison.Ordinal)) return;
        var selectionStart = tGPS.SelectionStart;
        var selectionLength = tGPS.SelectionLength;
        _gpsSuggestions.BeginUpdate();
        try
        {
            _gpsSuggestions.Items.Clear();
            foreach (var result in results) _gpsSuggestions.Items.Add(result);
            _gpsSuggestions.SelectedIndex = -1;
        }
        finally { _gpsSuggestions.EndUpdate(); }

        if (results.Count == 0 || !tGPS.Focused)
        {
            _gpsSuggestionsPopup.Close();
            return;
        }
        var width = Math.Max(tGPS.Width, 280);
        var height = Math.Min(240, Math.Max(_gpsSuggestions.ItemHeight, results.Count * _gpsSuggestions.ItemHeight + 4));
        _gpsSuggestions.Size = new Size(width, height);
        if (_gpsSuggestionsPopup.Items[0] is ToolStripControlHost host) host.Size = _gpsSuggestions.Size;
        _gpsSuggestionsPopup.Size = _gpsSuggestions.Size;
        if (!_gpsSuggestionsPopup.Visible) _gpsSuggestionsPopup.Show(tGPS, new Point(0, tGPS.Height));
        tGPS.Focus();
        tGPS.SelectionStart = Math.Clamp(selectionStart, 0, tGPS.Text.Length);
        tGPS.SelectionLength = Math.Clamp(selectionLength, 0, tGPS.Text.Length - tGPS.SelectionStart);
    }

    private void ClearGpsSuggestions()
    {
        _gpsSuggestionsPopup.Close();
        _gpsSuggestions.Items.Clear();
    }

    private void SelectGpsSuggestion()
    {
        if (_gpsSuggestions.SelectedItem is not Coordinates selected) return;
        _gpsSuggestionsPopup.Close();
        SetGpsSearchTextSilently(selected.Name);
        SetCurrentLocation(new GpsCoordinate(selected.Latitude, selected.Longitude, selected.Altitude), selected.Name);
        operationStatus.Text = "Lieu sélectionné. Sélectionnez les médias puis cliquez sur PRÉPARER.";
    }

    private void GpsSearchKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape && _gpsSuggestionsPopup.Visible)
        {
            _gpsSuggestionsPopup.Close();
            e.SuppressKeyPress = true;
            return;
        }
        if (!_gpsSuggestionsPopup.Visible || _gpsSuggestions.Items.Count == 0) return;
        if (e.KeyCode == Keys.Down)
        {
            _gpsSuggestions.SelectedIndex = Math.Min(_gpsSuggestions.Items.Count - 1, _gpsSuggestions.SelectedIndex + 1);
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Up)
        {
            _gpsSuggestions.SelectedIndex = Math.Max(0, _gpsSuggestions.SelectedIndex - 1);
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.Enter && _gpsSuggestions.SelectedIndex >= 0)
        {
            SelectGpsSuggestion();
            e.SuppressKeyPress = true;
        }
    }

    private void SetCurrentLocation(GpsCoordinate location, string? name)
    {
        _currentLocation = location;
        _currentLocationName = name?.Trim() ?? string.Empty;
        tName.Text = string.IsNullOrWhiteSpace(_currentLocationName) ? "Identification…" : _currentLocationName;
        UpdateCommandState();
    }

    private void SetGpsSearchTextSilently(string text)
    {
        _updatingGpsSuggestions = true;
        try
        {
            tGPS.Text = text;
            tGPS.SelectionStart = tGPS.Text.Length;
            tGPS.SelectionLength = 0;
        }
        finally { _updatingGpsSuggestions = false; }
    }

    private void dgv_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Delete) return;
        foreach (var item in SelectedItems) _session.Remove(item);
        e.Handled = true;
    }

    private async void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem is not PhotoItem item) return;
        DisplayActiveMetadata(item);
        if (_map.Visible) RefreshMapMarkers();
        try
        {
            var image = await _thumbnails.GetAsync(item.FilePath, 1600, _operationCts?.Token ?? CancellationToken.None);
            if (IsDisposed) { image.Dispose(); return; }
            var previous = picBox.Image;
            picBox.Image = image;
            previous?.Dispose();
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            AppLogger.Error($"Preview failed for {item.FilePath}.", ex);
            operationStatus.Text = "Aperçu indisponible — consultez les journaux.";
        }
    }

    private async void dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex != thumbnailColumn.Index || dgv.Rows[e.RowIndex].DataBoundItem is not PhotoItem item) return;
        if (_gridThumbnails.TryGetValue(item.FilePath, out var cached))
        {
            e.Value = cached;
            e.FormattingApplied = true;
            return;
        }
        if (!_thumbnailLoads.TryAdd(item.FilePath, 0)) return;
        try
        {
            var image = await _thumbnails.GetAsync(item.FilePath, 96, _operationCts?.Token ?? CancellationToken.None);
            if (IsDisposed) { image.Dispose(); return; }
            _gridThumbnails[item.FilePath] = image;
            if (_gridThumbnails.Count > 300)
            {
                var removable = _gridThumbnails.Keys.FirstOrDefault(key => !key.Equals(item.FilePath, StringComparison.OrdinalIgnoreCase));
                if (removable is not null && _gridThumbnails.TryRemove(removable, out var removed)) removed.Dispose();
            }
            var row = dgv.Rows.Cast<DataGridViewRow>().FirstOrDefault(candidate => ReferenceEquals(candidate.DataBoundItem, item));
            if (row is not null) dgv.InvalidateRow(row.Index);
        }
        catch (OperationCanceledException) { }
        finally { _thumbnailLoads.TryRemove(item.FilePath, out _); }
    }

    private void dgv_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
    {
        if (sender is not DataGridView grid || !grid.RowHeadersVisible) return;
        var rectangle = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
        TextRenderer.DrawText(e.Graphics, e.RowIndex.ToString(), grid.RowHeadersDefaultCellStyle.Font, rectangle, grid.RowHeadersDefaultCellStyle.ForeColor);
    }

    protected override bool ProcessCmdKey(ref Message message, Keys keyData)
    {
        if (keyData == Keys.Escape)
        {
            var cancelled = false;
            if (_operationCts is { IsCancellationRequested: false }) { _operationCts.Cancel(); cancelled = true; }
            if (_gpsSearchCts is { IsCancellationRequested: false }) { _gpsSearchCts.Cancel(); cancelled = true; }
            if (cancelled) return true;
        }
        if (_isBusy) return base.ProcessCmdKey(ref message, keyData);
        if (keyData == (Keys.Control | Keys.A)) { dgv.SelectAll(); return true; }
        if (keyData == (Keys.Control | Keys.Z)) { UndoPendingChanges(); return true; }
        if (keyData == (Keys.Control | Keys.Y)) { RedoPendingChanges(); return true; }
        return base.ProcessCmdKey(ref message, keyData);
    }

    private ToolStripItem Command(string name)
    {
        static ToolStripItem? Find(ToolStripItemCollection items, string target)
        {
            foreach (ToolStripItem item in items)
            {
                if (item.Name == target) return item;
                if (item is ToolStripDropDownItem dropDown && Find(dropDown.DropDownItems, target) is { } nested) return nested;
            }
            return null;
        }

        return Find(commands.Items, name) ?? Find(navigationMenu.Items, name) ?? throw new InvalidOperationException($"Command {name} not found.");
    }

    private void WireCommands()
    {
        _gpsSearchTimer.Tick += async (_, _) =>
        {
            _gpsSearchTimer.Stop();
            await SearchGpsSuggestionsAsync(showNoResultMessage: false);
        };
        tGPS.TextChanged += (_, _) => ScheduleGpsSearch();
        tGPS.KeyDown += GpsSearchKeyDown;
        Command("applyCommand").Click += async (_, _) => await ApplyPendingChangesAsync(_session.Media.ToList());
        applyMenuItem.Click += async (_, _) => await ApplyPendingChangesAsync(_session.Media.ToList());
        openFilesMenuItem.Click += button2_Click;
        openFilesQuickItem.Click += button2_Click;
        Command("openFolderCommand").Click += async (_, _) => await OpenFolderAsync();
        openFolderQuickItem.Click += async (_, _) => await OpenFolderAsync();
        Command("dateEditorCommand").Click += (_, _) => OpenDateEditor();
        dateQuickCommand.Click += (_, _) => OpenDateEditor();
        Command("settingsCommand").Click += async (_, _) => await OpenSettingsAsync();
        Command("cancelCommand").Click += (_, _) => _operationCts?.Cancel();
        cancelMenuItem.Click += (_, _) => _operationCts?.Cancel();
        Command("undoCommand").Click += (_, _) => UndoPendingChanges();
        Command("redoCommand").Click += (_, _) => RedoPendingChanges();
        undoMenuItem.Click += (_, _) => UndoPendingChanges();
        redoMenuItem.Click += (_, _) => RedoPendingChanges();
        Command("resetSelectedCommand").Click += (_, _) => ResetPatches(SelectedItems);
        Command("resetAllCommand").Click += (_, _) => ResetPatches(_session.Media);
        Command("minusHourCommand").Click += (_, _) => ShiftSelected(TimeSpan.FromHours(-1));
        Command("plusHourCommand").Click += (_, _) => ShiftSelected(TimeSpan.FromHours(1));
        Command("minusMinuteCommand").Click += (_, _) => ShiftSelected(TimeSpan.FromMinutes(-1));
        Command("plusMinuteCommand").Click += (_, _) => ShiftSelected(TimeSpan.FromMinutes(1));
        Command("removeGpsCommand").Click += (_, _) => RemoveGpsSelected();
        Command("copyGpsCommand").Click += (_, _) => CopyGpsSelected();
        Command("pasteGpsCommand").Click += (_, _) => PasteGpsSelected();
        Command("reverseGpsCommand").Click += async (_, _) => await ReverseGpsSelectedAsync();
        findGpsMenuItem.Click += bGPS_Click;
        findGpsQuickItem.Click += bGPS_Click;
        copyGpsQuickItem.Click += (_, _) => CopyGpsSelected();
        pasteGpsQuickItem.Click += (_, _) => PasteGpsSelected();
        removeGpsQuickItem.Click += (_, _) => RemoveGpsSelected();
        reverseGpsQuickItem.Click += async (_, _) => await ReverseGpsSelectedAsync();
        Command("mapCommand").Click += (_, _) => ToggleMap();
        mapQuickCommand.Click += (_, _) => ToggleMap();
        previewMenuItem.Click += (_, _) => ShowPreview();
        informationMenuItem.Click += async (_, _) => await ShowInformationAsync();
        quickActionsMenuItem.Click += (_, _) => ToggleQuickActions();
        Command("allFilterCommand").Click += (_, _) => ApplyFilter("Tous", _ => true);
        Command("modifiedFilterCommand").Click += (_, _) => ApplyFilter("Modifiés", item => item.PendingChanges.HasChanges);
        Command("noGpsFilterCommand").Click += (_, _) => ApplyFilter("Sans GPS", item => !item.EffectiveLatitude.HasValue || !item.EffectiveLongitude.HasValue);
        Command("noDateFilterCommand").Click += (_, _) => ApplyFilter("Sans date", item => !item.EffectiveCaptureDate.HasValue);
        Command("errorsFilterCommand").Click += (_, _) => ApplyFilter("Erreurs", item => item.Error is not null);
        allFilterQuickItem.Click += (_, _) => ApplyFilter("Tous", _ => true);
        modifiedFilterQuickItem.Click += (_, _) => ApplyFilter("Modifiés", item => item.PendingChanges.HasChanges);
        noGpsFilterQuickItem.Click += (_, _) => ApplyFilter("Sans GPS", item => !item.EffectiveLatitude.HasValue || !item.EffectiveLongitude.HasValue);
        noDateFilterQuickItem.Click += (_, _) => ApplyFilter("Sans date", item => !item.EffectiveCaptureDate.HasValue);
        errorsFilterQuickItem.Click += (_, _) => ApplyFilter("Erreurs", item => item.Error is not null);
        Command("restoreBackupCommand").Click += async (_, _) => await RestoreSelectedAsync();
        selectAllMenuItem.Click += (_, _) => dgv.SelectAll();
        removeFromSessionMenuItem.Click += (_, _) => RemoveSelectedFromSession();
        exitMenuItem.Click += (_, _) => Close();
        guideMenuItem.Click += (_, _) => OpenExternal("https://github.com/fatvicbart/exif-tweaker/blob/main/GUIDE_UTILISATEUR.md");
        logsMenuItem.Click += (_, _) => OpenLogsDirectory();
        verifyExifToolMenuItem.Click += async (_, _) => await VerifyExifToolAsync();
        checkUpdatesMenuItem.Click += async (_, _) => await CheckForUpdatesAsync();
        aboutMenuItem.Click += (_, _) => ShowAbout();
        dgv.SelectionChanged += async (_, _) => await ActiveSelectionChangedAsync();
    }

    private async Task ActiveSelectionChangedAsync()
    {
        UpdateCommandState();
        _activeItem = dgv.CurrentRow?.DataBoundItem as PhotoItem;
        if (_activeItem is null)
        {
            _informationCts?.Cancel();
            _informationView.ShowEmpty();
            return;
        }
        DisplayActiveMetadata(_activeItem);
        if (_informationView.Visible) await LoadInformationAsync(_activeItem);
    }

    private void ToggleQuickActions()
    {
        commands.Visible = !commands.Visible;
        quickActionsMenuItem.Checked = commands.Visible;
    }

    private async Task CheckForUpdatesAsync()
    {
        checkUpdatesMenuItem.Enabled = false;
        try { await _updates.CheckAndPromptAsync(this, manual: true); }
        finally { if (!IsDisposed) checkUpdatesMenuItem.Enabled = true; }
    }

    private void ApplyFilter(string name, Func<PhotoItem, bool> predicate)
    {
        _activeFilterName = name;
        _activeFilter = predicate;
        RefreshFilter();
    }

    private void RefreshFilter()
    {
        var desired = _session.Media.Where(_activeFilter).ToList();
        for (var index = _view.Count - 1; index >= 0; index--)
            if (!desired.Contains(_view[index])) _view.RemoveAt(index);
        for (var index = 0; index < desired.Count; index++)
        {
            if (index < _view.Count && ReferenceEquals(_view[index], desired[index])) continue;
            var existing = _view.IndexOf(desired[index]);
            if (existing >= 0) _view.RemoveAt(existing);
            _view.Insert(Math.Min(index, _view.Count), desired[index]);
        }
        _bindingSource.ResetBindings(false);
        if (filterQuickCommand is not null) filterQuickCommand.Text = $"Filtre : {_activeFilterName} ({_view.Count}/{_session.Media.Count})";
        UpdateFilterChecks();
    }

    private void OpenDateEditor()
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        var dates = selected.Select(item => item.EffectiveCaptureDate).Distinct().ToList();
        var offsets = selected.Select(item => item.EffectiveOffset).Distinct().ToList();
        using var dialog = new DateEditorForm(dates.Count == 1 ? dates[0] : null, offsets.Count == 1 ? offsets[0] : null, dates.Count > 1 || offsets.Count > 1);
        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.Request is not null)
            _sessionController.EditDate(selected, dialog.Request);
    }

    private async Task OpenSettingsAsync()
    {
        using var dialog = new SettingsForm(_settings);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try { await _map.InitializeAsync(_settings.MapTileUrl, _settings.MapAttribution); }
        catch (Exception ex) { AppLogger.Error("Map reconfiguration failed.", ex); }
        MessageBox.Show("Settings saved. Restart the application after changing the ExifTool path.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ToggleMap()
    {
        var showMap = !_map.Visible;
        _informationView.Visible = false;
        _map.Visible = showMap;
        if (showMap) _map.BringToFront();
        else picBox.BringToFront();
        UpdateMapChecks();
        RefreshMapMarkers();
    }

    private void ShowPreview()
    {
        _map.Visible = false;
        _informationView.Visible = false;
        picBox.BringToFront();
        UpdateMapChecks();
    }

    private async Task ShowInformationAsync()
    {
        _map.Visible = false;
        _informationView.Visible = true;
        _informationView.BringToFront();
        UpdateMapChecks();
        _activeItem ??= dgv.CurrentRow?.DataBoundItem as PhotoItem;
        if (_activeItem is null) _informationView.ShowEmpty();
        else await LoadInformationAsync(_activeItem);
    }

    private async Task LoadInformationAsync(PhotoItem item)
    {
        _informationCts?.Cancel();
        _informationCts?.Dispose();
        var cts = new CancellationTokenSource();
        _informationCts = cts;
        _informationView.ShowLoading(item.FilePath);
        try
        {
            var lastWriteUtc = File.GetLastWriteTimeUtc(item.FilePath);
            if (!_informationCache.TryGetValue(item.FilePath, out var cached) || cached.LastWriteUtc != lastWriteUtc)
            {
                var tags = await _exifTool.ReadAllMetadataAsync(item.FilePath, cts.Token);
                cached = (lastWriteUtc, tags);
                _informationCache[item.FilePath] = cached;
            }
            if (cts.IsCancellationRequested || !ReferenceEquals(_activeItem, item) || !_informationView.Visible) return;
            _informationView.ShowTags(item.FilePath, cached.Tags);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            AppLogger.Error($"Full metadata read failed for {item.FilePath}.", ex);
            if (!cts.IsCancellationRequested && ReferenceEquals(_activeItem, item))
                _informationView.ShowError(item.FilePath, "Impossible de lire les métadonnées — consultez les journaux.");
        }
    }

    private async Task SetLocationFromMapAsync(double latitude, double longitude)
    {
        var location = new GpsCoordinate(latitude, longitude);
        SetCurrentLocation(location, null);
        operationStatus.Text = "Identification du point sélectionné…";
        await IdentifyCurrentLocationAsync(location, showNoResultMessage: false);
    }

    private void RemoveGpsSelected()
    {
        var selected = SelectedItems;
        _sessionController.RemoveLocation(selected, _locations);
        RefreshMapMarkers();
    }

    private void CopyGpsSelected()
    {
        var active = SelectedItems.FirstOrDefault();
        if (active is null) return;
        try
        {
            _gpsClipboard = LocationEditorService.CopyLocation(active);
            operationStatus.Text = "Coordonnées GPS copiées.";
            UpdateCommandState();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void PasteGpsSelected()
    {
        if (_gpsClipboard is not { } gps || SelectedItems.Count == 0) return;
        var selected = SelectedItems;
        _sessionController.SetLocation(selected, gps.Latitude, gps.Longitude, gps.Altitude, _locations);
        SetCurrentLocation(gps, null);
        QueueLocationResolution(selected);
        RefreshMapMarkers();
    }

    private async Task ReverseGpsSelectedAsync()
    {
        var active = SelectedItems.FirstOrDefault();
        var location = _currentLocation ??
            (active?.EffectiveLatitude is double latitude && active.EffectiveLongitude is double longitude
                ? new GpsCoordinate(latitude, longitude, active.EffectiveAltitude)
                : null);
        if (location is null)
        {
            MessageBox.Show("Choisissez un point sur la carte ou sélectionnez une photo géolocalisée.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        SetCurrentLocation(location, null);
        await IdentifyCurrentLocationAsync(location, showNoResultMessage: true);
    }

    private async Task IdentifyCurrentLocationAsync(GpsCoordinate location, bool showNoResultMessage)
    {
        _currentLocationLookupCts?.Cancel();
        _currentLocationLookupCts?.Dispose();
        var cts = new CancellationTokenSource();
        _currentLocationLookupCts = cts;
        try
        {
            var result = await _geocoding.ReverseAsync(location.Latitude, location.Longitude, cts.Token);
            if (cts.IsCancellationRequested || _currentLocation != location) return;
            if (result is null)
            {
                tName.Text = "Adresse introuvable";
                operationStatus.Text = "Aucune adresse trouvée pour ce point.";
                if (showNoResultMessage)
                    MessageBox.Show("Aucune adresse trouvée pour ces coordonnées.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetCurrentLocation(location with { Altitude = result.Altitude ?? location.Altitude }, result.Name);
            SetGpsSearchTextSilently(result.Name);
            CacheLocationAddress(location.Latitude, location.Longitude, result.Name);
            foreach (var item in SelectedItems.Where(item => CoordinatesMatch(item, location.Latitude, location.Longitude)))
                item.SetResolvedLocation(location.Latitude, location.Longitude, result.Name);
            operationStatus.Text = "Adresse identifiée. Cliquez sur PRÉPARER pour l’appliquer.";
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            AppLogger.Error("Reverse geocoding failed.", ex);
            tName.Text = "Identification impossible";
            operationStatus.Text = "Impossible d’identifier ce point.";
            if (showNoResultMessage)
                MessageBox.Show(ex.Message, "Geocoding error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DisplayActiveMetadata(PhotoItem item)
    {
        if (item.EffectiveCaptureDate is DateTime captureDate) dateTimePicker1.Value = captureDate;
    }

    private void QueueLocationResolution(IEnumerable<PhotoItem> items)
    {
        var targets = items
            .Where(item => item.EffectiveLatitude.HasValue && item.EffectiveLongitude.HasValue)
            .ToList();
        if (targets.Count == 0 || _locationResolutionCts.IsCancellationRequested) return;
        _ = ResolveLocationAddressesAsync(targets, _locationResolutionCts.Token);
    }

    private async Task ResolveLocationAddressesAsync(IReadOnlyList<PhotoItem> items, CancellationToken ct)
    {
        var lockAcquired = false;
        try
        {
            await _locationResolutionLock.WaitAsync(ct);
            lockAcquired = true;
            foreach (var group in items
                         .Where(item => item.EffectiveLatitude.HasValue && item.EffectiveLongitude.HasValue)
                         .GroupBy(item => LocationCacheKey(item.EffectiveLatitude!.Value, item.EffectiveLongitude!.Value)))
            {
                ct.ThrowIfCancellationRequested();
                var sample = group.First();
                var latitude = sample.EffectiveLatitude!.Value;
                var longitude = sample.EffectiveLongitude!.Value;
                if (!_locationAddressCache.TryGetValue(group.Key, out var address))
                {
                    try
                    {
                        var result = await _geocoding.ReverseAsync(latitude, longitude, ct);
                        address = result?.Name?.Trim() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(address))
                            _locationAddressCache[group.Key] = address;
                        if (_settings.GeocodingProvider.Equals("Nominatim", StringComparison.OrdinalIgnoreCase))
                            await Task.Delay(1100, ct);
                    }
                    catch (OperationCanceledException) { throw; }
                    catch (Exception ex)
                    {
                        AppLogger.Error($"Unable to identify {latitude:F6}, {longitude:F6}.", ex);
                        address = "Adresse indisponible";
                    }
                }

                if (string.IsNullOrWhiteSpace(address)) address = "Adresse indisponible";
                foreach (var item in group.Where(item => CoordinatesMatch(item, latitude, longitude)))
                    item.SetResolvedLocation(latitude, longitude, address);
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            if (lockAcquired) _locationResolutionLock.Release();
        }
    }

    private void CacheLocationAddress(double latitude, double longitude, string address)
    {
        if (!string.IsNullOrWhiteSpace(address))
            _locationAddressCache[LocationCacheKey(latitude, longitude)] = address.Trim();
    }

    private static string LocationCacheKey(double latitude, double longitude) =>
        $"{latitude:F6},{longitude:F6}";

    private static bool CoordinatesMatch(PhotoItem item, double latitude, double longitude) =>
        item.EffectiveLatitude is double itemLatitude && item.EffectiveLongitude is double itemLongitude &&
        Math.Abs(itemLatitude - latitude) < 0.0000005 && Math.Abs(itemLongitude - longitude) < 0.0000005;

    private void RefreshMapMarkers()
    {
        if (!_map.Visible) return;
        var active = SelectedItems.FirstOrDefault();
        var markers = _session.Media
            .Where(item => item.EffectiveLatitude.HasValue && item.EffectiveLongitude.HasValue)
            .Select(item => new MapMarker(item.EffectiveLatitude!.Value, item.EffectiveLongitude!.Value, item.FileName, ReferenceEquals(item, active)))
            .ToList();
        _ = _map.SetMarkersAsync(markers, _session.Media.Count - markers.Count);
    }

    private void ShiftSelected(TimeSpan shift)
    {
        var selected = SelectedItems;
        _sessionController.ShiftDate(selected, shift);
    }

    private async Task RestoreSelectedAsync()
    {
        var selected = SelectedItems;
        if (selected.Count == 0 || MessageBox.Show("Restore selected files from their ExifTool backups?", "Restore backup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        SetBusy(true);
        try
        {
            var result = await _metadata.RestoreBackupsAsync(selected, StartOperation());
            foreach (var item in selected.Where(item => result.Files.Any(file => file.Succeeded && file.FilePath.Equals(item.FilePath, StringComparison.OrdinalIgnoreCase))))
            {
                _thumbnails.Invalidate(item.FilePath);
                _informationCache.TryRemove(item.FilePath, out _);
            }
            _history.Forget(selected.Where(item => result.Files.Any(file => file.Succeeded && file.FilePath.Equals(item.FilePath, StringComparison.OrdinalIgnoreCase))));
            _session.NotifyChanged();
            QueueLocationResolution(selected);
            using var report = new ApplyReportForm(result) { Text = "Restore report" };
            report.ShowDialog(this);
        }
        catch (OperationCanceledException) { AppLogger.Info("Restore cancelled."); }
        catch (Exception ex)
        {
            AppLogger.Error("Restore failed.", ex);
            MessageBox.Show(ex.Message, "Restore error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally { SetBusy(false); }
    }

    private void ResetPatches(IEnumerable<PhotoItem> items)
    {
        var list = items.ToList();
        _sessionController.Reset(list);
        QueueLocationResolution(list);
    }


    private void UndoPendingChanges()
    {
        if (!_history.Undo(_session.Media)) return;
        _session.NotifyChanged();
        QueueLocationResolution(_session.Media);
    }

    private void RedoPendingChanges()
    {
        if (!_history.Redo(_session.Media)) return;
        _session.NotifyChanged();
        QueueLocationResolution(_session.Media);
    }

    private void RemoveSelectedFromSession()
    {
        foreach (var item in SelectedItems) _session.Remove(item);
    }

    private void UpdateFilterChecks()
    {
        if (allFilterQuickItem is null) return;
        allFilterCommand.Checked = allFilterQuickItem.Checked = _activeFilterName == "Tous";
        modifiedFilterCommand.Checked = modifiedFilterQuickItem.Checked = _activeFilterName == "Modifiés";
        noGpsFilterCommand.Checked = noGpsFilterQuickItem.Checked = _activeFilterName == "Sans GPS";
        noDateFilterCommand.Checked = noDateFilterQuickItem.Checked = _activeFilterName == "Sans date";
        errorsFilterCommand.Checked = errorsFilterQuickItem.Checked = _activeFilterName == "Erreurs";
    }

    private void UpdateMapChecks()
    {
        if (mapQuickCommand is null) return;
        mapQuickCommand.Checked = _map.Visible;
        mapCommand.Checked = _map.Visible;
        informationMenuItem.Checked = _informationView.Visible;
        previewMenuItem.Checked = !_map.Visible && !_informationView.Visible;
    }

    private void UpdateCommandState()
    {
        if (applyMenuItem is null) return;
        var pending = _session.PendingChangeCount;
        var hasMedia = _session.Media.Count > 0;
        var hasSelection = SelectedItems.Count > 0;
        var canEditSelection = !_isBusy && hasSelection;
        var applyText = $"Vérifier et appliquer tout ({pending})";
        applyCommand.Text = applyText;
        applyMenuItem.Text = applyText;
        applyCommand.Enabled = !_isBusy && pending > 0;
        applyMenuItem.Enabled = !_isBusy && pending > 0;
        undoCommand.Enabled = undoMenuItem.Enabled = !_isBusy && _history.CanUndo;
        redoCommand.Enabled = redoMenuItem.Enabled = !_isBusy && _history.CanRedo;
        cancelCommand.Enabled = cancelMenuItem.Enabled = _isBusy;

        bChange.Enabled = canEditSelection;
        dateEditorCommand.Enabled = dateQuickCommand.Enabled = canEditSelection;
        resetSelectedCommand.Enabled = canEditSelection;
        resetAllCommand.Enabled = !_isBusy && pending > 0;
        minusHourCommand.Enabled = plusHourCommand.Enabled = canEditSelection;
        minusMinuteCommand.Enabled = plusMinuteCommand.Enabled = canEditSelection;
        copyGpsCommand.Enabled = copyGpsQuickItem.Enabled = canEditSelection;
        pasteGpsCommand.Enabled = pasteGpsQuickItem.Enabled = canEditSelection && _gpsClipboard is not null;
        removeGpsCommand.Enabled = removeGpsQuickItem.Enabled = canEditSelection;
        restoreBackupCommand.Enabled = removeFromSessionMenuItem.Enabled = canEditSelection;
        selectAllMenuItem.Enabled = !_isBusy && hasMedia;

        var canSearchGps = !_isBusy && !_gpsSearchInProgress && tGPS.Text.Trim().Length >= 2;
        bGPS.Enabled = findGpsMenuItem.Enabled = findGpsQuickItem.Enabled = canSearchGps;
        var canIdentifyLocation = _currentLocation is not null ||
            SelectedItems.Any(item => item.EffectiveLatitude.HasValue && item.EffectiveLongitude.HasValue);
        reverseGpsCommand.Enabled = reverseGpsQuickItem.Enabled = !_isBusy && canIdentifyLocation;
        operationStatus.Enabled = true;
    }

    private void OpenLogsDirectory()
    {
        var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExifTweaker", "logs");
        Directory.CreateDirectory(directory);
        OpenExternal(directory);
    }

    private static void OpenExternal(string target)
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(target) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Unable to open {target}.", ex);
            MessageBox.Show(ex.Message, "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async Task VerifyExifToolAsync()
    {
        try
        {
            SetBusy(true);
            var version = await _exifTool.GetVersionAsync(StartOperation());
            MessageBox.Show($"ExifTool {version} is available.\n\n{_exifTool.ExecutablePath}", "ExifTool verification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (OperationCanceledException)
        {
            AppLogger.Info("ExifTool verification cancelled.");
        }
        catch (Exception ex)
        {
            AppLogger.Error("ExifTool verification failed.", ex);
            MessageBox.Show(ex.Message, "ExifTool unavailable", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private static void ShowAbout()
    {
        var version = typeof(Form1).Assembly.GetName().Version?.ToString(3) ?? "unknown";
        MessageBox.Show($"ExifTweaker {version}\n\nBatch date, timezone and GPS metadata editor powered by ExifTool.", "About ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void UpdateSessionCaption()
    {
        var statistics = _session.Statistics;
        var range = statistics.FirstCaptureDate is DateTime first && statistics.LastCaptureDate is DateTime last
            ? $" | {first:yyyy-MM-dd} to {last:yyyy-MM-dd}"
            : string.Empty;
        Text = $"ExifTweaker — {statistics.MediaCount} media | {statistics.FilesWithGps} GPS | {statistics.PendingChangeCount} pending{range}";
        UpdateCommandState();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing && _session.HasPendingChanges &&
            MessageBox.Show("Pending metadata changes have not been applied. Close anyway?", "ExifTweaker", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            e.Cancel = true;
            return;
        }
        _operationCts?.Cancel();
        _currentLocationLookupCts?.Cancel();
        _locationResolutionCts.Cancel();
        base.OnFormClosing(e);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _operationCts?.Dispose();
        _gpsSearchTimer.Stop();
        _gpsSearchTimer.Dispose();
        _gpsSuggestionsPopup.Dispose();
        _informationCts?.Cancel();
        _informationCts?.Dispose();
        _gpsSearchCts?.Cancel();
        _gpsSearchCts?.Dispose();
        _currentLocationLookupCts?.Dispose();
        _locationResolutionCts.Dispose();
        foreach (var image in _gridThumbnails.Values) image.Dispose();
        _gridThumbnails.Clear();
        picBox.Image?.Dispose();
        _thumbnails.Dispose();
        if (_geocoding is IDisposable disposable) disposable.Dispose();
        base.OnFormClosed(e);
    }

    private CancellationToken StartOperation()
    {
        _operationCts?.Cancel();
        _operationCts?.Dispose();
        _operationCts = new CancellationTokenSource();
        return _operationCts.Token;
    }

    private void SetBusy(bool busy)
    {
        _isBusy = busy;
        if (busy)
        {
            _gpsSearchTimer.Stop();
            _gpsSearchCts?.Cancel();
        }
        main.Enabled = !busy;
        foreach (ToolStripItem item in commands.Items) item.Enabled = !busy;
        foreach (ToolStripItem item in navigationMenu.Items) item.Enabled = !busy;
        actionsMenu.Enabled = true;
        cancelCommand.Enabled = busy;
        cancelMenuItem.Enabled = busy;
        operationStatus.Enabled = true;
        operationStatus.Text = busy ? "Working… (Esc to cancel)" : "Ready";
        if (busy) pgb.Value = 0;
        UpdateCommandState();
    }

}
