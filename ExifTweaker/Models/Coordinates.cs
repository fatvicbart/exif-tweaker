namespace ExifTweaker.Models;
public sealed record Coordinates(double Latitude, double Longitude, string Name, string Type, double? Altitude = null);
