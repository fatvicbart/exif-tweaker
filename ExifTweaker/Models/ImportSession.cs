using System.ComponentModel;

namespace ExifTweaker.Models;

public sealed record ImportSessionStatistics(
    int MediaCount,
    int FilesWithCaptureDate,
    int FilesWithGps,
    int FilesWithoutGps,
    int FilesWithErrors,
    int PendingChangeCount,
    DateTime? FirstCaptureDate,
    DateTime? LastCaptureDate,
    IReadOnlyDictionary<string, int> Devices);

public sealed class ImportSession : INotifyPropertyChanged
{
    public ImportSession(DateTimeOffset? openedAt = null)
    {
        OpenedAt = openedAt ?? DateTimeOffset.Now;
        Media.ListChanged += (_, _) => OnPropertyChanged(string.Empty);
    }

    public DateTimeOffset OpenedAt { get; }
    public BindingList<PhotoItem> Media { get; } = new();
    public bool HasPendingChanges => Media.Any(item => item.HasPendingChanges);
    public int PendingChangeCount => Media.Count(item => item.HasPendingChanges);

    public ImportSessionStatistics Statistics
    {
        get
        {
            var dated = Media.Where(item => item.EffectiveCaptureDate.HasValue).ToList();
            DateTime? firstCaptureDate = dated.Count == 0 ? null : dated.Min(item => item.EffectiveCaptureDate!.Value);
            DateTime? lastCaptureDate = dated.Count == 0 ? null : dated.Max(item => item.EffectiveCaptureDate!.Value);

            var devices = Media
                .Select(item => string.Join(" ", new[] { item.Original.CameraMake, item.Original.CameraModel }.Where(value => !string.IsNullOrWhiteSpace(value))).Trim())
                .Where(device => !string.IsNullOrWhiteSpace(device))
                .GroupBy(device => device, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(group => group.Count())
                .ThenBy(group => group.Key, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

            return new ImportSessionStatistics(
                Media.Count,
                dated.Count,
                Media.Count(item => item.EffectiveLatitude.HasValue && item.EffectiveLongitude.HasValue),
                Media.Count(item => !item.EffectiveLatitude.HasValue || !item.EffectiveLongitude.HasValue),
                Media.Count(item => item.Error is not null),
                PendingChangeCount,
                firstCaptureDate,
                lastCaptureDate,
                devices);
        }
    }

    public void AddRange(IEnumerable<PhotoItem> items)
    {
        Media.RaiseListChangedEvents = false;
        try { foreach (var item in items) Media.Add(item); }
        finally
        {
            Media.RaiseListChangedEvents = true;
            Media.ResetBindings();
        }
        OnPropertyChanged(string.Empty);
    }

    public void Remove(PhotoItem item)
    {
        Media.Remove(item);
        OnPropertyChanged(string.Empty);
    }

    public void NotifyChanged() => OnPropertyChanged(string.Empty);

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged(string? propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
