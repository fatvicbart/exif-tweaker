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
    private readonly ColumnFilterService _columnFilters = new();
    private string? _headerColumn;
    private BindingList<PhotoItem> _files => _session.Media;
    private readonly AppSettings _settings = AppSettings.Load();
    private readonly FileDiscoveryService _discovery = new();
    private readonly MetadataService _metadata;
    private readonly IGeocodingService _geocoding;
    private readonly UpdateService _updates;
    private bool _isBusy;
    private string _activeFilterName = "Tous";
    private GpsCoordinate? _gpsClipboard;
    private (DateTime Date, TimeSpan? Offset)? _dateClipboard;
    private readonly System.Windows.Forms.Timer _gpsSearchTimer = new() { Interval = 450 };
    private CancellationTokenSource? _gpsSearchCts;
    private bool _gpsSearchInProgress;
    private bool _updatingGpsSuggestions;
    private readonly ListBox _gpsSuggestions = new() { DisplayMember = nameof(Coordinates.Name), IntegralHeight = false };
    private readonly ToolStripDropDown _gpsSuggestionsPopup = new() { AutoClose = false, Padding = System.Windows.Forms.Padding.Empty };
    private GpsCoordinate? _currentLocation;
    private string _currentLocationName = string.Empty;
    private string _validatedGpsText = string.Empty;
    private CancellationTokenSource? _currentLocationLookupCts;
    private readonly CancellationTokenSource _locationResolutionCts = new();
    private readonly SemaphoreSlim _locationResolutionLock = new(1, 1);
    private readonly ConcurrentDictionary<string, string> _locationAddressCache = new();
    private CancellationTokenSource? _operationCts;
    private readonly InformationControl _informationView = new();
    private readonly ConcurrentDictionary<string, (DateTime LastWriteUtc, IReadOnlyList<ExifTagInfo> Tags)> _informationCache = new(StringComparer.OrdinalIgnoreCase);
    private CancellationTokenSource? _informationCts;
    private PhotoItem? _activeItem;
    private bool _sessionRefreshQueued;
    private bool _sessionRefreshPending;
    private bool _restoringGridSelection;
    private (string FilePath, DateTime LastWriteUtc)? _informationRequest;
    private int _boundItemsUpdateDepth;
    private bool _immichAlbumsLoaded;
    private bool _refreshInformationAfterBoundUpdate;

    private List<PhotoItem> SelectedItems => dgv.SelectedRows.Cast<DataGridViewRow>()
        .Select(row => row.DataBoundItem as PhotoItem)
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
        ThemeService.Apply(this);
        ThemeService.Apply(_gpsSuggestionsPopup);
        ThemeService.Apply(gridContextMenu);
        ThemeService.Apply(headerContextMenu);
        ThemeService.ThemeChanged += OnThemeChanged;
        _bindingSource.DataSource = _view;
        dgv.DataSource = _bindingSource;
        immichAlbum.Format += (_, e) => { if (e.ListItem is ImmichAlbum album) e.Value = album.Name; };
        immichAlbum.Items.Add(NoAlbumEntry);
        immichAlbum.Items.Add(NewAlbumEntry);
        immichAlbum.SelectedIndex = 0;
        WireCommands();
        UpdateMapChecks();
        _map.BringToFront();
        _map.MapLocationChanged += MapLocationChanged;
        Shown += Form1_Shown;
        _session.PropertyChanged += SessionPropertyChanged;

        RefreshFilter();
        UpdateSessionCaption();
    }
    private async void Form1_Shown(object? sender, EventArgs e)
    {
        ThemeService.Apply(this);
        try { await _map.InitializeAsync(_settings.MapTileUrl, _settings.MapAttribution, ThemeService.IsDark(_settings.Theme)); }
        catch (Exception ex) { AppLogger.Error("Map initialization failed.", ex); ThemedMessageBox.Show(ex.Message, "Map unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        if (_settings.CheckForUpdatesAutomatically)
            await _updates.CheckAndPromptAsync(this, manual: false);
    }

    private async void MapLocationChanged(object? sender, MapLocationChangedEventArgs e) =>
        await SetLocationFromMapAsync(e.Latitude, e.Longitude);

    private void SessionPropertyChanged(object? sender, PropertyChangedEventArgs e) => QueueSessionRefresh();

    private void QueueSessionRefresh()
    {
        if (IsDisposed) return;
        if (InvokeRequired)
        {
            BeginInvoke(QueueSessionRefresh);
            return;
        }
        _sessionRefreshPending = true;
        if (_isBusy || _sessionRefreshQueued || !IsHandleCreated) return;
        _sessionRefreshQueued = true;
        BeginInvoke(ProcessSessionRefresh);
    }

    private void ProcessSessionRefresh()
    {
        _sessionRefreshQueued = false;
        if (IsDisposed || _isBusy || !_sessionRefreshPending) return;
        _sessionRefreshPending = false;
        UpdateSessionCaption();
        RefreshFilter();
    }

    private void BeginBoundItemsUpdate()
    {
        if (_boundItemsUpdateDepth++ > 0) return;
        _view.RaiseListChangedEvents = false;
        _session.Media.RaiseListChangedEvents = false;
        _informationView.BeginUpdate();
    }

    private void EndBoundItemsUpdate(bool refreshInformation = false)
    {
        _refreshInformationAfterBoundUpdate |= refreshInformation;
        if (_boundItemsUpdateDepth == 0 || --_boundItemsUpdateDepth > 0) return;

        _session.Media.RaiseListChangedEvents = true;
        _view.RaiseListChangedEvents = true;
        _informationView.EndUpdate();
        if (_refreshInformationAfterBoundUpdate) _informationRequest = null;
        _refreshInformationAfterBoundUpdate = false;
        _sessionRefreshPending = true;
        QueueSessionRefresh();
    }

    private void RunPreparedEdit(Action action)
    {
        BeginBoundItemsUpdate();
        try { action(); }
        finally { EndBoundItemsUpdate(); }
    }


    private void PrepareDateForSelection(object? sender, EventArgs e)
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        RunPreparedEdit(() => _sessionController.StageDate(selected, dateTimePicker1.Value));
        operationStatus.Text = $"Date préparée pour {selected.Count} fichier(s).";
    }

    private void PrepareDateForAll(object? sender, EventArgs e)
    {
        var all = _session.Media.ToList();
        if (all.Count == 0 || !ConfirmBulkAction($"Appliquer cette date à l’ensemble des {all.Count} média(s) de la session ?")) return;
        RunPreparedEdit(() => _sessionController.StageDate(all, dateTimePicker1.Value));
        operationStatus.Text = $"Date préparée pour {all.Count} fichier(s).";
    }

    private bool ConfirmBulkAction(string message) =>
        !_settings.ConfirmBulkPrepare ||
        ThemedMessageBox.Show(message, "ExifTweaker", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    private void PrepareGpsForSelection(object? sender, EventArgs e)
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        PrepareGpsFor(selected);
    }

    private void PrepareGpsForAll(object? sender, EventArgs e)
    {
        var all = _session.Media.ToList();
        if (all.Count == 0 || !ConfirmBulkAction($"Appliquer cette localisation à l’ensemble des {all.Count} média(s) de la session ?")) return;
        PrepareGpsFor(all);
    }

    private void PrepareGpsFor(IReadOnlyList<PhotoItem> targets)
    {
        if (!IsGpsInputValid || _currentLocation is not { } location) return;
        RunPreparedEdit(() =>
        {
            _sessionController.SetLocation(targets, location.Latitude, location.Longitude, location.Altitude, _locations);
            if (!string.IsNullOrWhiteSpace(_currentLocationName))
                foreach (var item in targets) item.SetResolvedLocation(location.Latitude, location.Longitude, _currentLocationName);
        });
        if (string.IsNullOrWhiteSpace(_currentLocationName)) QueueLocationResolution(targets);
        RefreshMapMarkers();
        operationStatus.Text = $"Localisation préparée pour {targets.Count} fichier(s).";
    }

    private bool IsGpsInputValid => _currentLocation is not null &&
        !string.IsNullOrWhiteSpace(_validatedGpsText) &&
        tGPS.Text.Trim().Equals(_validatedGpsText, StringComparison.Ordinal);

    private async void openFiles_Click(object? sender, EventArgs e)
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
                ThemedMessageBox.Show(details, "Some paths could not be imported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
        catch (Exception ex) { AppLogger.Error("Import failed.", ex); ThemedMessageBox.Show(ex.Message, "Import error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { SetBusy(false); }
    }

    private async Task<bool> ApplyPendingChangesAsync(IReadOnlyList<PhotoItem> photos)
    {
        var preview = _metadata.Preview(photos);
        if (preview.FileCount == 0) return true;
        using (var previewDialog = new ApplyPreviewForm(preview))
        {
            ThemeService.Apply(previewDialog);
            if (previewDialog.ShowDialog(this) != DialogResult.OK || !previewDialog.Confirmed) return false;
        }

        var refreshInformation = false;
        var appliedAll = false;
        BeginBoundItemsUpdate();
        try
        {
            SetBusy(true);
            var ct = StartOperation();
            var progress = new Progress<int>(value => pgb.Value = value);
            var result = await _metadata.ApplyPendingChangesAsync(photos, progress, ct);
            var succeeded = photos.Where(photo => result.Files.Any(file => file.Succeeded && file.FilePath.Equals(photo.FilePath, StringComparison.OrdinalIgnoreCase))).ToList();
            appliedAll = succeeded.Count == preview.FileCount;
            refreshInformation = succeeded.Any(item => ReferenceEquals(item, _activeItem));
            _history.Forget(succeeded);
            foreach (var item in succeeded)
            {
                _thumbnails.Invalidate(item.FilePath);
                _informationCache.TryRemove(item.FilePath, out _);
            }
            _session.NotifyChanged();
            QueueLocationResolution(succeeded);
            using var report = new ApplyReportForm(result);
            ThemeService.Apply(report);
            report.ShowDialog(this);
        }
        catch (OperationCanceledException) { AppLogger.Info("Apply cancelled."); }
        catch (Exception ex)
        {
            AppLogger.Error("Apply failed.", ex);
            ThemedMessageBox.Show(ex.Message, "Apply error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            EndBoundItemsUpdate(refreshInformation);
            SetBusy(false);
        }
        return appliedAll;
    }

    private async void findGps_Click(object? sender, EventArgs e)
    {
        await SearchGpsSuggestionsAsync(showNoResultMessage: true);
    }

    private void ScheduleGpsSearch()
    {
        if (_updatingGpsSuggestions) return;
        _currentLocation = null;
        _currentLocationName = string.Empty;
        _validatedGpsText = string.Empty;
        _currentLocationLookupCts?.Cancel();
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
                ThemedMessageBox.Show("Saisissez au moins deux caractères pour rechercher un lieu.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                ThemedMessageBox.Show("Aucun lieu trouvé.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (OperationCanceledException) { AppLogger.Info("Geocoding cancelled."); }
        catch (Exception ex)
        {
            AppLogger.Error("Geocoding failed.", ex);
            operationStatus.Text = "Échec de la recherche du lieu.";
            if (showNoResultMessage)
                ThemedMessageBox.Show(ex.Message, "Geocoding error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        _validatedGpsText = tGPS.Text.Trim();
        UpdateCommandState();
    }

    private static string FormatGpsSearchText(GpsCoordinate location) =>
        $"{location.Latitude:F6}, {location.Longitude:F6}";

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
        RemoveSelectedFromSession();
        e.Handled = true;
    }

    private void dgv_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;
        if (e.RowIndex == -1 && e.ColumnIndex >= 0)
        {
            _headerColumn = dgv.Columns[e.ColumnIndex].DataPropertyName;
            dgv.ContextMenuStrip = headerContextMenu;
            return;
        }
        dgv.ContextMenuStrip = gridContextMenu;
        if (e.RowIndex < 0) return;
        var row = dgv.Rows[e.RowIndex];
        if (row.Selected) return;
        dgv.ClearSelection();
        dgv.CurrentCell = row.Cells[Math.Clamp(e.ColumnIndex, 0, dgv.Columns.Count - 1)];
        row.Selected = true;
    }

    private void headerContextMenu_Opening(object? sender, CancelEventArgs e)
    {
        if (_isBusy)
        {
            e.Cancel = true;
            return;
        }

        var column = _headerColumn ?? string.Empty;
        var kind = string.IsNullOrEmpty(column) ? ColumnFilterKind.None : ColumnFilterService.KindOf(column);
        var header = dgv.Columns.Cast<DataGridViewColumn>()
            .FirstOrDefault(candidate => candidate.DataPropertyName == column)?.HeaderText ?? column;
        header = header.TrimEnd(' ', '\u25BC');

        hdrFilter.Visible = kind != ColumnFilterKind.None;
        hdrGranularity.Visible = kind == ColumnFilterKind.Date;
        hdrClearColumnFilter.Visible = kind != ColumnFilterKind.None;
        hdrClearColumnFilter.Enabled = _columnFilters.IsFiltered(column);
        hdrClearColumnFilter.Text = $"Effacer le filtre de « {header} »";
        hdrClearAllFilters.Enabled = _columnFilters.HasFilters;
        hdrClearAllFilters.Text = $"Effacer tous les filtres de colonne ({_columnFilters.ActiveColumnCount})";
        hdrSeparator1.Visible = kind != ColumnFilterKind.None;

        if (kind == ColumnFilterKind.Date)
        {
            var granularity = _columnFilters.GetGranularity(column);
            hdrGranularityYear.Checked = granularity == DateFilterGranularity.Year;
            hdrGranularityMonth.Checked = granularity == DateFilterGranularity.Month;
            hdrGranularityDay.Checked = granularity == DateFilterGranularity.Day;
        }

        if (kind != ColumnFilterKind.None) BuildColumnFilterMenu(column, kind, header);

        var sortable = dgv.Columns.Cast<DataGridViewColumn>()
            .FirstOrDefault(candidate => candidate.DataPropertyName == column);
        hdrSortAscending.Enabled = hdrSortDescending.Enabled = sortable is { SortMode: not DataGridViewColumnSortMode.NotSortable };
    }

    private void BuildColumnFilterMenu(string column, ColumnFilterKind kind, string header)
    {
        hdrFilter.Text = kind == ColumnFilterKind.City ? "Filtrer par ville" : $"Filtrer « {header} »";
        hdrFilter.DropDownItems.Clear();

        var granularity = _columnFilters.GetGranularity(column);
        // Cascade : seules les valeurs des médias satisfaisant les autres filtres sont proposées.
        var visible = _session.Media.Where(_activeFilter).Where(item => _columnFilters.Matches(item, column)).ToList();
        var counts = visible
            .GroupBy(item => ColumnFilterService.KeyFor(item, column, granularity), StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.Ordinal);
        var keys = counts.Keys
            .OrderBy(key => key == ColumnFilterService.EmptyKey ? 1 : 0)
            .ThenBy(key => key, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        if (keys.Count == 0)
        {
            hdrFilter.DropDownItems.Add(new ToolStripMenuItem("Aucune valeur") { Enabled = false });
            return;
        }

        var selectAll = new ToolStripMenuItem("(Tout sélectionner)") { Checked = !_columnFilters.IsFiltered(column) };
        selectAll.Click += (_, _) =>
        {
            _columnFilters.Clear(column);
            RefreshFilter();
        };
        hdrFilter.DropDownItems.Add(selectAll);
        hdrFilter.DropDownItems.Add(new ToolStripSeparator());

        foreach (var key in keys)
        {
            var current = key;
            var label = ColumnFilterService.LabelFor(current, column, granularity);
            var item = new ToolStripMenuItem($"{label} ({counts[current]})") { Checked = _columnFilters.IsSelected(column, current) };
            item.Click += (_, _) =>
            {
                _columnFilters.Toggle(column, current, keys);
                RefreshFilter();
            };
            hdrFilter.DropDownItems.Add(item);
        }
    }

    private void hdrClearColumnFilter_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_headerColumn)) return;
        _columnFilters.Clear(_headerColumn);
        RefreshFilter();
    }

    private void clearColumnFilters_Click(object? sender, EventArgs e)
    {
        _columnFilters.ClearAll();
        RefreshFilter();
    }

    private void hdrGranularityYear_Click(object? sender, EventArgs e) => SetDateGranularity(DateFilterGranularity.Year);

    private void hdrGranularityMonth_Click(object? sender, EventArgs e) => SetDateGranularity(DateFilterGranularity.Month);

    private void hdrGranularityDay_Click(object? sender, EventArgs e) => SetDateGranularity(DateFilterGranularity.Day);

    private void SetDateGranularity(DateFilterGranularity granularity)
    {
        if (string.IsNullOrEmpty(_headerColumn)) return;
        _columnFilters.SetGranularity(_headerColumn, granularity);
        RefreshFilter();
    }

    private void hdrSortAscending_Click(object? sender, EventArgs e) => SortHeaderColumn(ListSortDirection.Ascending);

    private void hdrSortDescending_Click(object? sender, EventArgs e) => SortHeaderColumn(ListSortDirection.Descending);

    private void SortHeaderColumn(ListSortDirection direction)
    {
        var column = dgv.Columns.Cast<DataGridViewColumn>()
            .FirstOrDefault(candidate => candidate.DataPropertyName == _headerColumn);
        if (column is null || column.SortMode == DataGridViewColumnSortMode.NotSortable) return;
        try { dgv.Sort(column, direction); }
        catch (InvalidOperationException ex) { AppLogger.Error($"Unable to sort {column.Name}.", ex); }
    }

    private void hdrAutoSize_Click(object? sender, EventArgs e) =>
        dgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);

    private void columnsMenu_DropDownOpening(object? sender, EventArgs e)
    {
        if (sender is not ToolStripMenuItem menu) return;
        menu.DropDownItems.Clear();
        foreach (DataGridViewColumn column in dgv.Columns)
        {
            var current = column;
            var text = string.IsNullOrWhiteSpace(current.HeaderText) ? current.Name : current.HeaderText.TrimEnd(' ', '\u25BC');
            var item = new ToolStripMenuItem(text) { Checked = current.Visible, CheckOnClick = true };
            item.Click += (_, _) =>
            {
                if (!item.Checked && dgv.Columns.Cast<DataGridViewColumn>().Count(candidate => candidate.Visible) <= 1)
                {
                    item.Checked = true;
                    return;
                }
                current.Visible = item.Checked;
                SaveColumnVisibility();
            };
            menu.DropDownItems.Add(item);
        }
    }

    private void dgv_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e) => ApplyColumnVisibility();

    private void ApplyColumnVisibility()
    {
        if (_settings.HiddenColumns.Count == 0) return;
        var hidden = _settings.HiddenColumns.ToHashSet(StringComparer.Ordinal);
        foreach (DataGridViewColumn column in dgv.Columns)
        {
            var key = ColumnKey(column);
            var visible = !hidden.Contains(key);
            if (column.Visible != visible) column.Visible = visible;
        }
        if (dgv.Columns.Cast<DataGridViewColumn>().All(column => !column.Visible) && dgv.Columns.Count > 0)
            dgv.Columns[0].Visible = true;
    }

    private void SaveColumnVisibility()
    {
        _settings.HiddenColumns = dgv.Columns.Cast<DataGridViewColumn>()
            .Where(column => !column.Visible)
            .Select(ColumnKey)
            .ToList();
        try { _settings.Save(); }
        catch (Exception ex) { AppLogger.Error("Unable to persist column visibility.", ex); }
    }

    private static string ColumnKey(DataGridViewColumn column) =>
        string.IsNullOrEmpty(column.DataPropertyName) ? column.Name : column.DataPropertyName;

    private void UpdateColumnFilterIndicators()
    {
        foreach (DataGridViewColumn column in dgv.Columns)
        {
            var filtered = !string.IsNullOrEmpty(column.DataPropertyName) && _columnFilters.IsFiltered(column.DataPropertyName);
            var baseText = column.HeaderText.TrimEnd(' ', '\u25BC');
            var desired = filtered ? $"{baseText} \u25BC" : baseText;
            if (column.HeaderText != desired) column.HeaderText = desired;
        }
        clearColumnFiltersMenuItem.Enabled = _columnFilters.HasFilters;
        clearColumnFiltersMenuItem.Text = $"Effacer les filtres de colonne ({_columnFilters.ActiveColumnCount})";
    }

    private void gridContextMenu_Opening(object? sender, CancelEventArgs e)
    {
        var selected = SelectedItems;
        if (_isBusy || selected.Count == 0)
        {
            e.Cancel = true;
            return;
        }

        var count = selected.Count;
        var pending = selected.Count(item => item.HasPendingChanges);
        var sharedGps = SharedLocation(selected);
        var sharedDate = SharedDate(selected);

        ctxCopyGps.Enabled = sharedGps is not null;
        ctxCopyGps.Text = sharedGps is { } gps ? $"GPS : {FormatGpsSearchText(gps)}" : "GPS (valeurs différentes)";
        ctxCopyDate.Enabled = sharedDate is not null;
        ctxCopyDate.Text = sharedDate is { } date ? $"Date : {FormatDateClipboard(date)}" : "Date (valeurs différentes)";
        ctxCopyBoth.Enabled = sharedGps is not null && sharedDate is not null;

        ctxPasteGps.Enabled = _gpsClipboard is not null;
        ctxPasteGps.Text = _gpsClipboard is { } clipboardGps ? $"GPS : {FormatGpsSearchText(clipboardGps)}" : "GPS (presse-papiers vide)";
        ctxPasteDate.Enabled = _dateClipboard is not null;
        ctxPasteDate.Text = _dateClipboard is { } clipboardDate ? $"Date : {FormatDateClipboard(clipboardDate)}" : "Date (presse-papiers vide)";
        ctxPasteBoth.Enabled = _gpsClipboard is not null && _dateClipboard is not null;

        ctxPrepareGps.Enabled = IsGpsInputValid;
        ctxPrepareGps.Text = IsGpsInputValid && _currentLocation is { } current
            ? $"GPS : {(string.IsNullOrWhiteSpace(_currentLocationName) ? FormatGpsSearchText(current) : _currentLocationName)}"
            : "GPS (aucun lieu validé)";
        ctxPrepareDate.Text = $"Date : {dateTimePicker1.Value:yyyy-MM-dd}";
        ctxPrepareBoth.Enabled = IsGpsInputValid;

        ctxRemoveGps.Enabled = selected.Any(item => item.EffectiveLatitude.HasValue || item.EffectiveLongitude.HasValue);
        ctxResetSelection.Enabled = pending > 0;
        ctxResetSelection.Text = $"Restaurer la sélection ({pending})";
        ctxApply.Enabled = pending > 0;
        ctxApply.Text = $"Vérifier et appliquer la sélection ({pending})";
        ctxImmich.Enabled = _settings.ImmichEnabled;
        ctxImmich.Text = $"Envoyer sur Immich ({count})";
        ctxRemove.Text = $"Retirer de la session ({count})";
        ctxOpenLocation.Enabled = count == 1;
        ctxShowOnMap.Enabled = selected.Any(item => item.EffectiveLatitude.HasValue && item.EffectiveLongitude.HasValue);
        ctxShowInformation.Enabled = count == 1;
    }

    private static string FormatDateClipboard((DateTime Date, TimeSpan? Offset) value) =>
        value.Offset is { } offset
            ? $"{value.Date:yyyy-MM-dd HH:mm:ss} ({(offset < TimeSpan.Zero ? "-" : "+")}{Math.Abs((int)offset.TotalHours):00}:{Math.Abs(offset.Minutes):00})"
            : $"{value.Date:yyyy-MM-dd HH:mm:ss}";

    private static GpsCoordinate? SharedLocation(IReadOnlyList<PhotoItem> items)
    {
        var first = items[0];
        if (first.EffectiveLatitude is not double latitude || first.EffectiveLongitude is not double longitude) return null;
        return items.All(item => CoordinatesMatch(item, latitude, longitude))
            ? new GpsCoordinate(latitude, longitude, first.EffectiveAltitude)
            : null;
    }

    private static (DateTime Date, TimeSpan? Offset)? SharedDate(IReadOnlyList<PhotoItem> items)
    {
        var first = items[0];
        if (first.EffectiveCaptureDate is not DateTime date) return null;
        var offset = first.EffectiveOffset;
        return items.All(item => item.EffectiveCaptureDate == date && item.EffectiveOffset == offset)
            ? (date, offset)
            : null;
    }

    private void ctxCopyGps_Click(object? sender, EventArgs e) => CopyGpsSelected();

    private void ctxCopyDate_Click(object? sender, EventArgs e) => CopyDateSelected();

    private void ctxCopyBoth_Click(object? sender, EventArgs e)
    {
        CopyGpsSelected();
        CopyDateSelected();
        operationStatus.Text = "GPS et date copiés.";
    }

    private void CopyDateSelected()
    {
        var selected = SelectedItems;
        if (selected.Count == 0 || SharedDate(selected) is not { } shared) return;
        _dateClipboard = shared;
        operationStatus.Text = "Date copiée.";
        UpdateCommandState();
    }

    private void ctxPasteGps_Click(object? sender, EventArgs e) => PasteGpsSelected();

    private void ctxPasteDate_Click(object? sender, EventArgs e) => PasteDateSelected();

    private void ctxPasteBoth_Click(object? sender, EventArgs e)
    {
        if (_gpsClipboard is not { } gps || _dateClipboard is null) return;
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        RunPreparedEdit(() =>
        {
            using (_history.BeginBatch())
            {
                _sessionController.SetLocation(selected, gps.Latitude, gps.Longitude, gps.Altitude, _locations);
                ApplyDateClipboard(selected);
            }
        });
        QueueLocationResolution(selected);
        RefreshMapMarkers();
        operationStatus.Text = $"GPS et date collés sur {selected.Count} fichier(s).";
    }

    private void PasteDateSelected()
    {
        var selected = SelectedItems;
        if (_dateClipboard is null || selected.Count == 0) return;
        RunPreparedEdit(() => ApplyDateClipboard(selected));
        operationStatus.Text = $"Date collée sur {selected.Count} fichier(s).";
    }

    private void ApplyDateClipboard(IReadOnlyList<PhotoItem> targets)
    {
        if (_dateClipboard is not { } clipboard) return;
        _sessionController.EditDate(targets, new DateEditRequest
        {
            Mode = DateEditMode.Set,
            Date = clipboard.Date,
            ChangeTimezone = true,
            TimezoneOffset = clipboard.Offset,
            RemoveTimezone = clipboard.Offset is null,
            TimezoneMode = TimezoneChangeMode.KeepLocalTime
        });
    }

    private void ctxPrepareGps_Click(object? sender, EventArgs e) => PrepareGpsForSelection(sender, e);

    private void ctxPrepareDate_Click(object? sender, EventArgs e) => PrepareDateForSelection(sender, e);

    private void ctxPrepareBoth_Click(object? sender, EventArgs e)
    {
        var selected = SelectedItems;
        if (selected.Count == 0 || !IsGpsInputValid || _currentLocation is not { } location) return;
        RunPreparedEdit(() =>
        {
            using (_history.BeginBatch())
            {
                _sessionController.SetLocation(selected, location.Latitude, location.Longitude, location.Altitude, _locations);
                _sessionController.StageDate(selected, dateTimePicker1.Value);
            }
            if (!string.IsNullOrWhiteSpace(_currentLocationName))
                foreach (var item in selected) item.SetResolvedLocation(location.Latitude, location.Longitude, _currentLocationName);
        });
        if (string.IsNullOrWhiteSpace(_currentLocationName)) QueueLocationResolution(selected);
        RefreshMapMarkers();
        operationStatus.Text = $"GPS et date préparés pour {selected.Count} fichier(s).";
    }

    private async void ctxImmich_DropDownOpening(object? sender, EventArgs e)
    {
        if (!_immichAlbumsLoaded && !_isBusy) await LoadImmichAlbumsAsync();
        BuildImmichContextMenu();
    }

    private void BuildImmichContextMenu()
    {
        ctxImmich.DropDownItems.Clear();
        var noAlbum = new ToolStripMenuItem(NoAlbumEntry);
        noAlbum.Click += async (_, _) => await SendToImmichAsync(SelectedItems);
        ctxImmich.DropDownItems.Add(noAlbum);
        foreach (var album in immichAlbum.Items.OfType<ImmichAlbum>())
        {
            var current = album;
            var item = new ToolStripMenuItem(current.Name);
            item.Click += (_, _) =>
            {
                immichAlbum.SelectedItem = current;
                _ = SendToImmichAsync(SelectedItems);
            };
            ctxImmich.DropDownItems.Add(item);
        }
        var newAlbum = new ToolStripMenuItem(NewAlbumEntry);
        newAlbum.Click += (_, _) =>
        {
            immichAlbum.SelectedItem = NewAlbumEntry;
            _ = SendToImmichAsync(SelectedItems);
        };
        ctxImmich.DropDownItems.Add(new ToolStripSeparator());
        ctxImmich.DropDownItems.Add(newAlbum);
    }

    private void ctxShowOnMap_Click(object? sender, EventArgs e)
    {
        if (!_map.Visible) ToggleMap();
        else RefreshMapMarkers();
    }

    private void ctxOpenLocation_Click(object? sender, EventArgs e)
    {
        var active = SelectedItems.FirstOrDefault();
        if (active is null) return;
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("explorer.exe", $"/select,\"{active.FilePath}\"") { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Unable to reveal {active.FilePath}.", ex);
            ThemedMessageBox.Show(ex.Message, "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ctxCopyPath_Click(object? sender, EventArgs e) => CopySelectedPaths();

    private void CopySelectedPaths()
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        try
        {
            Clipboard.SetText(string.Join(Environment.NewLine, selected.Select(item => item.FilePath)));
            operationStatus.Text = $"{selected.Count} chemin(s) copié(s).";
        }
        catch (Exception ex)
        {
            AppLogger.Error("Unable to copy file paths.", ex);
            operationStatus.Text = "Impossible de copier les chemins.";
        }
    }

    private async void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem is not PhotoItem item) return;
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

    private void dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.RowIndex >= _bindingSource.Count) return;
        if (dgv.Columns[e.ColumnIndex].DataPropertyName == nameof(PhotoItem.Details))
        {
            e.CellStyle.WrapMode = DataGridViewTriState.True;
            return;
        }
        if (e.ColumnIndex != thumbnailColumn.Index) return;
        PhotoItem? item;
        try { item = _bindingSource[e.RowIndex] as PhotoItem; }
        catch (ArgumentOutOfRangeException) { return; }
        if (item is null) return;
        if (_gridThumbnails.TryGetValue(item.FilePath, out var cached))
        {
            e.Value = cached;
            e.FormattingApplied = true;
            return;
        }
        if (!_thumbnailLoads.TryAdd(item.FilePath, 0)) return;
        _ = LoadGridThumbnailAsync(item);
    }

    private async Task LoadGridThumbnailAsync(PhotoItem item)
    {
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
        catch (Exception ex)
        {
            AppLogger.Error($"Thumbnail loading failed for {item.FilePath}.", ex);
        }
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
        if (dgv.Focused && SelectedItems.Count > 0)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.C)) { CopySelectedPaths(); return true; }
            if (keyData == (Keys.Control | Keys.C)) { ctxCopyBoth_Click(this, EventArgs.Empty); return true; }
            if (keyData == (Keys.Control | Keys.V)) { ctxPasteBoth_Click(this, EventArgs.Empty); return true; }
        }
        return base.ProcessCmdKey(ref message, keyData);
    }

    private void WireCommands()
    {
        _gpsSearchTimer.Tick += GpsSearchTimerTick;
    }

    private async void GpsSearchTimerTick(object? sender, EventArgs e)
    {
        _gpsSearchTimer.Stop();
        await SearchGpsSuggestionsAsync(showNoResultMessage: false);
    }

    private void tGPS_TextChanged(object sender, EventArgs e) => ScheduleGpsSearch();

    private async void applyAllButton_Click(object sender, EventArgs e) => await ApplyPendingChangesAsync(_session.Media.ToList());

    private async void applySelectedMenuItem_Click(object sender, EventArgs e) => await ApplyPendingChangesAsync(SelectedItems);

    private async void uploadImmichSelected_Click(object sender, EventArgs e) => await SendToImmichAsync(SelectedItems);

    private async void uploadImmichAll_Click(object sender, EventArgs e) => await SendToImmichAsync(_session.Media.ToList());

    private async void openFolderCommand_Click(object sender, EventArgs e) => await OpenFolderAsync();

    private void dateEditorCommand_Click(object sender, EventArgs e) => OpenDateEditor();

    private async void settingsCommand_Click(object sender, EventArgs e) => await OpenSettingsAsync();

    private void cancelCommand_Click(object sender, EventArgs e) => _operationCts?.Cancel();

    private void undoCommand_Click(object sender, EventArgs e) => UndoPendingChanges();

    private void redoCommand_Click(object sender, EventArgs e) => RedoPendingChanges();

    private void resetSelectedCommand_Click(object sender, EventArgs e) => ResetPatches(SelectedItems);

    private void resetAllCommand_Click(object sender, EventArgs e) => ResetPatches(_session.Media);

    private void minusHourCommand_Click(object sender, EventArgs e) => ShiftSelected(TimeSpan.FromHours(-1));

    private void plusHourCommand_Click(object sender, EventArgs e) => ShiftSelected(TimeSpan.FromHours(1));

    private void minusMinuteCommand_Click(object sender, EventArgs e) => ShiftSelected(TimeSpan.FromMinutes(-1));

    private void plusMinuteCommand_Click(object sender, EventArgs e) => ShiftSelected(TimeSpan.FromMinutes(1));

    private void removeGpsCommand_Click(object sender, EventArgs e) => RemoveGpsSelected();

    private void copyGpsCommand_Click(object sender, EventArgs e) => CopyGpsSelected();

    private void pasteGpsCommand_Click(object sender, EventArgs e) => PasteGpsSelected();

    private async void reverseGpsCommand_Click(object sender, EventArgs e) => await ReverseGpsSelectedAsync();

    private void mapCommand_Click(object sender, EventArgs e) => ToggleMap();

    private void previewMenuItem_Click(object sender, EventArgs e) => ShowPreview();

    private async void informationMenuItem_Click(object sender, EventArgs e) => await ShowInformationAsync();

    private void quickActions_Click(object sender, EventArgs e) => ToggleQuickActions();

    private void allFilterCommand_Click(object sender, EventArgs e) => ApplyFilter("Tous", _ => true);

    private void modifiedFilterCommand_Click(object sender, EventArgs e) => ApplyFilter("Modifiés", item => item.HasPendingChanges);

    private void noGpsFilterCommand_Click(object sender, EventArgs e) => ApplyFilter("Sans GPS", item => !item.EffectiveLatitude.HasValue || !item.EffectiveLongitude.HasValue);

    private void noDateFilterCommand_Click(object sender, EventArgs e) => ApplyFilter("Sans date", item => !item.EffectiveCaptureDate.HasValue);

    private void errorsFilterCommand_Click(object sender, EventArgs e) => ApplyFilter("Erreurs", item => item.Error is not null);

    private async void restoreBackupCommand_Click(object sender, EventArgs e) => await RestoreSelectedAsync();

    private void selectAllMenuItem_Click(object sender, EventArgs e) => dgv.SelectAll();

    private void removeFromSessionMenuItem_Click(object sender, EventArgs e) => RemoveSelectedFromSession();

    private void exitMenuItem_Click(object sender, EventArgs e) => Close();

    private void guideMenuItem_Click(object sender, EventArgs e) =>
        OpenExternal("https://github.com/fatvicbart/exif-tweaker/blob/main/GUIDE_UTILISATEUR.md");

    private void logsMenuItem_Click(object sender, EventArgs e) => ShowLogs();

    private async void verifyExifToolMenuItem_Click(object sender, EventArgs e) => await VerifyExifToolAsync();

    private async void checkUpdatesMenuItem_Click(object sender, EventArgs e) => await CheckForUpdatesAsync();

    private void aboutMenuItem_Click(object sender, EventArgs e) => ShowAbout();

    private async void dgv_SelectionChanged(object sender, EventArgs e)
    {
        if (!_restoringGridSelection) await ActiveSelectionChangedAsync();
    }

    private async Task ActiveSelectionChangedAsync()
    {
        UpdateCommandState();
        _activeItem = dgv.CurrentRow?.DataBoundItem as PhotoItem;
        if (_activeItem is null)
        {
            _informationCts?.Cancel();
            _informationView.ShowEmpty();
            _informationRequest = null;
            return;
        }
        if (_informationView.Visible && !_isBusy && _boundItemsUpdateDepth == 0) await LoadInformationAsync(_activeItem);
    }

    private void ToggleQuickActions()
    {
        commands.Visible = !commands.Visible;
        quickActionsMenuItem.Checked = commands.Visible;
        quickActionsToggleItem.Checked = commands.Visible;
        quickActionsToggleItem.Text = commands.Visible ? "Actions rapides : affichées" : "Actions rapides : masquées";
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
        var desired = _session.Media.Where(item => _activeFilter(item) && _columnFilters.Matches(item)).ToList();
        var selectedBefore = SelectedItems.ToHashSet();
        var currentBefore = dgv.CurrentRow?.DataBoundItem as PhotoItem;
        var currentColumn = dgv.CurrentCell?.ColumnIndex ?? 0;
        _restoringGridSelection = true;
        _view.RaiseListChangedEvents = false;
        try
        {
            for (var index = _view.Count - 1; index >= 0; index--)
                if (!desired.Contains(_view[index])) _view.RemoveAt(index);
            for (var index = 0; index < desired.Count; index++)
            {
                if (index < _view.Count && ReferenceEquals(_view[index], desired[index])) continue;
                var existing = _view.IndexOf(desired[index]);
                if (existing >= 0) _view.RemoveAt(existing);
                _view.Insert(Math.Min(index, _view.Count), desired[index]);
            }
        }
        finally { _view.RaiseListChangedEvents = true; }
        _bindingSource.ResetBindings(false);
        var currentRow = dgv.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => ReferenceEquals(row.DataBoundItem, currentBefore))
            ?? dgv.Rows.Cast<DataGridViewRow>().FirstOrDefault(row => row.DataBoundItem is PhotoItem item && selectedBefore.Contains(item))
            ?? dgv.Rows.Cast<DataGridViewRow>().FirstOrDefault();
        if (currentRow is not null)
            dgv.CurrentCell = currentRow.Cells[Math.Clamp(currentColumn, 0, dgv.Columns.Count - 1)];
        dgv.ClearSelection();
        foreach (DataGridViewRow row in dgv.Rows)
            if (row.DataBoundItem is PhotoItem item && selectedBefore.Contains(item)) row.Selected = true;
        if (dgv.SelectedRows.Count == 0 && currentRow is not null) currentRow.Selected = true;
        _restoringGridSelection = false;
        UpdateColumnFilterIndicators();
        if (filterQuickCommand is not null)
        {
            var columnSuffix = _columnFilters.HasFilters ? $" + {_columnFilters.ActiveColumnCount} colonne(s)" : string.Empty;
            filterQuickCommand.Text = $"Filtre : {_activeFilterName}{columnSuffix} ({_view.Count}/{_session.Media.Count})";
        }
        UpdateFilterChecks();
        UpdateCommandState();
        var currentItem = dgv.CurrentRow?.DataBoundItem as PhotoItem;
        if (!ReferenceEquals(currentItem, _activeItem)) _ = ActiveSelectionChangedAsync();
        else if (_informationView.Visible && !_isBusy && _boundItemsUpdateDepth == 0 && currentItem is not null) _ = LoadInformationAsync(currentItem);
    }

    private void OpenDateEditor()
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        var dates = selected.Select(item => item.EffectiveCaptureDate).Distinct().ToList();
        var offsets = selected.Select(item => item.EffectiveOffset).Distinct().ToList();
        using var dialog = new DateEditorForm(dates.Count == 1 ? dates[0] : null, offsets.Count == 1 ? offsets[0] : null, dates.Count > 1 || offsets.Count > 1);
        ThemeService.Apply(dialog);
        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.Request is not null)
            RunPreparedEdit(() => _sessionController.EditDate(selected, dialog.Request));
    }

    private async Task OpenSettingsAsync()
    {
        using var dialog = new SettingsForm(_settings);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        ThemeService.Apply(this);
        ThemeService.Apply(_gpsSuggestionsPopup);
        try { await _map.InitializeAsync(_settings.MapTileUrl, _settings.MapAttribution, ThemeService.IsDark(_settings.Theme)); }
        catch (Exception ex) { AppLogger.Error("Map reconfiguration failed.", ex); }
        ThemedMessageBox.Show("Paramètres enregistrés.\n\nRedémarrez l’application uniquement si le chemin d’ExifTool a été modifié.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async Task SendToImmichAsync(IReadOnlyList<PhotoItem> photos)
    {
        if (photos.Count == 0) return;
        var secrets = new WindowsSecretStore();
        var key = secrets.Read("immich-api-key");
        if (!_settings.ImmichEnabled || string.IsNullOrWhiteSpace(_settings.ImmichServerUrl) || string.IsNullOrWhiteSpace(key))
        {
            var configure = ThemedMessageBox.Show(
                "L’intégration Immich n’est pas encore configurée. Ouvrir sa configuration maintenant ?",
                "Immich", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (configure != DialogResult.Yes) return;
            using (var settingsDialog = new ImmichSettingsForm(_settings, secrets))
            {
                ThemeService.Apply(settingsDialog);
                if (settingsDialog.ShowDialog(this) != DialogResult.OK) return;
            }
            key = secrets.Read("immich-api-key");
            if (!_settings.ImmichEnabled || string.IsNullOrWhiteSpace(key)) return;
        }

        using var client = new ImmichClient(_settings.ImmichServerUrl, key);
        ImmichServerInfo server;
        IReadOnlyList<ImmichAlbum> albums;
        try
        {
            SetBusy(true);
            var ct = StartOperation();
            operationStatus.Text = "Connexion à Immich…";
            server = await client.GetServerInfoAsync(ct);
            operationStatus.Text = "Chargement des albums Immich…";
            albums = await client.GetAlbumsAsync(ct);
            PopulateAlbumCombo(albums);
        }
        catch (OperationCanceledException) { return; }
        catch (Exception ex)
        {
            AppLogger.Error("Unable to prepare the Immich upload.", ex);
            ThemedMessageBox.Show(
                $"{ex.Message}\n\nVérifiez l’adresse, la clé et ses permissions : server.about, asset.upload, asset.share, album.read, album.create et albumAsset.create.",
                "Connexion Immich", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        finally { SetBusy(false); }

        using var prepare = new ImmichUploadForm(photos, albums, _settings, server, SelectedAlbumId, SelectedNewAlbumName);
        ThemeService.Apply(prepare);
        if (prepare.ShowDialog(this) != DialogResult.OK || prepare.Request is null) return;

        var pending = photos.Where(photo => photo.HasPendingChanges).ToList();
        if (pending.Count > 0 && prepare.ApplyBeforeUpload && !await ApplyPendingChangesAsync(pending)) return;

        var request = prepare.Request with { FilePaths = photos.Select(photo => photo.FilePath).ToList() };
        using var progressDialog = new ImmichUploadProgressForm(new ImmichUploadService(client), request);
        ThemeService.Apply(progressDialog);
        progressDialog.ShowDialog(this);
    }

    private const string NoAlbumEntry = "Aucun album";
    private const string NewAlbumEntry = "Créer un nouvel album…";

    private string? SelectedAlbumId => immichAlbum.SelectedItem as ImmichAlbum is { } album ? album.Id : null;

    private string? SelectedNewAlbumName =>
        Equals(immichAlbum.SelectedItem, NewAlbumEntry) && !string.IsNullOrWhiteSpace(immichNewAlbum.Text)
            ? immichNewAlbum.Text.Trim()
            : null;

    private void PopulateAlbumCombo(IReadOnlyList<ImmichAlbum> albums)
    {
        var previousId = SelectedAlbumId;
        var wasNewAlbum = Equals(immichAlbum.SelectedItem, NewAlbumEntry);
        immichAlbum.BeginUpdate();
        try
        {
            immichAlbum.Items.Clear();
            immichAlbum.Items.Add(NoAlbumEntry);
            foreach (var album in albums) immichAlbum.Items.Add(album);
            immichAlbum.Items.Add(NewAlbumEntry);
            var restored = albums.FirstOrDefault(album => album.Id == previousId)
                ?? albums.FirstOrDefault(album => album.Id == _settings.ImmichDefaultAlbumId);
            if (wasNewAlbum) immichAlbum.SelectedItem = NewAlbumEntry;
            else if (restored is not null) immichAlbum.SelectedItem = restored;
            else immichAlbum.SelectedIndex = 0;
        }
        finally { immichAlbum.EndUpdate(); }
        _immichAlbumsLoaded = true;
    }

    private void immichAlbum_SelectedIndexChanged(object? sender, EventArgs e) =>
        immichNewAlbum.Enabled = Equals(immichAlbum.SelectedItem, NewAlbumEntry);

    private async void immichAlbum_DropDown(object? sender, EventArgs e)
    {
        if (_immichAlbumsLoaded || _isBusy) return;
        await LoadImmichAlbumsAsync();
    }

    private async Task LoadImmichAlbumsAsync()
    {
        var key = new WindowsSecretStore().Read("immich-api-key");
        if (!_settings.ImmichEnabled || string.IsNullOrWhiteSpace(_settings.ImmichServerUrl) || string.IsNullOrWhiteSpace(key)) return;
        try
        {
            using var client = new ImmichClient(_settings.ImmichServerUrl, key);
            PopulateAlbumCombo(await client.GetAlbumsAsync(CancellationToken.None));
        }
        catch (Exception ex)
        {
            AppLogger.Error("Unable to load Immich albums.", ex);
            operationStatus.Text = "Albums Immich indisponibles.";
        }
    }

    private void ToggleMap()
    {
        var showMap = !_map.Visible;
        _informationView.Visible = false;
        _informationCts?.Cancel();
        _informationRequest = null;
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
        _informationCts?.Cancel();
        _informationRequest = null;
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
        var lastWriteUtc = File.GetLastWriteTimeUtc(item.FilePath);
        if (_informationRequest is { } request &&
            request.FilePath.Equals(item.FilePath, StringComparison.OrdinalIgnoreCase) &&
            request.LastWriteUtc == lastWriteUtc) return;

        _informationRequest = (item.FilePath, lastWriteUtc);
        _informationCts?.Cancel();
        _informationCts?.Dispose();
        var cts = new CancellationTokenSource();
        _informationCts = cts;
        try
        {
            if (!_informationCache.TryGetValue(item.FilePath, out var cached) || cached.LastWriteUtc != lastWriteUtc)
            {
                _informationView.ShowLoading(item.FilePath);
                var tags = await _exifTool.ReadAllMetadataAsync(item.FilePath, cts.Token);
                cached = (lastWriteUtc, tags);
                _informationCache[item.FilePath] = cached;
            }
            if (cts.IsCancellationRequested || !ReferenceEquals(_activeItem, item) || !_informationView.Visible) return;
            _informationView.ShowTags(item.FilePath, cached.Tags);
        }
        catch (OperationCanceledException)
        {
            if (ReferenceEquals(_informationCts, cts)) _informationRequest = null;
        }
        catch (Exception ex)
        {
            if (ReferenceEquals(_informationCts, cts)) _informationRequest = null;
            AppLogger.Error($"Full metadata read failed for {item.FilePath}.", ex);
            if (!cts.IsCancellationRequested && ReferenceEquals(_activeItem, item))
                _informationView.ShowError(item.FilePath, "Impossible de lire les métadonnées — consultez les journaux.");
        }
    }

    private async Task SetLocationFromMapAsync(double latitude, double longitude)
    {
        var location = new GpsCoordinate(latitude, longitude);
        SetGpsSearchTextSilently(FormatGpsSearchText(location));
        SetCurrentLocation(location, null);
        operationStatus.Text = "Identification du point sélectionné…";
        await IdentifyCurrentLocationAsync(location, showNoResultMessage: false);
    }

    private void RemoveGpsSelected()
    {
        var selected = SelectedItems;
        RunPreparedEdit(() => _sessionController.RemoveLocation(selected, _locations));
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
        catch (Exception ex) { ThemedMessageBox.Show(ex.Message, "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void PasteGpsSelected()
    {
        if (_gpsClipboard is not { } gps || SelectedItems.Count == 0) return;
        var selected = SelectedItems;
        RunPreparedEdit(() => _sessionController.SetLocation(selected, gps.Latitude, gps.Longitude, gps.Altitude, _locations));
        SetGpsSearchTextSilently(FormatGpsSearchText(gps));
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
            ThemedMessageBox.Show("Choisissez un point sur la carte ou sélectionnez une photo géolocalisée.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_currentLocation != location || !IsGpsInputValid)
        {
            SetGpsSearchTextSilently(FormatGpsSearchText(location));
            SetCurrentLocation(location, null);
        }
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
                operationStatus.Text = "Aucune adresse trouvée pour ce point.";
                if (showNoResultMessage)
                    ThemedMessageBox.Show("Aucune adresse trouvée pour ces coordonnées.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetGpsSearchTextSilently(result.Name);
            SetCurrentLocation(location with { Altitude = result.Altitude ?? location.Altitude }, result.Name);
            CacheLocationAddress(location.Latitude, location.Longitude, result.Name);
            foreach (var item in SelectedItems.Where(item => CoordinatesMatch(item, location.Latitude, location.Longitude)))
                item.SetResolvedLocation(location.Latitude, location.Longitude, result.Name);
            operationStatus.Text = "Adresse identifiée. Cliquez sur « Préparer le GPS à la sélection » pour l’appliquer.";
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            AppLogger.Error("Reverse geocoding failed.", ex);
            operationStatus.Text = "Impossible d’identifier ce point.";
            if (showNoResultMessage)
                ThemedMessageBox.Show(ex.Message, "Geocoding error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
        RunPreparedEdit(() => _sessionController.ShiftDate(selected, shift));
    }

    private async Task RestoreSelectedAsync()
    {
        var selected = SelectedItems;
        if (selected.Count == 0 || ThemedMessageBox.Show("Les fichiers sélectionnés vont être restaurés depuis leurs sauvegardes ExifTool.\n\nVoulez-vous continuer ?", "Restore backup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        var refreshInformation = false;
        BeginBoundItemsUpdate();
        SetBusy(true);
        try
        {
            var result = await _metadata.RestoreBackupsAsync(selected, StartOperation());
            var succeeded = selected.Where(item => result.Files.Any(file => file.Succeeded && file.FilePath.Equals(item.FilePath, StringComparison.OrdinalIgnoreCase))).ToList();
            refreshInformation = succeeded.Any(item => ReferenceEquals(item, _activeItem));
            foreach (var item in succeeded)
            {
                _thumbnails.Invalidate(item.FilePath);
                _informationCache.TryRemove(item.FilePath, out _);
            }
            _history.Forget(succeeded);
            _session.NotifyChanged();
            QueueLocationResolution(selected);
            using var report = new ApplyReportForm(result) { Text = "Restore report" };
            ThemeService.Apply(report);
            report.ShowDialog(this);
        }
        catch (OperationCanceledException) { AppLogger.Info("Restore cancelled."); }
        catch (Exception ex)
        {
            AppLogger.Error("Restore failed.", ex);
            ThemedMessageBox.Show(ex.Message, "Restore error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            EndBoundItemsUpdate(refreshInformation);
            SetBusy(false);
        }
    }

    private void ResetPatches(IEnumerable<PhotoItem> items)
    {
        var list = items.ToList();
        RunPreparedEdit(() => _sessionController.Reset(list));
        QueueLocationResolution(list);
    }


    private void UndoPendingChanges()
    {
        var changed = false;
        RunPreparedEdit(() =>
        {
            changed = _history.Undo(_session.Media);
            if (changed) _session.NotifyChanged();
        });
        if (changed) QueueLocationResolution(_session.Media);
    }

    private void RedoPendingChanges()
    {
        var changed = false;
        RunPreparedEdit(() =>
        {
            changed = _history.Redo(_session.Media);
            if (changed) _session.NotifyChanged();
        });
        if (changed) QueueLocationResolution(_session.Media);
    }

    private void RemoveSelectedFromSession()
    {
        var selected = SelectedItems;
        _session.RemoveRange(selected);
        if (_map.Visible) RefreshMapMarkers();
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
        var selected = SelectedItems;
        var hasSelection = selected.Count > 0;
        var selectedPending = selected.Count(item => item.HasPendingChanges);
        var canEditSelection = !_isBusy && hasSelection;
        var applyText = $"Vérifier et appliquer tout ({pending})";
        applyAllButton.Text = applyText.ToUpperInvariant();
        applyMenuItem.Text = applyText;
        applySelectedMenuItem.Text = $"Vérifier et appliquer la sélection ({selectedPending})";
        resetAllCommand.Text = $"Restaurer tout ({pending})";
        resetSelectedCommand.Text = $"Restaurer la sélection ({selectedPending})";
        applyAllButton.Enabled = !_isBusy && pending > 0;
        applyMenuItem.Enabled = !_isBusy && pending > 0;
        applySelectedMenuItem.Enabled = !_isBusy && selectedPending > 0;
        undoCommand.Enabled = undoMenuItem.Enabled = !_isBusy && _history.CanUndo;
        redoCommand.Enabled = redoMenuItem.Enabled = !_isBusy && _history.CanRedo;
        cancelCommand.Enabled = cancelMenuItem.Enabled = _isBusy;

        uploadImmichSelectedMenuItem.Enabled = uploadImmichSelectedQuickItem.Enabled = canEditSelection;
        uploadImmichAllMenuItem.Enabled = uploadImmichAllQuickItem.Enabled = !_isBusy && hasMedia;
        uploadImmichSelectedMenuItem.Text = $"Envoyer la sélection vers Immich… ({selected.Count})";
        uploadImmichSelectedQuickItem.Text = $"Envoyer la sélection… ({selected.Count})";
        uploadImmichAllMenuItem.Text = $"Envoyer toutes les images vers Immich… ({_session.Media.Count})";
        uploadImmichAllQuickItem.Text = $"Envoyer toutes les images… ({_session.Media.Count})";
        immichQuickCommand.Enabled = !_isBusy && hasMedia;

        bOpen.Enabled = canEditSelection;
        bOpen.Text = $"PRÉPARER LA SÉLECTION ({selected.Count})";
        bOpenAll.Enabled = !_isBusy && hasMedia;
        bOpenAll.Text = $"PRÉPARER TOUT ({_session.Media.Count})";
        dateEditorCommand.Enabled = dateQuickCommand.Enabled = canEditSelection;
        resetSelectedCommand.Enabled = !_isBusy && selectedPending > 0;
        resetAllCommand.Enabled = !_isBusy && pending > 0;
        minusHourCommand.Enabled = plusHourCommand.Enabled = canEditSelection;
        minusMinuteCommand.Enabled = plusMinuteCommand.Enabled = canEditSelection;
        copyGpsCommand.Enabled = copyGpsQuickItem.Enabled = canEditSelection;
        pasteGpsCommand.Enabled = pasteGpsQuickItem.Enabled = canEditSelection && _gpsClipboard is not null;
        removeGpsCommand.Enabled = removeGpsQuickItem.Enabled = canEditSelection;
        restoreBackupCommand.Enabled = removeFromSessionMenuItem.Enabled = canEditSelection;
        selectAllMenuItem.Enabled = !_isBusy && hasMedia;

        var canSearchGps = !_isBusy && !_gpsSearchInProgress && tGPS.Text.Trim().Length >= 2;
        bGPS.Enabled = canEditSelection && IsGpsInputValid;
        bGPS.Text = $"PRÉPARER LA SÉLECTION ({selected.Count})";
        bGPSAll.Enabled = !_isBusy && hasMedia && IsGpsInputValid;
        bGPSAll.Text = $"PRÉPARER TOUT ({_session.Media.Count})";
        immichAlbum.Enabled = !_isBusy && _settings.ImmichEnabled;
        immichNewAlbum.Enabled = immichAlbum.Enabled && Equals(immichAlbum.SelectedItem, NewAlbumEntry);
        immichSendSelected.Enabled = canEditSelection;
        immichSendSelected.Text = $"ENVOYER LA SÉLECTION ({selected.Count})";
        immichSendAll.Enabled = !_isBusy && hasMedia;
        immichSendAll.Text = $"ENVOYER TOUT ({_session.Media.Count})";
        findGpsMenuItem.Enabled = findGpsQuickItem.Enabled = canSearchGps;
        var canIdentifyLocation = _currentLocation is not null ||
            SelectedItems.Any(item => item.EffectiveLatitude.HasValue && item.EffectiveLongitude.HasValue);
        reverseGpsCommand.Enabled = reverseGpsQuickItem.Enabled = !_isBusy && canIdentifyLocation;
        operationStatus.Enabled = true;
    }

    private void ShowLogs()
    {
        using var dialog = new LogViewerForm();
        ThemeService.Apply(dialog);
        dialog.ShowDialog(this);
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
            ThemedMessageBox.Show(ex.Message, "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async Task VerifyExifToolAsync()
    {
        try
        {
            SetBusy(true);
            var version = await _exifTool.GetVersionAsync(StartOperation());
            ThemedMessageBox.Show($"ExifTool {version} is available.\n\n{_exifTool.ExecutablePath}", "ExifTool verification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (OperationCanceledException)
        {
            AppLogger.Info("ExifTool verification cancelled.");
        }
        catch (Exception ex)
        {
            AppLogger.Error("ExifTool verification failed.", ex);
            ThemedMessageBox.Show(ex.Message, "ExifTool unavailable", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private static void ShowAbout()
    {
        var version = typeof(Form1).Assembly.GetName().Version?.ToString(3) ?? "unknown";
        ThemedMessageBox.Show($"ExifTweaker {version}\n\nBatch date, timezone and GPS metadata editor powered by ExifTool.", "About ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

    private async void OnThemeChanged(object? sender, EventArgs e)
    {
        if (IsDisposed) return;
        ThemeService.Apply(_gpsSuggestionsPopup);
        ThemeService.Apply(gridContextMenu);
        ThemeService.Apply(headerContextMenu);
        try { await _map.SetThemeAsync(ThemeService.IsDarkNow); }
        catch (Exception ex) { AppLogger.Error("Map theme update failed.", ex); }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing && _session.HasPendingChanges &&
            ThemedMessageBox.Show("Des modifications préparées n’ont pas été appliquées.\n\nVoulez-vous vraiment fermer l’application ?", "ExifTweaker", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
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
        ThemeService.ThemeChanged -= OnThemeChanged;
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
        if (!busy && _sessionRefreshPending) QueueSessionRefresh();
    }

}
