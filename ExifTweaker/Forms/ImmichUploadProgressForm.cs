using ExifTweaker.Infrastructure;
using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker.Forms;

public sealed class ImmichUploadProgressForm : Form
{
    private readonly ImmichUploadService _service;
    private ImmichUploadRequest _request;
    private CancellationTokenSource? _cts;
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private readonly ProgressBar _progress = new() { Dock = DockStyle.Top, Height = 22 };
    private readonly Label _summary = new() { Dock = DockStyle.Top, Height = 32, TextAlign = ContentAlignment.MiddleLeft };
    private readonly Button _cancel = new() { Text = "Interrompre", AutoSize = true };
    private readonly Button _retry = new() { Text = "Réessayer les échecs", AutoSize = true, Enabled = false };
    private readonly Button _close = new() { Text = "Fermer", AutoSize = true, Enabled = false };
    private bool _running;
    public ImmichUploadResult? Result { get; private set; }

    public ImmichUploadProgressForm(ImmichUploadService service, ImmichUploadRequest request)
    {
        _service = service;
        _request = request;
        _grid.Columns.Add("file", "Fichier");
        _grid.Columns.Add("status", "État");
        _grid.Columns.Add("details", "Détails");
        _grid.Columns[0].FillWeight = 38;
        _grid.Columns[1].FillWeight = 18;
        _grid.Columns[2].FillWeight = 44;
        foreach (var path in request.FilePaths) _grid.Rows.Add(Path.GetFileName(path), "En attente", string.Empty);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 50, Padding = new Padding(8), FlowDirection = FlowDirection.RightToLeft };
        buttons.Controls.AddRange(new Control[] { _close, _cancel, _retry });
        Controls.Add(_grid);
        Controls.Add(_summary);
        Controls.Add(_progress);
        Controls.Add(buttons);
        _cancel.Click += (_, _) => _cts?.Cancel();
        _close.Click += (_, _) => Close();
        _retry.Click += async (_, _) => await RetryAsync();
        Shown += async (_, _) => await RunAsync();
        FormClosing += (_, e) => { if (_running) { e.Cancel = true; _cts?.Cancel(); } };
        ClientSize = new Size(850, 500);
        MinimumSize = new Size(700, 420);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Envoi vers Immich";
        ThemeService.Apply(this);
    }

    private async Task RunAsync()
    {
        _running = true;
        _cancel.Enabled = true;
        _retry.Enabled = false;
        _close.Enabled = false;
        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        var rowByPath = _request.FilePaths.Select((path, index) => (path, index)).ToDictionary(item => item.path, item => item.index, StringComparer.OrdinalIgnoreCase);
        var progress = new Progress<ImmichUploadProgress>(item =>
        {
            _progress.Maximum = Math.Max(1, item.Total);
            _progress.Value = Math.Clamp(item.Completed, 0, _progress.Maximum);
            _summary.Text = $"{item.Completed}/{item.Total} — {Path.GetFileName(item.FilePath)}";
            if (rowByPath.TryGetValue(item.FilePath, out var index))
            {
                _grid.Rows[index].Cells[1].Value = StatusText(item.Status);
                _grid.Rows[index].Cells[2].Value = item.Message ?? string.Empty;
            }
        });
        try
        {
            Result = await _service.UploadAsync(_request, progress, _cts.Token);
            foreach (var result in Result.Files)
            {
                if (!rowByPath.TryGetValue(result.FilePath, out var index)) continue;
                _grid.Rows[index].Cells[1].Value = StatusText(result.Status);
                _grid.Rows[index].Cells[2].Value = result.Error ?? string.Empty;
            }
            _summary.Text = $"{Result.Uploaded} envoyée(s), {Result.Duplicates} déjà présente(s), {Result.Failed} échec(s), {Result.Cancelled} annulée(s)";
        }
        catch (OperationCanceledException) { _summary.Text = "Opération interrompue."; }
        catch (Exception ex)
        {
            AppLogger.Error("Immich batch upload failed.", ex);
            _summary.Text = "L’envoi a échoué.";
            ThemedMessageBox.Show(ex.Message, "Envoi Immich", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _running = false;
            _cancel.Enabled = false;
            _close.Enabled = true;
            _retry.Enabled = Result?.Failed > 0;
        }
    }

    private async Task RetryAsync()
    {
        var failed = Result?.Files.Where(file => file.Status == ImmichUploadStatus.Failed).Select(file => file.FilePath).ToList() ?? [];
        if (failed.Count == 0) return;
        _request = _request with { FilePaths = failed, AlbumId = _service.LastResolvedAlbumId, NewAlbumName = null };
        _grid.Rows.Clear();
        foreach (var path in failed) _grid.Rows.Add(Path.GetFileName(path), "En attente", string.Empty);
        await RunAsync();
    }

    private static string StatusText(ImmichUploadStatus status) => status switch
    {
        ImmichUploadStatus.Uploading => "Envoi…",
        ImmichUploadStatus.Uploaded => "Envoyée",
        ImmichUploadStatus.Duplicate => "Déjà présente",
        ImmichUploadStatus.Failed => "Échec",
        ImmichUploadStatus.Cancelled => "Annulée",
        _ => "En attente"
    };

    protected override void Dispose(bool disposing)
    {
        if (disposing) _cts?.Dispose();
        base.Dispose(disposing);
    }
}
