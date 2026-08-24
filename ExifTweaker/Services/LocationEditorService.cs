using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed class LocationEditorService
{
    public void SetLocation(IEnumerable<PhotoItem> items, double latitude, double longitude, double? altitude = null)
    {
        Validate(latitude, longitude, altitude);
        foreach (var item in items)
        {
            item.PendingChanges.RemoveLocation = false;
            item.PendingChanges.Latitude = latitude;
            item.PendingChanges.Longitude = longitude;
            item.PendingChanges.Altitude = altitude;
            item.PendingChanges.RemoveAltitude = !altitude.HasValue;
        }
    }

    public void RemoveLocation(IEnumerable<PhotoItem> items)
    {
        foreach (var item in items)
        {
            item.PendingChanges.RemoveLocation = true;
            item.PendingChanges.Latitude = null;
            item.PendingChanges.Longitude = null;
            item.PendingChanges.Altitude = null;
            item.PendingChanges.RemoveAltitude = false;
        }
    }

    public static GpsCoordinate CopyLocation(PhotoItem item)
    {
        if (item.EffectiveLatitude is not double latitude || item.EffectiveLongitude is not double longitude)
            throw new InvalidOperationException("The selected item has no GPS coordinates to copy.");
        return new GpsCoordinate(latitude, longitude, item.EffectiveAltitude);
    }

    public static void Validate(double latitude, double longitude, double? altitude = null)
    {
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(latitude), "GPS coordinates are outside their valid ranges.");
        if (altitude is < -12000 or > 100000)
            throw new ArgumentOutOfRangeException(nameof(altitude), "GPS altitude is outside a realistic range.");
    }
}
