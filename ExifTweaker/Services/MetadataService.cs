using ExifTweaker.Infrastructure;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed record MetadataApplyFileResult(string FilePath, bool Succeeded, string? Error);
public sealed record MetadataApplyResult(IReadOnlyList<MetadataApplyFileResult> Files)
{
    public int SucceededCount => Files.Count(file => file.Succeeded);
    public int FailedCount => Files.Count - SucceededCount;
}

public sealed class MetadataService
{
    private readonly ExifToolService _exifTool;
    private readonly AppSettings _settings;

    public MetadataService(ExifToolService exifTool, AppSettings settings)
    {
        _exifTool = exifTool;
        _settings = settings;
    }

    public async Task<IReadOnlyList<PhotoItem>> LoadAsync(IEnumerable<string> paths, IProgress<int>? progress = null, CancellationToken ct = default)
    {
        var files = paths.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var metadata = await _exifTool.ReadAsync(files, ct);
        var items = new List<PhotoItem>(files.Count);
        for (var index = 0; index < files.Count; index++)
        {
            ct.ThrowIfCancellationRequested();
            var item = new PhotoItem(files[index]);
            if (metadata.TryGetValue(item.FilePath, out var original)) item.Original = original;
            else item.Error = "Metadata not returned by ExifTool";
            items.Add(item);
            progress?.Report(files.Count == 0 ? 100 : (int)(100d * (index + 1) / files.Count));
        }
        return items;
    }

    public async Task<MetadataApplyResult> ApplyPendingChangesAsync(IEnumerable<PhotoItem> items, IProgress<int>? progress = null, CancellationToken ct = default)
    {
        var changed = items.Where(item => item.PendingChanges.HasChanges).ToList();
        var results = new List<MetadataApplyFileResult>(changed.Count);
        var backupOriginal = _settings.BackupStrategy == BackupStrategy.ExifToolOriginal;
        for (var index = 0; index < changed.Count; index++)
        {
            ct.ThrowIfCancellationRequested();
            var item = changed[index];
            try
            {
                await _exifTool.WriteAsync(item, backupOriginal, ct);
                var readBack = await _exifTool.ReadAsync(new[] { item.FilePath }, ct);
                if (!readBack.TryGetValue(item.FilePath, out var refreshed))
                    throw new InvalidOperationException("ExifTool did not return metadata after writing.");
                item.Original = refreshed;
                item.PendingChanges.Clear();
                item.Error = null;
                item.NotifyChanged();
                results.Add(new MetadataApplyFileResult(item.FilePath, true, null));
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                item.Error = ex.Message;
                AppLogger.Error($"Unable to apply metadata to {item.FilePath}.", ex);
                results.Add(new MetadataApplyFileResult(item.FilePath, false, ex.Message));
            }
            progress?.Report(changed.Count == 0 ? 100 : (int)(100d * (index + 1) / changed.Count));
        }
        return new MetadataApplyResult(results);
    }
}
