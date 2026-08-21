using System.Text.Json;

namespace ExifTweaker.Controls;

public sealed class MapLocationChangedEventArgs(double latitude, double longitude) : EventArgs
{
    public double Latitude { get; } = latitude;
    public double Longitude { get; } = longitude;
}

public sealed record MapMarker(double Latitude, double Longitude, string Label, bool IsActive);

public sealed partial class MapControl : UserControl
{
    public event EventHandler<MapLocationChangedEventArgs>? MapLocationChanged;

    public MapControl()
    {
        InitializeComponent();
        browser.WebMessageReceived += (_, args) =>
        {
            try { var point = JsonSerializer.Deserialize<MapPoint>(args.TryGetWebMessageAsString()); if (point is not null) MapLocationChanged?.Invoke(this, new MapLocationChangedEventArgs(point.Latitude, point.Longitude)); }
            catch (JsonException) { }
        };
    }

    public async Task InitializeAsync() { await browser.EnsureCoreWebView2Async(); browser.NavigateToString(Html); }

    public Task SetMarkerAsync(double latitude, double longitude) =>
        SetMarkersAsync(new[] { new MapMarker(latitude, longitude, string.Empty, true) });

    public async Task SetMarkersAsync(IReadOnlyList<MapMarker> markers)
    {
        if (browser.CoreWebView2 is null) return;
        var payload = JsonSerializer.Serialize(markers);
        await browser.ExecuteScriptAsync($"setMarkers({payload});");
    }

    private sealed record MapPoint(double Latitude, double Longitude);
    private const string Html = """
<!doctype html><html><head><meta charset="utf-8"><link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"><script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script><style>html,body,#map{height:100%;margin:0}.active{filter:hue-rotate(135deg) saturate(1.5)}</style></head><body><div id="map"></div><script>const map=L.map("map").setView([48.8566,2.3522],5);L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",{attribution:"OpenStreetMap"}).addTo(map);let layer=L.layerGroup().addTo(map);function setMarkers(points){layer.clearLayers();const bounds=[];for(const p of points){const m=L.marker([p.Latitude,p.Longitude],{title:p.Label||""}).addTo(layer);if(p.Label)m.bindTooltip(p.Label);if(p.IsActive&&m._icon)m._icon.classList.add("active");bounds.push([p.Latitude,p.Longitude]);}if(bounds.length===1)map.setView(bounds[0],13);else if(bounds.length>1)map.fitBounds(bounds,{padding:[24,24]});}function setMarker(a,b){setMarkers([{Latitude:a,Longitude:b,Label:"",IsActive:true}]);}map.on("click",e=>{setMarker(e.latlng.lat,e.latlng.lng);window.chrome.webview.postMessage(JSON.stringify({Latitude:e.latlng.lat,Longitude:e.latlng.lng}))});</script></body></html>
""";
}
