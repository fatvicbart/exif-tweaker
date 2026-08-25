using System.Text.Json;

namespace ExifTweaker.Infrastructure;

public enum BackupStrategy { ExifToolOriginal, OverwriteOriginal }
public enum AppThemeMode { Automatic, Light, Dark }

public sealed class AppSettings
{
    public string GeocodingProvider { get; set; } = "Nominatim";
    public string? MapsCoApiKey { get; set; }
    public string? ExifToolPath { get; set; }
    public BackupStrategy BackupStrategy { get; set; } = BackupStrategy.ExifToolOriginal;
    public AppThemeMode Theme { get; set; } = AppThemeMode.Automatic;
    public int MaxParallelism { get; set; } = Math.Clamp(Environment.ProcessorCount, 2, 8);
    public bool RecursiveImport { get; set; } = true;
    public bool ThumbnailDiskCache { get; set; } = true;
    public string MapTileUrl { get; set; } = "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png";
    public string MapAttribution { get; set; } = "OpenStreetMap contributors";
    public bool CheckForUpdatesAutomatically { get; set; } = true;
    public bool IncludePrereleaseUpdates { get; set; } = false;
    public bool ConfirmBulkPrepare { get; set; } = true;
    public List<string> HiddenColumns { get; set; } = new();
    public bool ImmichEnabled { get; set; }
    public string ImmichServerUrl { get; set; } = string.Empty;
    public string? ImmichDefaultAlbumId { get; set; }
    public string? ImmichDefaultAlbumName { get; set; }
    public string ImmichDefaultVisibility { get; set; } = "timeline";
    public int ImmichUploadConcurrency { get; set; } = 3;

    public static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExifTweaker", "settings.json");
    public static string CacheDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExifTweaker", "cache");

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var saved = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath));
                if (saved is not null) return saved.ApplyEnvironmentOverrides();
            }
        }
        catch (Exception ex) { AppLogger.Error("Unable to load settings.", ex); }
        return new AppSettings().ApplyEnvironmentOverrides();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            AppLogger.Error("Unable to save settings.", ex);
            throw;
        }
    }

    private AppSettings ApplyEnvironmentOverrides()
    {
        MapsCoApiKey = Environment.GetEnvironmentVariable("EXIFTWEAKER_MAPSCO_API_KEY") ?? MapsCoApiKey;
        ExifToolPath = Environment.GetEnvironmentVariable("EXIFTWEAKER_EXIFTOOL_PATH") ?? ExifToolPath;
        ImmichServerUrl = Environment.GetEnvironmentVariable("EXIFTWEAKER_IMMICH_URL") ?? ImmichServerUrl;
        MaxParallelism = Math.Clamp(MaxParallelism, 1, 16);
        GeocodingProvider = string.IsNullOrWhiteSpace(GeocodingProvider) ? "Nominatim" : GeocodingProvider;
        MapTileUrl = string.IsNullOrWhiteSpace(MapTileUrl) ? "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png" : MapTileUrl;
        ImmichUploadConcurrency = Math.Clamp(ImmichUploadConcurrency, 1, 8);
        ImmichDefaultVisibility = ImmichDefaultVisibility is "archive" or "hidden" or "locked" ? ImmichDefaultVisibility : "timeline";
        HiddenColumns ??= new List<string>();
        return this;
    }
}
