using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using ExifTweaker.Infrastructure;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public interface IGeocodingService
{
    Task<IReadOnlyList<Coordinates>> SearchAsync(string location, CancellationToken ct = default);
    Task<Coordinates?> ReverseAsync(double latitude, double longitude, CancellationToken ct = default);
}

public sealed class GeocodingService : IGeocodingService, IDisposable
{
    private readonly ConcurrentDictionary<string, IReadOnlyList<Coordinates>> _searchCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<string, Coordinates> _reverseCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly HttpClient _http = new();
    private readonly AppSettings _settings;

    public GeocodingService(AppSettings settings)
    {
        _settings = settings;
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("ExifTweaker/2.0");
    }

    public async Task<IReadOnlyList<Coordinates>> SearchAsync(string location, CancellationToken ct = default)
    {
        var cacheKey = location.Trim();
        if (string.IsNullOrWhiteSpace(cacheKey)) return Array.Empty<Coordinates>();
        if (_searchCache.TryGetValue(cacheKey, out var cached)) return cached;

        var uri = BuildSearchUri(cacheKey);
        using var response = await _http.GetAsync(uri, ct);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        var results = new List<Coordinates>();
        foreach (var item in doc.RootElement.EnumerateArray())
        {
            if (!TryGetCoordinate(item, "lat", out var lat) || !TryGetCoordinate(item, "lon", out var lon)) continue;
            var name = item.TryGetProperty("display_name", out var dn) ? dn.GetString() ?? string.Empty : string.Empty;
            var type = item.TryGetProperty("type", out var tp) ? tp.GetString() ?? string.Empty : string.Empty;
            results.Add(new Coordinates(lat, lon, name, type));
        }

        _searchCache[cacheKey] = results;
        return results;
    }

    public async Task<Coordinates?> ReverseAsync(double latitude, double longitude, CancellationToken ct = default)
    {
        LocationEditorService.Validate(latitude, longitude);
        var cacheKey = $"{latitude.ToString("F6", CultureInfo.InvariantCulture)},{longitude.ToString("F6", CultureInfo.InvariantCulture)}";
        if (_reverseCache.TryGetValue(cacheKey, out var cached)) return cached;

        var uri = BuildReverseUri(latitude, longitude);
        using var response = await _http.GetAsync(uri, ct);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        if (doc.RootElement.ValueKind != JsonValueKind.Object) return null;

        var name = doc.RootElement.TryGetProperty("display_name", out var dn) ? dn.GetString() ?? string.Empty : string.Empty;
        if (string.IsNullOrWhiteSpace(name)) return null;
        var type = doc.RootElement.TryGetProperty("type", out var tp) ? tp.GetString() ?? string.Empty : "reverse";
        var result = new Coordinates(latitude, longitude, name, type);
        _reverseCache[cacheKey] = result;
        return result;
    }

    private string BuildSearchUri(string query)
    {
        if (_settings.GeocodingProvider.Equals("Nominatim", StringComparison.OrdinalIgnoreCase))
            return $"https://nominatim.openstreetmap.org/search?format=jsonv2&limit=10&q={Uri.EscapeDataString(query)}";
        EnsureMapsCoKey();
        return $"https://geocode.maps.co/search?q={Uri.EscapeDataString(query)}&api_key={Uri.EscapeDataString(_settings.MapsCoApiKey!)}";
    }

    private string BuildReverseUri(double latitude, double longitude)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);
        if (_settings.GeocodingProvider.Equals("Nominatim", StringComparison.OrdinalIgnoreCase))
            return $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat}&lon={lon}";
        EnsureMapsCoKey();
        return $"https://geocode.maps.co/reverse?lat={lat}&lon={lon}&api_key={Uri.EscapeDataString(_settings.MapsCoApiKey!)}";
    }

    private void EnsureMapsCoKey()
    {
        if (string.IsNullOrWhiteSpace(_settings.MapsCoApiKey))
            throw new InvalidOperationException("No Maps.co API key configured. Open Settings or set EXIFTWEAKER_MAPSCO_API_KEY.");
    }

    public void Dispose() => _http.Dispose();

    private static bool TryGetCoordinate(JsonElement item, string propertyName, out double value)
    {
        value = default;
        return item.TryGetProperty(propertyName, out var property) &&
               double.TryParse(property.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }
}
