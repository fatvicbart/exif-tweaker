using System.ComponentModel;
using System.Globalization;
using ExifTweaker.Infrastructure;
using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker;

public partial class Form1 : Form
{
    private readonly ImportSession _session = new();
    private readonly EditHistory _history = new();
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
        InitializeComponent();
        dgv.AutoGenerateColumns = true;
        dgv.DataSource = new BindingSource { DataSource = _files };
        ConfigureGrid();
        bChange.Text = "STAGE";
        _session.PropertyChanged += (_, _) => UpdateSessionCaption();
        UpdateSessionCaption();
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        var hasLatitude = !string.IsNullOrWhiteSpace(tLat.Text);
        var hasLongitude = !string.IsNullOrWhiteSpace(tLon.Text);
        if (hasLatitude != hasLongitude || (hasLatitude && (!TryCoordinate(tLat.Text, out var lat) || !TryCoordinate(tLon.Text, out var lon) || !IsValidCoordinate(lat, lon))))
        {
            MessageBox.Show("Latitude/longitude invalid.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _history.Capture(selected);
        foreach (var photo in selected)
        {
            photo.PendingChanges.CaptureDate = dateTimePicker1.Value;
            if (hasLatitude)
            {
                TryCoordinate(tLat.Text, out var latitude);
                TryCoordinate(tLon.Text, out var longitude);
                photo.PendingChanges.Latitude = latitude;
                photo.PendingChanges.Longitude = longitude;
            }
            photo.NotifyChanged();
        }
        _session.NotifyChanged();
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
            if (result.FailedCount > 0)
                MessageBox.Show($"{result.FailedCount} file(s) could not be updated. See their status and the application log.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            var first = results.FirstOrDefault();
            if (first is null) { MessageBox.Show("No location found."); return; }
            tLat.Text = first.Latitude.ToString(CultureInfo.InvariantCulture);
            tLon.Text = first.Longitude.ToString(CultureInfo.InvariantCulture);
            tName.Text = first.Name;
            tType.Text = first.Type;
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

    private void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.RowIndex < 0 || dgv.Rows[e.RowIndex].DataBoundItem is not PhotoItem item) return;
        try
        {
            using var stream = new FileStream(item.FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var source = Image.FromStream(stream);
            var clone = new Bitmap(source);
            var previous = picBox.Image; picBox.Image = clone; previous?.Dispose();
        }
        catch { var previous = picBox.Image; picBox.Image = null; previous?.Dispose(); }
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
        _history.Capture(list);
        foreach (var item in list) { item.PendingChanges.Clear(); item.NotifyChanged(); }
        _session.NotifyChanged();
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
        if (busy) pgb.Value = 0;
    }

    private static bool TryCoordinate(string value, out double result) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result) ||
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
    private static bool IsValidCoordinate(double latitude, double longitude) => latitude is >= -90 and <= 90 && longitude is >= -180 and <= 180;
}
