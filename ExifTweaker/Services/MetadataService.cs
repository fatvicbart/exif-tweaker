using ExifTweaker.Infrastructure;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed record MetadataApplyPreviewFile(string FilePath, string FileType, string OriginalDate, string EffectiveDate, string OriginalLocation, string EffectiveLocation, bool BackupAvailable);
public sealed record MetadataApplyPreview(int FileCount, int DateChanges, int LocationChanges, int LocationRemovals, int OffsetChanges, bool BackupOriginals, IReadOnlyList<MetadataApplyPreviewFile> Files)
{
    public string FileTypeSummary => string.Join(", ", Files.GroupBy(file => string.IsNullOrWhiteSpace(file.FileType) ? "Unknown" : file.FileType).OrderByDescending(group => group.Count()).ThenBy(group => group.Key).Select(group => $"{group.Key} {group.Count()}"));
}
public sealed record MetadataApplyFileResult(string FilePath, bool Succeeded, string? Error, string? FileType = null, bool BackupAvailable = false, bool Cancelled = false, string? Warning = null);
public sealed record MetadataApplyResult(IReadOnlyList<MetadataApplyFileResult> Files)
{
    public int SucceededCount => Files.Count(file => file.Succeeded);
    public int CancelledCount => Files.Count(file => file.Cancelled);
    public int FailedCount => Files.Count(file => !file.Succeeded && !file.Cancelled);
    public bool WasCancelled => CancelledCount > 0;
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
        var pending = items.ToList();
        var results = new List<MetadataApplyFileResult>(pending.Count);
        for (var index = 0; index < pending.Count; index++)
        {
            var item = pending[index];
            if (ct.IsCancellationRequested)
            {
                results.AddRange(pending.Skip(index).Select(cancelled => new MetadataApplyFileResult(cancelled.FilePath, false, null, cancelled.Original.FileType, File.Exists(cancelled.FilePath + "_original"), Cancelled: true)));
                break;
            }
            try
            {
                await RestoreBackupAsync(item, ct);
                results.Add(new MetadataApplyFileResult(item.FilePath, true, null, item.Original.FileType, File.Exists(item.FilePath + "_original")));
            }
            catch (OperationCanceledException)
            {
                results.Add(new MetadataApplyFileResult(item.FilePath, false, null, item.Original.FileType, File.Exists(item.FilePath + "_original"), Cancelled: true));
                results.AddRange(pending.Skip(index + 1).Select(cancelled => new MetadataApplyFileResult(cancelled.FilePath, false, null, cancelled.Original.FileType, File.Exists(cancelled.FilePath + "_original"), Cancelled: true)));
                break;
            }
            catch (Exception ex)
            {
                item.Error = ex.Message;
                AppLogger.Error($"Unable to restore {item.FilePath}.", ex);
                results.Add(new MetadataApplyFileResult(item.FilePath, false, ex.Message, item.Original.FileType, File.Exists(item.FilePath + "_original")));
            }
        }
        return new MetadataApplyResult(results);
    }

    public async Task RestoreBackupAsync(PhotoItem item, CancellationToken ct = default)
    {
        await _exifTool.RestoreBackupAsync(item, ct);
        var readBack = await _exifTool.ReadAsync(new[] { item.FilePath }, CancellationToken.None);
        if (!readBack.TryGetValue(item.FilePath, out var restored)) throw new InvalidOperationException("ExifTool did not return metadata after restore.");
        item.Original = restored; item.PendingChanges.Clear(); item.Error = null; item.NotifyChanged();
    }

    public async Task<MetadataApplyResult> ApplyPendingChangesAsync(IEnumerable<PhotoItem> items, IProgress<int>? progress = null, CancellationToken ct = default)
    {
        var changed = items.Where(item => item.PendingChanges.HasChanges).ToList();
        var results = new MetadataApplyFileResult?[changed.Count];
        var backupOriginal = _settings.BackupStrategy == BackupStrategy.ExifToolOriginal;
        using var gate = new SemaphoreSlim(Math.Clamp(_settings.MaxParallelism, 1, 16));
        var completed = 0;

        async Task ApplyOneAsync(PhotoItem item, int index)
        {
            var entered = false;
            try
            {
                await gate.WaitAsync(ct);
                entered = true;
                var expected = item.EffectiveMetadata;
                var patch = item.PendingChanges.Clone();
                var warning = await _exifTool.WriteAsync(item, backupOriginal, ct);
                var readBack = await _exifTool.ReadAsync(new[] { item.FilePath }, CancellationToken.None);
                if (!readBack.TryGetValue(item.FilePath, out var refreshed))
                    throw new InvalidOperationException("ExifTool did not return metadata after writing.");
                if (!VerifyCriticalMetadata(item.FilePath, expected, patch, refreshed, out var verificationError))
                    throw new InvalidOperationException($"Read-back verification failed: {verificationError}");

                item.Original = refreshed;
                item.PendingChanges.Clear();
                item.Error = null;
                item.NotifyChanged();
                results[index] = new MetadataApplyFileResult(item.FilePath, true, null, refreshed.FileType, File.Exists(item.FilePath + "_original"), Warning: warning);
            }
            catch (OperationCanceledException)
            {
                results[index] = new MetadataApplyFileResult(item.FilePath, false, null, item.Original.FileType, File.Exists(item.FilePath + "_original"), Cancelled: true);
            }
            catch (Exception ex)
            {
                item.Error = ex.Message;
                AppLogger.Error($"Unable to apply metadata to {item.FilePath}.", ex);
                results[index] = new MetadataApplyFileResult(item.FilePath, false, ex.Message, item.Original.FileType, File.Exists(item.FilePath + "_original"));
            }
            finally
            {
                if (entered) gate.Release();
                var done = Interlocked.Increment(ref completed);
                progress?.Report(changed.Count == 0 ? 100 : (int)(100d * done / changed.Count));
            }
        }

        await Task.WhenAll(changed.Select(ApplyOneAsync));
        for (var index = 0; index < results.Length; index++)
            results[index] ??= new MetadataApplyFileResult(changed[index].FilePath, false, null, changed[index].Original.FileType, File.Exists(changed[index].FilePath + "_original"), Cancelled: true);
        return new MetadataApplyResult(results.Select(result => result!).ToList());
    }

    internal static bool VerifyCriticalMetadata(string filePath, PhotoMetadata expected, MetadataPatch patch, PhotoMetadata actual, out string error)
    {
        var problems = new List<string>();
        if (patch.HasDateChange && expected.CaptureDate is DateTime expectedDate)
        {
            if (IsVideo(filePath) && expected.Offset is TimeSpan offset)
                expectedDate = new DateTimeOffset(DateTime.SpecifyKind(expectedDate, DateTimeKind.Unspecified), offset).UtcDateTime;
            if (!actual.CaptureDate.HasValue || Math.Abs((actual.CaptureDate.Value - expectedDate).TotalSeconds) > 1)
                problems.Add($"capture date expected {expectedDate:O}, read {actual.CaptureDate:O}");
        }
        if (patch.HasOffsetChange && actual.Offset != expected.Offset)
            problems.Add($"offset expected {expected.Offset}, read {actual.Offset}");
        if (patch.RemoveLocation)
        {
            if (actual.Latitude.HasValue || actual.Longitude.HasValue) problems.Add("GPS removal was not confirmed");
        }
        else if (patch.HasLocationChange)
        {
            if (!Near(expected.Latitude, actual.Latitude, 0.00001) || !Near(expected.Longitude, actual.Longitude, 0.00001))
                problems.Add("GPS coordinates differ from requested values");
            if ((patch.Altitude.HasValue || patch.RemoveAltitude) && !Near(expected.Altitude, actual.Altitude, 0.5))
                problems.Add("GPS altitude differs from requested value");
        }
        error = string.Join("; ", problems);
        return problems.Count == 0;
    }

    private static bool Near(double? expected, double? actual, double tolerance) =>
        !expected.HasValue && !actual.HasValue || expected.HasValue && actual.HasValue && Math.Abs(expected.Value - actual.Value) <= tolerance;

    private static bool IsVideo(string path) =>
        Path.GetExtension(path).Equals(".mov", StringComparison.OrdinalIgnoreCase) ||
        Path.GetExtension(path).Equals(".mp4", StringComparison.OrdinalIgnoreCase);

    private static string FormatDate(DateTime? value) => value?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;

    private static string FormatLocation(double? latitude, double? longitude, double? altitude)
    {
        if (!latitude.HasValue || !longitude.HasValue) return string.Empty;
        var gps = $"{latitude.Value:F6}, {longitude.Value:F6}";
        return altitude.HasValue ? $"{gps}, {altitude.Value:F2} m" : gps;
    }
}
