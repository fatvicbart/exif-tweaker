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
            try
            {
                var point = JsonSerializer.Deserialize<MapPoint>(args.TryGetWebMessageAsString());
                if (point is not null) MapLocationChanged?.Invoke(this, new MapLocationChangedEventArgs(point.Latitude, point.Longitude));
            }
            catch (JsonException) { }
        };
    }

    public async Task InitializeAsync(string tileUrl, string attribution, bool darkTheme = false)
    {
        await browser.EnsureCoreWebView2Async();
        browser.DefaultBackgroundColor = darkTheme ? Color.FromArgb(30, 30, 30) : Color.White;
        browser.NavigateToString(BuildHtml(tileUrl, attribution, darkTheme));
    }

    public Task SetMarkerAsync(double latitude, double longitude) =>
        SetMarkersAsync(new[] { new MapMarker(latitude, longitude, string.Empty, true) }, 0);

    public async Task SetMarkersAsync(IReadOnlyList<MapMarker> markers, int missingGpsCount)
    {
        if (browser.CoreWebView2 is null) return;
        var payload = JsonSerializer.Serialize(markers);
        await browser.ExecuteScriptAsync($"setMarkers({payload},{missingGpsCount});");
    }

    private static string BuildHtml(string tileUrl, string attribution, bool darkTheme) =>
        Html.Replace("__TILE_URL__", JsonSerializer.Serialize(tileUrl), StringComparison.Ordinal)
            .Replace("__ATTRIBUTION__", JsonSerializer.Serialize(attribution), StringComparison.Ordinal)
            .Replace("__THEME__", darkTheme ? "dark" : "light", StringComparison.Ordinal);

    private sealed record MapPoint(double Latitude, double Longitude);
    private const string Html = """
<!doctype html><html><head><meta charset="utf-8"><link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"><script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script><style>html,body,#map{height:100%;margin:0}.active{filter:hue-rotate(135deg) saturate(1.5)}#missing{position:absolute;z-index:1000;right:8px;top:8px;background:#fff;padding:5px 8px;border-radius:4px;box-shadow:0 1px 5px #777;font:12px sans-serif}.dark{background:#1e1e1e}.dark .leaflet-tile-pane{filter:brightness(.65) invert(1) contrast(.9) hue-rotate(180deg)}.dark .leaflet-control,.dark .leaflet-control a,.dark .leaflet-tooltip,.dark #missing{background:#2d2d30;color:#f1f1f1;border-color:#4b4b4e}.dark .leaflet-control-attribution a{color:#7eb7e6}</style></head><body class="__THEME__"><div id="map"></div><div id="missing"></div><script>const map=L.map("map").setView([48.8566,2.3522],5);L.tileLayer(__TILE_URL__,{attribution:__ATTRIBUTION__}).addTo(map);let layer=L.layerGroup().addTo(map);function setMarkers(points,missing){layer.clearLayers();document.getElementById("missing").textContent=missing?missing+" media without GPS":"";const bounds=[];for(const p of points){const m=L.marker([p.Latitude,p.Longitude],{title:p.Label||""}).addTo(layer);if(p.Label)m.bindTooltip(p.Label);if(p.IsActive&&m._icon)m._icon.classList.add("active");bounds.push([p.Latitude,p.Longitude]);}if(bounds.length===1)map.setView(bounds[0],13);else if(bounds.length>1)map.fitBounds(bounds,{padding:[24,24]});}function setMarker(a,b){setMarkers([{Latitude:a,Longitude:b,Label:"",IsActive:true}],0);}map.on("click",e=>{setMarker(e.latlng.lat,e.latlng.lng);window.chrome.webview.postMessage(JSON.stringify({Latitude:e.latlng.lat,Longitude:e.latlng.lng}))});</script></body></html>
""";
}
