using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public interface IImmichClient : IDisposable
{
    Task<ImmichServerInfo> GetServerInfoAsync(CancellationToken ct);
    Task<IReadOnlyList<ImmichAlbum>> GetAlbumsAsync(CancellationToken ct);
    Task<ImmichAlbum> CreateAlbumAsync(string name, CancellationToken ct);
    Task<(string AssetId, bool Duplicate)> UploadAssetAsync(string filePath, ImmichAssetVisibility visibility, CancellationToken ct);
    Task AddAssetsToAlbumAsync(string albumId, IReadOnlyList<string> assetIds, CancellationToken ct);
}

public sealed class ImmichClient : IImmichClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly HttpClient _http;

    public ImmichClient(string serverUrl, string apiKey, HttpMessageHandler? handler = null)
    {
        _http = handler is null ? new HttpClient() : new HttpClient(handler, disposeHandler: true);
        _http.BaseAddress = new Uri(NormalizeServerUrl(serverUrl), UriKind.Absolute);
        _http.Timeout = TimeSpan.FromMinutes(30);
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _http.DefaultRequestHeaders.Add("x-api-key", apiKey);
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("ExifTweaker/2.2");
    }

    public static string NormalizeServerUrl(string value)
    {
        if (!Uri.TryCreate(value.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("L’adresse Immich doit être une URL HTTP ou HTTPS valide.", nameof(value));
        var builder = new UriBuilder(uri) { Query = string.Empty, Fragment = string.Empty };
        var path = builder.Path.TrimEnd('/');
        if (!path.EndsWith("/api", StringComparison.OrdinalIgnoreCase)) path += "/api";
        builder.Path = path + "/";
        return builder.Uri.AbsoluteUri;
    }

    public async Task<ImmichServerInfo> GetServerInfoAsync(CancellationToken ct)
    {
        using var response = await _http.GetAsync("server/about", ct);
        var dto = JsonSerializer.Deserialize<ServerDto>(await ReadSuccessAsync(response, ct), JsonOptions);
        return new ImmichServerInfo(dto?.Name ?? "Immich", dto?.Version ?? "version inconnue");
    }

    public async Task<IReadOnlyList<ImmichAlbum>> GetAlbumsAsync(CancellationToken ct)
    {
        using var response = await _http.GetAsync("albums?isOwned=true", ct);
        var json = await ReadSuccessAsync(response, ct);
        return (JsonSerializer.Deserialize<List<AlbumDto>>(json, JsonOptions) ?? [])
            .Where(album => !string.IsNullOrWhiteSpace(album.Id) && !string.IsNullOrWhiteSpace(album.AlbumName))
            .Select(album => new ImmichAlbum(album.Id!, album.AlbumName!))
            .OrderBy(album => album.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }

    public async Task<ImmichAlbum> CreateAlbumAsync(string name, CancellationToken ct)
    {
        using var content = JsonContent(new { albumName = name.Trim() });
        using var response = await _http.PostAsync("albums", content, ct);
        var dto = JsonSerializer.Deserialize<AlbumDto>(await ReadSuccessAsync(response, ct), JsonOptions)
            ?? throw new ImmichApiException("Immich a renvoyé un album invalide.");
        return new ImmichAlbum(dto.Id ?? throw new ImmichApiException("Identifiant d’album absent."), dto.AlbumName ?? name.Trim());
    }

    public async Task<(string AssetId, bool Duplicate)> UploadAssetAsync(string filePath, ImmichAssetVisibility visibility, CancellationToken ct)
    {
        var info = new FileInfo(filePath);
        if (!info.Exists) throw new FileNotFoundException("Le fichier n’existe plus.", filePath);

        await using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 128 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan);
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(info.LastWriteTimeUtc.ToString("O")), "fileCreatedAt");
        form.Add(new StringContent(info.LastWriteTimeUtc.ToString("O")), "fileModifiedAt");
        form.Add(new StringContent(info.Length.ToString(System.Globalization.CultureInfo.InvariantCulture)), "fileSize");
        form.Add(new StringContent("false"), "isFavorite");
        form.Add(new StringContent(ToApiVisibility(visibility)), "visibility");
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        form.Add(fileContent, "assetData", info.Name);

        using var response = await _http.PostAsync("assets", form, ct);
        var dto = JsonSerializer.Deserialize<UploadDto>(await ReadSuccessAsync(response, ct), JsonOptions)
            ?? throw new ImmichApiException("Immich a renvoyé une réponse d’upload invalide.");
        if (string.IsNullOrWhiteSpace(dto.Id)) throw new ImmichApiException("Immich n’a pas renvoyé l’identifiant de l’image.");
        return (dto.Id, string.Equals(dto.Status, "duplicate", StringComparison.OrdinalIgnoreCase) || response.StatusCode == HttpStatusCode.OK);
    }

    public async Task AddAssetsToAlbumAsync(string albumId, IReadOnlyList<string> assetIds, CancellationToken ct)
    {
        foreach (var batch in assetIds.Chunk(1000))
        {
            using var content = JsonContent(new { ids = batch });
            using var response = await _http.PutAsync($"albums/{Uri.EscapeDataString(albumId)}/assets", content, ct);
            await ReadSuccessAsync(response, ct);
        }
    }

    private static StringContent JsonContent(object value) =>
        new(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");

    private static async Task<string> ReadSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadAsStringAsync(ct);
        if (response.IsSuccessStatusCode) return body;
        var correlation = response.Headers.TryGetValues("X-Correlation-ID", out var values) ? values.FirstOrDefault() : null;
        var suffix = string.IsNullOrWhiteSpace(correlation) ? string.Empty : $" (référence {correlation})";
        throw new ImmichApiException($"Immich a répondu {(int)response.StatusCode} {response.ReasonPhrase}{suffix} : {ExtractError(body)}", response.StatusCode);
    }

    private static string ExtractError(string body)
    {
        if (string.IsNullOrWhiteSpace(body)) return "aucun détail";
        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("message", out var message)) return message.ToString();
        }
        catch (JsonException) { }
        return body.Length <= 500 ? body : body[..500] + "…";
    }

    private static string ToApiVisibility(ImmichAssetVisibility value) => value switch
    {
        ImmichAssetVisibility.Archive => "archive",
        ImmichAssetVisibility.Hidden => "hidden",
        ImmichAssetVisibility.Locked => "locked",
        _ => "timeline"
    };

    public void Dispose() => _http.Dispose();

    private sealed record ServerDto(string? Name, string? Version);
    private sealed record AlbumDto(string? Id, string? AlbumName);
    private sealed record UploadDto(string Id, string? Status);
}

public sealed class ImmichApiException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public ImmichApiException(string message, HttpStatusCode? statusCode = null) : base(message) => StatusCode = statusCode;
}
