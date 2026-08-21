using System.ComponentModel;
using System.Globalization;
using ExifTweaker.Infrastructure;
using ExifTweaker.Controls;
using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker;

public partial class Form1 : Form
{
    private readonly ImportSession _session = new();
    private readonly EditHistory _history = new();
    private readonly SessionController _sessionController;
    private readonly ThumbnailService _thumbnails = new();
    private readonly LocationEditorService _locations = new();
    private MapControl _map => mapControl;
    private readonly BindingSource _bindingSource = new();
    private Func<PhotoItem, bool> _activeFilter = _ => true;
    private BindingList<PhotoItem> _files => _session.Media;
    private readonly AppSettings _settings = AppSettings.Load();
    private readonly FileDiscoveryService _discovery = new();
    private readonly MetadataService _metadata;
    private readonly IGeocodingService _geocoding;
    private GpsClipboard? _gpsClipboard;
    private CancellationTokenSource? _operationCts;

    private List<PhotoItem> SelectedItems => dgv.SelectedRows.Cast<DataGridViewRow>()
        .Select(row => row.DataBoundItem as PhotoItem).Where(item => item is not null).Cast<PhotoItem>().ToList();

    public Form1()
    {
        _metadata = new MetadataService(new ExifToolService(_settings.ExifToolPath), _settings);
        _geocoding = new GeocodingService(_settings);
        _sessionController = new SessionController(_session, _history);
        InitializeComponent();
        _bindingSource.DataSource = _files;
        dgv.DataSource = _bindingSource;
        bChange.Text = "STAGE";
        WireCommands();
        _map.BringToFront();
        _map.LocationChanged += (_, point) => SetLocationFromMap(point.Latitude, point.Longitude);
        Shown += async (_, _) =>
        {
            try { await _map.InitializeAsync(); }
            catch (Exception ex) { AppLogger.Error("Map initialization failed.", ex); MessageBox.Show(ex.Message, "Map unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        };
        _session.PropertyChanged += (_, _) => { UpdateSessionCaption(); RefreshFilter(); };
        UpdateSessionCaption();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        _sessionController.StageDate(selected, dateTimePicker1.Value);
    }

    private async void button2_Click(object sender, EventArgs e)
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
            var existing = _files.Select(item => item.FilePath).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var newPaths = discovery.Files.Where(path => !existing.Contains(path)).ToList();
            if (newPaths.Count == 0) return;
            pgb.Value = 5;
            var progress = new Progress<int>(value => pgb.Value = Math.Clamp(5 + value * 95 / 100, 0, 100));
            var items = await _metadata.LoadAsync(newPaths, progress, ct);
            _session.AddRange(items);
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
            if (result.FailedCount == 0) _history.Clear();
            _session.NotifyChanged();
            using var report = new ApplyReportForm(result);
            report.ShowDialog(this);
        }
        catch (OperationCanceledException) { AppLogger.Info("Apply cancelled."); }
        finally { SetBusy(false); }
    }

    private async void bGPS_Click(object sender, EventArgs e)
    {
        try
        {
            SetBusy(true);
            var results = await _geocoding.SearchAsync(tGPS.Text, StartOperation());
            if (results.Count == 0) { MessageBox.Show("No location found."); return; }
            using var chooser = new GeocodingSelectionForm(results);
            if (chooser.ShowDialog(this) != DialogResult.OK || chooser.Selected is not Coordinates selected) return;
            tLat.Text = selected.Latitude.ToString(CultureInfo.InvariantCulture);
            tLon.Text = selected.Longitude.ToString(CultureInfo.InvariantCulture);
            tAlt.Text = selected.Altitude?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            tName.Text = selected.Name;
            tType.Text = selected.Type;
        }
        catch (OperationCanceledException) { AppLogger.Info("Geocoding cancelled."); }
        catch (Exception ex) { AppLogger.Error("Geocoding failed.", ex); MessageBox.Show(ex.Message, "Geocoding error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { SetBusy(false); }
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
        var image = await _thumbnails.GetAsync(item.FilePath, 1600);
        var previous = picBox.Image;
        picBox.Image = image;
        previous?.Dispose();
    }

    private void dgv_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
    {
        if (sender is not DataGridView grid || !grid.RowHeadersVisible) return;
        var rectangle = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
        TextRenderer.DrawText(e.Graphics, e.RowIndex.ToString(), grid.RowHeadersDefaultCellStyle.Font, rectangle, grid.RowHeadersDefaultCellStyle.ForeColor);
    }

    protected override bool ProcessCmdKey(ref Message message, Keys keyData)
    {
        if (keyData == Keys.Escape && _operationCts is { IsCancellationRequested: false }) { _operationCts.Cancel(); return true; }
        if (keyData == (Keys.Control | Keys.A)) { dgv.SelectAll(); return true; }
        if (keyData == (Keys.Control | Keys.Z)) { if (_history.Undo(_session.Media)) _session.NotifyChanged(); return true; }
        if (keyData == (Keys.Control | Keys.Y)) { if (_history.Redo(_session.Media)) _session.NotifyChanged(); return true; }
        return base.ProcessCmdKey(ref message, keyData);
    }

    private ToolStripItem Command(string name) =>
        commands.Items[name] ?? throw new InvalidOperationException($"Designer command {name} not found.");

    private void WireCommands()
    {
        Command("applyCommand").Click += async (_, _) => await ApplyPendingChangesAsync(_session.Media.ToList());
        Command("undoCommand").Click += (_, _) => { if (_history.Undo(_session.Media)) _session.NotifyChanged(); };
        Command("redoCommand").Click += (_, _) => { if (_history.Redo(_session.Media)) _session.NotifyChanged(); };
        Command("resetSelectedCommand").Click += (_, _) => ResetPatches(SelectedItems);
        Command("resetAllCommand").Click += (_, _) => ResetPatches(_session.Media);
        Command("minusHourCommand").Click += (_, _) => ShiftSelected(TimeSpan.FromHours(-1));
        Command("plusHourCommand").Click += (_, _) => ShiftSelected(TimeSpan.FromHours(1));
        Command("minusMinuteCommand").Click += (_, _) => ShiftSelected(TimeSpan.FromMinutes(-1));
        Command("plusMinuteCommand").Click += (_, _) => ShiftSelected(TimeSpan.FromMinutes(1));
        Command("removeGpsCommand").Click += (_, _) => RemoveGpsSelected();
        Command("setGpsCommand").Click += (_, _) => StageGpsFromFields();
        Command("copyGpsCommand").Click += (_, _) => CopyGpsSelected();
        Command("pasteGpsCommand").Click += (_, _) => PasteGpsSelected();
        Command("reverseGpsCommand").Click += async (_, _) => await ReverseGpsSelectedAsync();
        Command("mapCommand").Click += (_, _) => ToggleMap();
        Command("allFilterCommand").Click += (_, _) => ApplyFilter(_ => true);
        Command("modifiedFilterCommand").Click += (_, _) => ApplyFilter(item => item.PendingChanges.HasChanges);
        Command("noGpsFilterCommand").Click += (_, _) => ApplyFilter(item => !item.EffectiveLatitude.HasValue || !item.EffectiveLongitude.HasValue);
        Command("noDateFilterCommand").Click += (_, _) => ApplyFilter(item => !item.EffectiveCaptureDate.HasValue);
        Command("errorsFilterCommand").Click += (_, _) => ApplyFilter(item => item.Error is not null);
        Command("restoreBackupCommand").Click += async (_, _) => await RestoreSelectedAsync();
    }

    private void ApplyFilter(Func<PhotoItem, bool> predicate)
    {
        _activeFilter = predicate;
        RefreshFilter();
    }

    private void RefreshFilter()
    {
        _bindingSource.DataSource = new BindingList<PhotoItem>(_session.Media.Where(_activeFilter).ToList());
    }

    private void StageGpsFromFields()
    {
        if (!TryCoordinate(tLat.Text, out var latitude) || !TryCoordinate(tLon.Text, out var longitude) || !TryOptionalCoordinate(tAlt.Text, out var altitude) || !IsValidCoordinate(latitude, longitude, altitude))
        {
            MessageBox.Show("Latitude, longitude or altitude invalid.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var selected = SelectedItems;
        _sessionController.SetLocation(selected, latitude, longitude, altitude, _locations);
        RefreshMapMarkers();
    }

    private void ToggleMap()
    {
        _map.Visible = !_map.Visible;
        if (_map.Visible) _map.BringToFront();
        RefreshMapMarkers();
    }

    private void SetLocationFromMap(double latitude, double longitude)
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        if (!TryOptionalCoordinate(tAlt.Text, out var altitude) || !IsValidCoordinate(latitude, longitude, altitude))
        {
            MessageBox.Show("Latitude, longitude or altitude invalid.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        _sessionController.SetLocation(selected, latitude, longitude, altitude, _locations);
        tLat.Text = latitude.ToString(CultureInfo.InvariantCulture);
        tLon.Text = longitude.ToString(CultureInfo.InvariantCulture);
        RefreshMapMarkers();
    }

    private void RemoveGpsSelected()
    {
        var selected = SelectedItems;
        _sessionController.RemoveLocation(selected, _locations);
        tLat.Clear(); tLon.Clear(); tAlt.Clear();
        RefreshMapMarkers();
    }

    private void CopyGpsSelected()
    {
        var active = SelectedItems.FirstOrDefault();
        if (active is null) return;
        try { _gpsClipboard = LocationEditorService.CopyLocation(active); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
    }

    private void PasteGpsSelected()
    {
        if (_gpsClipboard is not { } gps || SelectedItems.Count == 0) return;
        _sessionController.SetLocation(SelectedItems, gps.Latitude, gps.Longitude, gps.Altitude, _locations);
        tLat.Text = gps.Latitude.ToString(CultureInfo.InvariantCulture);
        tLon.Text = gps.Longitude.ToString(CultureInfo.InvariantCulture);
        tAlt.Text = gps.Altitude?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        RefreshMapMarkers();
    }

    private async Task ReverseGpsSelectedAsync()
    {
        var active = SelectedItems.FirstOrDefault();
        var latitudeText = active?.EffectiveLatitude?.ToString(CultureInfo.InvariantCulture) ?? tLat.Text;
        var longitudeText = active?.EffectiveLongitude?.ToString(CultureInfo.InvariantCulture) ?? tLon.Text;
        if (!TryCoordinate(latitudeText, out var latitude) || !TryCoordinate(longitudeText, out var longitude)) return;
        try
        {
            SetBusy(true);
            var result = await _geocoding.ReverseAsync(latitude, longitude, StartOperation());
            if (result is null) { MessageBox.Show("No reverse geocoding result found."); return; }
            tName.Text = result.Name;
            tType.Text = result.Type;
            tGPS.Text = result.Name;
        }
        catch (OperationCanceledException) { AppLogger.Info("Reverse geocoding cancelled."); }
        catch (Exception ex) { AppLogger.Error("Reverse geocoding failed.", ex); MessageBox.Show(ex.Message, "Geocoding error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { SetBusy(false); }
    }

    private void DisplayActiveMetadata(PhotoItem item)
    {
        if (item.EffectiveCaptureDate is DateTime captureDate) dateTimePicker1.Value = captureDate;
        tLat.Text = item.EffectiveLatitude?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        tLon.Text = item.EffectiveLongitude?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        tAlt.Text = item.EffectiveAltitude?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
        tName.Text = string.Join(", ", new[] { item.City, item.Country }.Where(value => !string.IsNullOrWhiteSpace(value)));
        tType.Text = item.Original.FileType ?? string.Empty;
    }

    private void RefreshMapMarkers()
    {
        if (!_map.Visible) return;
        var active = SelectedItems.FirstOrDefault();
        var markers = _session.Media
            .Where(item => item.EffectiveLatitude.HasValue && item.EffectiveLongitude.HasValue)
            .Select(item => new MapMarker(item.EffectiveLatitude!.Value, item.EffectiveLongitude!.Value, item.FileName, ReferenceEquals(item, active)))
            .ToList();
        _ = _map.SetMarkersAsync(markers);
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
            _session.NotifyChanged();
            using var report = new ApplyReportForm(result) { Text = "Restore report" };
            report.ShowDialog(this);
        }
        catch (OperationCanceledException) { AppLogger.Info("Restore cancelled."); }
        finally { SetBusy(false); }
    }

    private void ResetPatches(IEnumerable<PhotoItem> items)
    {
        var list = items.ToList();
        _sessionController.Reset(list);
    }

    private void UpdateSessionCaption()
    {
        var statistics = _session.Statistics;
        var range = statistics.FirstCaptureDate is DateTime first && statistics.LastCaptureDate is DateTime last
            ? $" | {first:yyyy-MM-dd} to {last:yyyy-MM-dd}"
            : string.Empty;
        Text = $"ExifTweaker — {statistics.MediaCount} media | {statistics.FilesWithGps} GPS | {statistics.PendingChangeCount} pending{range}";
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
        base.OnFormClosing(e);
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
        main.Enabled = !busy;
        commands.Enabled = !busy;
        if (busy) pgb.Value = 0;
    }

    private static bool TryCoordinate(string value, out double result) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result) ||
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);

    private static bool TryOptionalCoordinate(string value, out double? result)
    {
        if (string.IsNullOrWhiteSpace(value)) { result = null; return true; }
        if (TryCoordinate(value, out var parsed)) { result = parsed; return true; }
        result = null;
        return false;
    }

    private static bool IsValidCoordinate(double latitude, double longitude, double? altitude = null) =>
        latitude is >= -90 and <= 90 &&
        longitude is >= -180 and <= 180 &&
        (!altitude.HasValue || altitude.Value is >= -12000 and <= 100000);
}
