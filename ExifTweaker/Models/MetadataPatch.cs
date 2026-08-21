namespace ExifTweaker.Models;

public sealed class MetadataPatch
{
    public DateTime? CaptureDate { get; set; }
    public TimeSpan? DateShift { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool RemoveLocation { get; set; }

    public bool HasChanges => CaptureDate.HasValue || DateShift.HasValue ||
                              (Latitude.HasValue && Longitude.HasValue) || RemoveLocation;

    public void Clear()
    {
        CaptureDate = null;
        DateShift = null;
        Latitude = null;
        Longitude = null;
        RemoveLocation = false;
    }
}
