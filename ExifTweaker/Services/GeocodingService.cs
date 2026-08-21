using System.Globalization;
using System.Text.Json;
using ExifTweaker.Infrastructure;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed class GeocodingService
{
    private readonly HttpClient _http = new();
    private readonly AppSettings _settings;
    public GeocodingService(AppSettings settings) { _settings = settings; _http.DefaultRequestHeaders.UserAgent.ParseAdd("ExifTweaker/2.0"); }

    public async Task<IReadOnlyList<Coordinates>> SearchAsync(string location, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.MapsCoApiKey))
            throw new InvalidOperationException("No geocoding API key configured. Set the EXIFTWEAKER_MAPSCO_API_KEY environment variable.");
        var uri = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(location)}&api_key={Uri.EscapeDataString(_settings.MapsCoApiKey)}";
        using var response = await _http.GetAsync(uri, ct); response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        var results = new List<Coordinates>();
        foreach (var item in doc.RootElement.EnumerateArray())
        {
            if (!double.TryParse(item.GetProperty("lat").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var lat) ||
                !double.TryParse(item.GetProperty("lon").GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var lon)) continue;
            var name = item.TryGetProperty("display_name", out var dn) ? dn.GetString() ?? string.Empty : string.Empty;
            var type = item.TryGetProperty("type", out var tp) ? tp.GetString() ?? string.Empty : string.Empty;
            results.Add(new Coordinates(lat, lon, name, type));
        }
        return results;
    }
}
