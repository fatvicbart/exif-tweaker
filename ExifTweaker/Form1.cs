using System.ComponentModel;
using System.Globalization;
using ExifTweaker.Infrastructure;
using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker;

public partial class Form1 : Form
{
    private readonly BindingList<PhotoItem> _files = new();
    private readonly FileDiscoveryService _discovery = new();
    private readonly ExifToolService _exifTool = new();
    private readonly GeocodingService _geocoding = new(new AppSettings());
    private CancellationTokenSource? _operationCts;

    private List<PhotoItem> SelectedItems => dgv.SelectedRows.Cast<DataGridViewRow>()
        .Select(r => r.DataBoundItem as PhotoItem).Where(x => x is not null).Cast<PhotoItem>().ToList();

    public Form1()
    {
        InitializeComponent();
        dgv.AutoGenerateColumns = true;
        dgv.DataSource = new BindingSource { DataSource = _files };
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        var selected = SelectedItems;
        if (selected.Count == 0) return;
        if (!TryCoordinate(tLat.Text, out var lat) || !TryCoordinate(tLon.Text, out var lon))
        {
            MessageBox.Show("Latitude/longitude invalid.", "ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Phase 0-4 compatibility: preserve the current Change button behaviour,
        // but route it through PendingChanges + ExifTool instead of ExifLibrary.
        foreach (var photo in selected)
        {
            photo.PendingChanges.CaptureDate = dateTimePicker1.Value;
            photo.PendingChanges.Latitude = lat;
            photo.PendingChanges.Longitude = lon;
            photo.NotifyChanged();
        }
        await ApplyPendingChangesAsync(selected);
    }

    private async void button2_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog
        {
            InitialDirectory = Environment.CurrentDirectory,
            Filter = _discovery.DialogFilter,
            FilterIndex = 1,
            RestoreDirectory = true,
            Multiselect = true,
            Title = "Select photos and videos..."
        };
        if (dlg.ShowDialog() == DialogResult.OK) await AddFilesAsync(dlg.FileNames);
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
            _operationCts?.Cancel();
            _operationCts = new CancellationTokenSource();
            var ct = _operationCts.Token;
            var existing = _files.Select(x => x.FilePath).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var discovered = await Task.Run(() => _discovery.Discover(paths), ct);
            var newPaths = discovered.Where(x => !existing.Contains(x)).ToList();
            if (newPaths.Count == 0) return;

            pgb.Value = 10;
            var metadata = await _exifTool.ReadAsync(newPaths, ct);
            for (var i = 0; i < newPaths.Count; i++)
            {
                var path = newPaths[i];
                var item = new PhotoItem(path);
                if (metadata.TryGetValue(Path.GetFullPath(path), out var md)) item.Original = md;
                else item.Error = "Metadata not returned by ExifTool";
                _files.Add(item);
                pgb.Value = Math.Min(99, 10 + (int)(89d * (i + 1) / newPaths.Count));
            }
            pgb.Value = 100;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Import error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { SetBusy(false); }
    }

    private async Task ApplyPendingChangesAsync(IReadOnlyList<PhotoItem> photos)
    {
        try
        {
            SetBusy(true);
            _operationCts?.Cancel();
            _operationCts = new CancellationTokenSource();
            var ct = _operationCts.Token;
            var changed = photos.Where(p => p.PendingChanges.HasChanges).ToList();
            for (var i = 0; i < changed.Count; i++)
            {
                var photo = changed[i];
                try
                {
                    await _exifTool.WriteAsync(photo, backupOriginal: true, ct);
                    var refreshed = await _exifTool.ReadAsync(new[] { photo.FilePath }, ct);
                    if (refreshed.TryGetValue(Path.GetFullPath(photo.FilePath), out var md)) photo.Original = md;
                    photo.PendingChanges.Clear(); photo.Error = null; photo.NotifyChanged();
                }
                catch (Exception ex) { photo.Error = ex.Message; }
                pgb.Value = changed.Count == 0 ? 100 : (int)(100d * (i + 1) / changed.Count);
            }
        }
        catch (OperationCanceledException) { }
        finally { SetBusy(false); }
    }

    private async void bGPS_Click(object sender, EventArgs e)
    {
        try
        {
            SetBusy(true);
            var results = await _geocoding.SearchAsync(tGPS.Text);
            var first = results.FirstOrDefault();
            if (first is null) { MessageBox.Show("No location found."); return; }
            tLat.Text = first.Latitude.ToString(CultureInfo.InvariantCulture);
            tLon.Text = first.Longitude.ToString(CultureInfo.InvariantCulture);
            tName.Text = first.Name;
            tType.Text = first.Type;
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Geocoding error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally { SetBusy(false); }
    }

    private void dgv_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Delete) return;
        foreach (var item in SelectedItems) _files.Remove(item);
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
        if (sender is not DataGridView g || !g.RowHeadersVisible) return;
        var r = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, g.RowHeadersWidth, e.RowBounds.Height);
        TextRenderer.DrawText(e.Graphics, e.RowIndex.ToString(), g.RowHeadersDefaultCellStyle.Font, r, g.RowHeadersDefaultCellStyle.ForeColor);
    }

    private void SetBusy(bool busy)
    {
        main.Enabled = !busy;
        if (busy) pgb.Value = 0;
    }

    private static bool TryCoordinate(string value, out double result) =>
        double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out result) ||
        double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
}
