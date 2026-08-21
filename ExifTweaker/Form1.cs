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
    private readonly MapControl _map = new() { Visible = false };
    private readonly BindingSource _bindingSource = new();
    private ToolStrip? _commands;
    private Func<PhotoItem, bool> _activeFilter = _ => true;
    private BindingList<PhotoItem> _files => _session.Media;
    private readonly AppSettings _settings = AppSettings.Load();
    private readonly FileDiscoveryService _discovery = new();
    private readonly MetadataService _metadata;
    private readonly GeocodingService _geocoding;
    private CancellationTokenSource? _operationCts;

    private List<PhotoItem> SelectedItems => dgv.SelectedRows.Cast<DataGridViewRow>()
        .Select(row => row.DataBoundItem as PhotoItem).Where(item => item is not null).Cast<PhotoItem>().ToList();

    public Form1()
    {
        _metadata = new MetadataService(new ExifToolService(_settings.ExifToolPath), _settings);
        _geocoding = new GeocodingService(_settings);
        _sessionController = new SessionController(_session, _history);
        InitializeComponent();
        dgv.AutoGenerateColumns = true;
        _bindingSource.DataSource = _files;
        dgv.DataSource = _bindingSource;
        ConfigureGrid();
        bChange.Text = "STAGE";
        CreateCommands();
        splitContainer1.Panel2.Controls.Add(_map);
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
        var message = string.Format("Apply metadata changes to {0} file(s)?\nDates: {1} | Locations: {2} | Offsets: {3}", preview.FileCount, preview.DateChanges, preview.LocationChanges, preview.OffsetChanges);
        if (MessageBox.Show(message, "Confirm Apply", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
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
        if (_map.Visible && item.EffectiveLatitude is double latitude && item.EffectiveLongitude is double longitude) _ = _map.SetMarkerAsync(latitude, longitude);
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
        return base.ProcessCmdKey(ref message, keyData);
    }

    private void CreateCommands()
    {
        var commands = _commands = new ToolStrip { Dock = DockStyle.Top };
        commands.Items.Add("Apply", null, async (_, _) => await ApplyPendingChangesAsync(_session.Media));
        commands.Items.Add("Undo", null, (_, _) => { if (_history.Undo(_session.Media)) _session.NotifyChanged(); });
        commands.Items.Add("Redo", null, (_, _) => { if (_history.Redo(_session.Media)) _session.NotifyChanged(); });
        commands.Items.Add("Reset selected", null, (_, _) => ResetPatches(SelectedItems));
        commands.Items.Add("Reset all", null, (_, _) => ResetPatches(_session.Media));
        commands.Items.Add("-1 hour", null, (_, _) => ShiftSelected(TimeSpan.FromHours(-1)));
        commands.Items.Add("+1 hour", null, (_, _) => ShiftSelected(TimeSpan.FromHours(1)));
        commands.Items.Add("-1 minute", null, (_, _) => ShiftSelected(TimeSpan.FromMinutes(-1)));
        commands.Items.Add("+1 minute", null, (_, _) => ShiftSelected(TimeSpan.FromMinutes(1)));
        commands.Items.Add("Remove GPS", null, (_, _) => RemoveGpsSelected());
        commands.Items.Add("Set GPS", null, (_, _) => StageGpsFromFields());
        commands.Items.Add("Map", null, (_, _) => ToggleMap());
        commands.Items.Add("All", null, (_, _) => ApplyFilter(_ => true));
        commands.Items.Add("Modified", null, (_, _) => ApplyFilter(item => item.PendingChanges.HasChanges));
        commands.Items.Add("No GPS", null, (_, _) => ApplyFilter(item => !item.EffectiveLatitude.HasValue || !item.EffectiveLongitude.HasValue));
        commands.Items.Add("Errors", null, (_, _) => ApplyFilter(item => item.Error is not null));
        commands.Items.Add("Restore backup", null, async (_, _) => await RestoreSelectedAsync());
        Controls.Add(commands);
        commands.BringToFront();
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
        if (!TryCoordinate(tLat.Text, out var latitude) || !TryCoordinate(tLon.Text, out var longitude) || !IsValidCoordinate(latitude, longitude))
        {
            MessageBox.Show("Latitude/longitude invalid.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var selected = SelectedItems;
        _sessionController.SetLocation(selected, latitude, longitude, _locations);
    }

    private void ToggleMap()
    {
        _map.Visible = !_map.Visible;
        if (_map.Visible) _map.BringToFront();
        var active = SelectedItems.FirstOrDefault();
        if (_map.Visible && active?.EffectiveLatitude is double latitude && active.EffectiveLongitude is double longitude) _ = _map.SetMarkerAsync(latitude, longitude);
    }

    private void SetLocationFromMap(double latitude, double longitude)
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        _sessionController.SetLocation(selected, latitude, longitude, _locations);
        tLat.Text = latitude.ToString(CultureInfo.InvariantCulture);
        tLon.Text = longitude.ToString(CultureInfo.InvariantCulture);
    }

    private void RemoveGpsSelected()
    {
        var selected = SelectedItems;
        _sessionController.RemoveLocation(selected, _locations);
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

    private void ConfigureGrid()
    {
        dgv.AutoGenerateColumns = false;
        dgv.MultiSelect = true;
        dgv.Columns.Clear();
        foreach (var name in new[] { "FileName", "Date", "Latitude", "Longitude", "City", "Country", "Status" })
            dgv.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = name, Name = name, HeaderText = name });
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
        if (_commands is not null) _commands.Enabled = !busy;
        if (busy) pgb.Value = 0;
    }

    private static bool TryCoordinate(string value, out double result) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result) ||
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
    private static bool IsValidCoordinate(double latitude, double longitude) => latitude is >= -90 and <= 90 && longitude is >= -180 and <= 180;
}
