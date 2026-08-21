using System.Text.Json;

namespace ExifTweaker.Infrastructure;

public enum BackupStrategy { ExifToolOriginal, OverwriteOriginal }

public sealed class AppSettings
{
    public string GeocodingProvider { get; init; } = "Maps.co";
    public string? MapsCoApiKey { get; init; }
    public string? ExifToolPath { get; init; }
    public BackupStrategy BackupStrategy { get; init; } = BackupStrategy.ExifToolOriginal;
    public int MaxParallelism { get; init; } = Math.Clamp(Environment.ProcessorCount, 2, 8);
    public bool RecursiveImport { get; init; } = true;

    public static string SettingsPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExifTweaker", "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var saved = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath));
                if (saved is not null) return saved.WithEnvironmentOverrides();
            }
        }
        catch (Exception ex) { AppLogger.Error("Unable to load settings.", ex); }
        return new AppSettings().WithEnvironmentOverrides();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }

    private AppSettings WithEnvironmentOverrides() => new()
    {
        GeocodingProvider = GeocodingProvider,
        MapsCoApiKey = Environment.GetEnvironmentVariable("EXIFTWEAKER_MAPSCO_API_KEY") ?? MapsCoApiKey,
        ExifToolPath = Environment.GetEnvironmentVariable("EXIFTWEAKER_EXIFTOOL_PATH") ?? ExifToolPath,
        BackupStrategy = BackupStrategy,
        MaxParallelism = Math.Clamp(MaxParallelism, 1, 16),
        RecursiveImport = RecursiveImport
    };
}
