namespace ExifTweaker.Infrastructure;

public sealed class AppSettings
{
    public string? MapsCoApiKey { get; init; } = Environment.GetEnvironmentVariable("EXIFTWEAKER_MAPSCO_API_KEY");
    public int MaxParallelism { get; init; } = Math.Clamp(Environment.ProcessorCount, 2, 8);
}
