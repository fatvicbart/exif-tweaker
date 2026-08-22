namespace ExifTweaker.Models;

public sealed record PhotoMetadata
{
    public DateTime? CaptureDate { get; init; }
    public TimeSpan? Offset { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
    public double? Altitude { get; init; }
    public string? CameraMake { get; init; }
    public string? CameraModel { get; init; }
    public string? Lens { get; init; }
    public int? Orientation { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public string? FileType { get; init; }
    public string? MimeType { get; init; }
    public DateTime? FileCreateDate { get; init; }
    public DateTime? FileModifyDate { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
}
