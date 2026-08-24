using System.Collections.Concurrent;
using ExifTweaker.Infrastructure;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed class ImmichUploadService
{
    private readonly IImmichClient _client;
    public string? LastResolvedAlbumId { get; private set; }


    public ImmichUploadService(IImmichClient client) => _client = client;

    public async Task<ImmichUploadResult> UploadAsync(
        ImmichUploadRequest request,
        IProgress<ImmichUploadProgress>? progress,
        CancellationToken ct)
    {
        var albumId = request.AlbumId;
        if (!string.IsNullOrWhiteSpace(request.NewAlbumName))
            albumId = (await _client.CreateAlbumAsync(request.NewAlbumName, ct)).Id;
        LastResolvedAlbumId = albumId;

        var results = new ConcurrentDictionary<string, ImmichUploadItemResult>(StringComparer.OrdinalIgnoreCase);
        var completed = 0;
        using var gate = new SemaphoreSlim(Math.Clamp(request.Concurrency, 1, 8));
        var tasks = request.FilePaths.Select(async path =>
        {
            try
            {
                await gate.WaitAsync(ct);
                try
                {
                    progress?.Report(new ImmichUploadProgress(path, ImmichUploadStatus.Uploading, Volatile.Read(ref completed), request.FilePaths.Count));
                    var upload = await UploadWithRetryAsync(path, request.Visibility, ct);
                    var status = upload.Duplicate ? ImmichUploadStatus.Duplicate : ImmichUploadStatus.Uploaded;
                    results[path] = new ImmichUploadItemResult(path, status, upload.AssetId);
                    progress?.Report(new ImmichUploadProgress(path, status, Interlocked.Increment(ref completed), request.FilePaths.Count));
                }
                finally { gate.Release(); }
            }
            catch (OperationCanceledException)
            {
                results[path] = new ImmichUploadItemResult(path, ImmichUploadStatus.Cancelled);
                progress?.Report(new ImmichUploadProgress(path, ImmichUploadStatus.Cancelled, Interlocked.Increment(ref completed), request.FilePaths.Count));
            }
            catch (Exception ex)
            {
                AppLogger.Error($"Immich upload failed for {path}.", ex);
                results[path] = new ImmichUploadItemResult(path, ImmichUploadStatus.Failed, Error: ex.Message);
                progress?.Report(new ImmichUploadProgress(path, ImmichUploadStatus.Failed, Interlocked.Increment(ref completed), request.FilePaths.Count, ex.Message));
            }
        }).ToArray();

        await Task.WhenAll(tasks);

        if (!string.IsNullOrWhiteSpace(albumId))
        {
            var assetIds = results.Values
                .Where(result => result.Status is ImmichUploadStatus.Uploaded or ImmichUploadStatus.Duplicate)
                .Select(result => result.AssetId!)
                .Distinct()
                .ToList();
            if (assetIds.Count > 0 && !ct.IsCancellationRequested)
            {
                try { await _client.AddAssetsToAlbumAsync(albumId, assetIds, ct); }
                catch (Exception ex)
                {
                    AppLogger.Error("Unable to add uploaded assets to the Immich album.", ex);
                    foreach (var result in results.Values.Where(result => result.AssetId is not null).ToList())
                        results[result.FilePath] = result with { Status = ImmichUploadStatus.Failed, Error = $"Image envoyée, mais ajout à l’album impossible : {ex.Message}" };
                }
            }
        }

        return new ImmichUploadResult(request.FilePaths.Select(path => results.TryGetValue(path, out var result)
            ? result
            : new ImmichUploadItemResult(path, ImmichUploadStatus.Cancelled)).ToList());
    }

    private async Task<(string AssetId, bool Duplicate)> UploadWithRetryAsync(string path, ImmichAssetVisibility visibility, CancellationToken ct)
    {
        for (var attempt = 1; ; attempt++)
        {
            try { return await _client.UploadAssetAsync(path, visibility, ct); }
            catch (HttpRequestException) when (attempt < 3)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(400 * attempt), ct);
            }
            catch (ImmichApiException ex) when (attempt < 3 && ex.StatusCode.HasValue && (int)ex.StatusCode.Value >= 500)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(400 * attempt), ct);
            }
        }
    }
}
