using ExifTweaker.Infrastructure;
using ExifTweaker.Services;

namespace ExifTweaker.Forms;

public sealed partial class SettingsForm : Form
{
    private readonly AppSettings _settings;

    public SettingsForm(AppSettings settings)
    {
        _settings = settings;
        InitializeComponent();
        provider.Items.AddRange(new object[] { "Maps.co", "Nominatim" });
        provider.SelectedItem = provider.Items.Cast<string>().FirstOrDefault(x => x.Equals(settings.GeocodingProvider, StringComparison.OrdinalIgnoreCase)) ?? "Maps.co";
        apiKey.Text = settings.MapsCoApiKey ?? string.Empty;
        exifToolPath.Text = settings.ExifToolPath ?? string.Empty;
        backup.SelectedIndex = settings.BackupStrategy == BackupStrategy.ExifToolOriginal ? 0 : 1;
        parallelism.Value = Math.Clamp(settings.MaxParallelism, 1, 16);
        recursive.Checked = settings.RecursiveImport;
        diskCache.Checked = settings.ThumbnailDiskCache;
        mapTiles.Text = settings.MapTileUrl;
        autoUpdates.Checked = settings.CheckForUpdatesAutomatically;
        prereleaseUpdates.Checked = settings.IncludePrereleaseUpdates;
        installedVersion.Text = $"Version {new UpdateService(settings).DisplayVersion}";
    }

    private void saveButton_Click(object? sender, EventArgs e)
    {
        _settings.GeocodingProvider = provider.SelectedItem?.ToString() ?? "Maps.co";
        _settings.MapsCoApiKey = string.IsNullOrWhiteSpace(apiKey.Text) ? null : apiKey.Text.Trim();
        _settings.ExifToolPath = string.IsNullOrWhiteSpace(exifToolPath.Text) ? null : exifToolPath.Text.Trim();
        _settings.BackupStrategy = backup.SelectedIndex == 0 ? BackupStrategy.ExifToolOriginal : BackupStrategy.OverwriteOriginal;
        _settings.MaxParallelism = (int)parallelism.Value;
        _settings.RecursiveImport = recursive.Checked;
        _settings.ThumbnailDiskCache = diskCache.Checked;
        _settings.MapTileUrl = mapTiles.Text.Trim();
        _settings.CheckForUpdatesAutomatically = autoUpdates.Checked;
        _settings.IncludePrereleaseUpdates = prereleaseUpdates.Checked;
        _settings.Save();
        DialogResult = DialogResult.OK;
    }

    private async void checkUpdatesButton_Click(object? sender, EventArgs e)
    {
        checkUpdatesButton.Enabled = false;
        try { await new UpdateService(_settings).CheckAndPromptAsync(this, manual: true); }
        finally { if (!IsDisposed) checkUpdatesButton.Enabled = true; }
    }

    private void browseButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "ExifTool|exiftool.exe;exiftool|All files|*.*", CheckFileExists = true };
        if (dialog.ShowDialog(this) == DialogResult.OK) exifToolPath.Text = dialog.FileName;
    }
}
