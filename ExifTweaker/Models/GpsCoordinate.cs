namespace ExifTweaker.Models;

public sealed record GpsCoordinate(double Latitude, double Longitude, double? Altitude = null);
