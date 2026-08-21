using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed record GeocodingResult(double Latitude, double Longitude, string Name, string Type);

public interface IGeocodingService
{
    Task<IReadOnlyList<GeocodingResult>> SearchAsync(string query, CancellationToken ct = default);
}

public sealed class LocationEditorService
{
    public void SetLocation(IEnumerable<PhotoItem> items, double latitude, double longitude)
    {
        Validate(latitude, longitude);
        foreach (var item in items)
        {
            item.PendingChanges.RemoveLocation = false;
            item.PendingChanges.Latitude = latitude;
            item.PendingChanges.Longitude = longitude;
            item.NotifyChanged();
        }
    }

    public void RemoveLocation(IEnumerable<PhotoItem> items)
    {
        foreach (var item in items)
        {
            item.PendingChanges.RemoveLocation = true;
            item.PendingChanges.Latitude = null;
            item.PendingChanges.Longitude = null;
            item.NotifyChanged();
        }
    }

    public static void Validate(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(latitude), "GPS coordinates are outside their valid ranges.");
    }
}
