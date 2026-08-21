using ExifTweaker.Infrastructure;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed record MetadataApplyPreviewFile(string FilePath, string FileType, string OriginalDate, string EffectiveDate, string OriginalLocation, string EffectiveLocation, bool BackupAvailable);
public sealed record MetadataApplyPreview(int FileCount, int DateChanges, int LocationChanges, int LocationRemovals, int OffsetChanges, bool BackupOriginals, IReadOnlyList<MetadataApplyPreviewFile> Files)
{
    public string FileTypeSummary => string.Join(", ", Files.GroupBy(file => string.IsNullOrWhiteSpace(file.FileType) ? "Unknown" : file.FileType).OrderByDescending(group => group.Count()).ThenBy(group => group.Key).Select(group => $"{group.Key} {group.Count()}"));
}
public sealed record MetadataApplyFileResult(string FilePath, bool Succeeded, string? Error, string? FileType = null, bool BackupAvailable = false);
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

    public MetadataApplyPreview Preview(IEnumerable<PhotoItem> items)
    {
        var changed = items.Where(item => item.PendingChanges.HasChanges).ToList();
        var files = changed.Select(item => new MetadataApplyPreviewFile(
            item.FilePath,
            item.Original.FileType ?? Path.GetExtension(item.FilePath).TrimStart('.').ToUpperInvariant(),
            FormatDate(item.Original.CaptureDate),
            FormatDate(item.EffectiveCaptureDate),
            FormatLocation(item.Original.Latitude, item.Original.Longitude, item.Original.Altitude),
            FormatLocation(item.EffectiveLatitude, item.EffectiveLongitude, item.EffectiveAltitude),
            File.Exists(item.FilePath + "_original"))).ToList();
        return new MetadataApplyPreview(
            changed.Count,
            changed.Count(item => item.PendingChanges.HasDateChange),
            changed.Count(item => item.PendingChanges.HasLocationChange),
            changed.Count(item => item.PendingChanges.RemoveLocation),
            changed.Count(item => item.PendingChanges.HasOffsetChange),
            _settings.BackupStrategy == BackupStrategy.ExifToolOriginal,
            files);
    }

    public async Task<MetadataApplyResult> RestoreBackupsAsync(IEnumerable<PhotoItem> items, CancellationToken ct = default)
    {
        var results = new List<MetadataApplyFileResult>();
        foreach (var item in items)
        {
            try { await RestoreBackupAsync(item, ct); results.Add(new MetadataApplyFileResult(item.FilePath, true, null, item.Original.FileType, File.Exists(item.FilePath + "_original"))); }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex) { item.Error = ex.Message; AppLogger.Error($"Unable to restore {item.FilePath}.", ex); results.Add(new MetadataApplyFileResult(item.FilePath, false, ex.Message, item.Original.FileType, File.Exists(item.FilePath + "_original"))); }
        }
        return new MetadataApplyResult(results);
    }

    public async Task RestoreBackupAsync(PhotoItem item, CancellationToken ct = default)
    {
        await _exifTool.RestoreBackupAsync(item, ct);
        var readBack = await _exifTool.ReadAsync(new[] { item.FilePath }, ct);
        if (!readBack.TryGetValue(item.FilePath, out var restored)) throw new InvalidOperationException("ExifTool did not return metadata after restore.");
        item.Original = restored; item.PendingChanges.Clear(); item.Error = null; item.NotifyChanged();
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
                results.Add(new MetadataApplyFileResult(item.FilePath, true, null, item.Original.FileType, File.Exists(item.FilePath + "_original")));
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                item.Error = ex.Message;
                AppLogger.Error($"Unable to apply metadata to {item.FilePath}.", ex);
                results.Add(new MetadataApplyFileResult(item.FilePath, false, ex.Message, item.Original.FileType, File.Exists(item.FilePath + "_original")));
            }
            progress?.Report(changed.Count == 0 ? 100 : (int)(100d * (index + 1) / changed.Count));
        }
        return new MetadataApplyResult(results);
    }
    private static string FormatDate(DateTime? value) => value?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;

    private static string FormatLocation(double? latitude, double? longitude, double? altitude)
    {
        if (!latitude.HasValue || !longitude.HasValue) return string.Empty;
        var gps = $"{latitude.Value:F6}, {longitude.Value:F6}";
        return altitude.HasValue ? $"{gps}, {altitude.Value:F2} m" : gps;
    }
}
