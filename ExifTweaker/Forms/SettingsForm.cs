using ExifTweaker.Infrastructure;
using ExifTweaker.Services;

namespace ExifTweaker.Forms;

public sealed partial class SettingsForm : Form
{
    private readonly AppSettings _settings;
    private readonly AppThemeMode _originalTheme;

    public SettingsForm(AppSettings settings)
    {
        _settings = settings;
        _originalTheme = settings.Theme;
        InitializeComponent();
        theme.SelectedIndexChanged += (_, _) => ThemeService.SetMode(SelectedTheme);
        provider.Items.AddRange(new object[] { "Maps.co", "Nominatim" });
        provider.SelectedItem = provider.Items.Cast<string>().FirstOrDefault(x => x.Equals(settings.GeocodingProvider, StringComparison.OrdinalIgnoreCase)) ?? "Nominatim";
        apiKey.Text = settings.MapsCoApiKey ?? string.Empty;
        exifToolPath.Text = settings.ExifToolPath ?? string.Empty;
        backup.SelectedIndex = settings.BackupStrategy == BackupStrategy.ExifToolOriginal ? 0 : 1;
        parallelism.Value = Math.Clamp(settings.MaxParallelism, 1, 16);
        recursive.Checked = settings.RecursiveImport;
        diskCache.Checked = settings.ThumbnailDiskCache;
        mapTiles.Text = settings.MapTileUrl;
        theme.SelectedIndex = settings.Theme switch { AppThemeMode.Light => 1, AppThemeMode.Dark => 2, _ => 0 };
        ThemeService.Apply(this);
        autoUpdates.Checked = settings.CheckForUpdatesAutomatically;
        prereleaseUpdates.Checked = settings.IncludePrereleaseUpdates;
        confirmBulkPrepare.Checked = settings.ConfirmBulkPrepare;
        installedVersion.Text = $"Version {new UpdateService(settings).DisplayVersion}";
    }

    private AppThemeMode SelectedTheme => theme.SelectedIndex switch
    {
        1 => AppThemeMode.Light,
        2 => AppThemeMode.Dark,
        _ => AppThemeMode.Automatic
    };

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        ThemeService.Apply(this);
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        if (DialogResult != DialogResult.OK) ThemeService.SetMode(_originalTheme);
        base.OnFormClosed(e);
    }

    private void saveButton_Click(object? sender, EventArgs e)
    {
        _settings.GeocodingProvider = provider.SelectedItem?.ToString() ?? "Nominatim";
        _settings.MapsCoApiKey = string.IsNullOrWhiteSpace(apiKey.Text) ? null : apiKey.Text.Trim();
        _settings.ExifToolPath = string.IsNullOrWhiteSpace(exifToolPath.Text) ? null : exifToolPath.Text.Trim();
        _settings.BackupStrategy = backup.SelectedIndex == 0 ? BackupStrategy.ExifToolOriginal : BackupStrategy.OverwriteOriginal;
        _settings.MaxParallelism = (int)parallelism.Value;
        _settings.RecursiveImport = recursive.Checked;
        _settings.ThumbnailDiskCache = diskCache.Checked;
        _settings.MapTileUrl = mapTiles.Text.Trim();
        _settings.Theme = SelectedTheme;
        _settings.CheckForUpdatesAutomatically = autoUpdates.Checked;
        _settings.IncludePrereleaseUpdates = prereleaseUpdates.Checked;
        _settings.ConfirmBulkPrepare = confirmBulkPrepare.Checked;
        _settings.Save();
        DialogResult = DialogResult.OK;
    }

    private async void checkUpdatesButton_Click(object? sender, EventArgs e)
    {
        checkUpdatesButton.Enabled = false;
        try { await new UpdateService(_settings).CheckAndPromptAsync(this, manual: true); }
        finally { if (!IsDisposed) checkUpdatesButton.Enabled = true; }
    }

    private void immichSettingsButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new ImmichSettingsForm(_settings);
        ThemeService.Apply(dialog);
        dialog.ShowDialog(this);
    }

    private void browseButton_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Filter = "ExifTool|exiftool.exe;exiftool|All files|*.*", CheckFileExists = true };
        if (dialog.ShowDialog(this) == DialogResult.OK) exifToolPath.Text = dialog.FileName;
    }
}
