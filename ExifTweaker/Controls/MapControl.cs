using System.Text.Json;
using Microsoft.Web.WebView2.WinForms;

namespace ExifTweaker.Controls;

public sealed class MapLocationChangedEventArgs(double latitude, double longitude) : EventArgs
{
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;
}

public sealed class MapControl : UserControl
{
    private readonly WebView2 _browser = new() { Dock = DockStyle.Fill };
    public event EventHandler<MapLocationChangedEventArgs>? LocationChanged;

    public MapControl()
    {
        Controls.Add(_browser);
        _browser.WebMessageReceived += (_, args) =>
        {
            try { var point = JsonSerializer.Deserialize<MapPoint>(args.TryGetWebMessageAsString()); if (point is not null) LocationChanged?.Invoke(this, new MapLocationChangedEventArgs(point.Latitude, point.Longitude)); }
            catch (JsonException) { }
        };
    }

    public async Task InitializeAsync() { await _browser.EnsureCoreWebView2Async(); _browser.NavigateToString(Html); }
    public async Task SetMarkerAsync(double latitude, double longitude)
    {
        if (_browser.CoreWebView2 is not null) await _browser.ExecuteScriptAsync($"setMarker({latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)});");
    }

    private sealed record MapPoint(double Latitude, double Longitude);
    private const string Html = """
<!doctype html><html><head><meta charset="utf-8"><link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"><script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script><style>html,body,#map{height:100%;margin:0}</style></head><body><div id="map"></div><script>const map=L.map("map").setView([48.8566,2.3522],5);L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",{attribution:"OpenStreetMap"}).addTo(map);let marker;function setMarker(a,b){if(marker)marker.setLatLng([a,b]);else marker=L.marker([a,b]).addTo(map);map.setView([a,b],13)}map.on("click",e=>{setMarker(e.latlng.lat,e.latlng.lng);window.chrome.webview.postMessage(JSON.stringify({Latitude:e.latlng.lat,Longitude:e.latlng.lng}))});</script></body></html>
""";
}
