using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExifTweaker.Models;

public sealed class PhotoItem : INotifyPropertyChanged
{
    private PhotoMetadata _original = new();
    private string? _error;
    private bool _isSelected;

    public PhotoItem(string filePath) => FilePath = filePath;

    [Browsable(false)] public string FilePath { get; }
    [Browsable(false)] public PhotoMetadata Original { get => _original; set { _original = value; OnPropertyChanged(string.Empty); } }
    [Browsable(false)] public MetadataPatch PendingChanges { get; } = new();
    [Browsable(false)] public string? Error { get => _error; set { _error = value; OnPropertyChanged(); OnPropertyChanged(nameof(Status)); } }
    public bool IsSelected { get => _isSelected; set { if (_isSelected == value) return; _isSelected = value; OnPropertyChanged(); } }

    public string Name => Path.GetFileNameWithoutExtension(FilePath);
    public string FileName => Path.GetFileName(FilePath);
    public string Date => EffectiveCaptureDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
    public string Timezone => EffectiveOffset is TimeSpan offset ? FormatOffset(offset) : string.Empty;
    public string City => Original.City ?? string.Empty;
    public string Country => Original.Country ?? string.Empty;
    public string Latitude => EffectiveLatitude?.ToString("F6") ?? string.Empty;
    public string Longitude => EffectiveLongitude?.ToString("F6") ?? string.Empty;
    public string Altitude => EffectiveAltitude?.ToString("F2") ?? string.Empty;
    public string Location => EffectiveLatitude.HasValue && EffectiveLongitude.HasValue ? $"{Latitude}, {Longitude}" : string.Empty;
    public string Device => string.Join(" ", new[] { Original.CameraMake, Original.CameraModel }.Where(value => !string.IsNullOrWhiteSpace(value)));
    public string Dimensions => Original.Width.HasValue && Original.Height.HasValue ? $"{Original.Width}×{Original.Height}" : string.Empty;
    public string Status => Error is not null ? "Error" : PendingChanges.HasChanges ? "Modified" : !Original.CaptureDate.HasValue ? "Metadata issue" : "Unchanged";
    [Browsable(false)] public bool HasPendingChanges => PendingChanges.HasChanges;

    [Browsable(false)] public DateTime? EffectiveCaptureDate
    {
        get
        {
            var value = PendingChanges.CaptureDate ?? Original.CaptureDate;
            if (!value.HasValue) return null;
            value = value.Value.AddYears(PendingChanges.DateShiftYears).AddMonths(PendingChanges.DateShiftMonths);
            if (PendingChanges.DateShift.HasValue) value = value.Value.Add(PendingChanges.DateShift.Value);
            if (PendingChanges.ConvertToOffset && PendingChanges.OffsetTimeOriginal is TimeSpan target && Original.Offset is TimeSpan source)
                value = new DateTimeOffset(DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified), source).ToOffset(target).DateTime;
            return value;
        }
    }
    [Browsable(false)] public double? EffectiveLatitude => PendingChanges.RemoveLocation ? null : PendingChanges.Latitude ?? Original.Latitude;
    [Browsable(false)] public double? EffectiveLongitude => PendingChanges.RemoveLocation ? null : PendingChanges.Longitude ?? Original.Longitude;
    [Browsable(false)] public double? EffectiveAltitude => PendingChanges.RemoveLocation || PendingChanges.RemoveAltitude ? null : PendingChanges.Altitude ?? Original.Altitude;
    [Browsable(false)] public TimeSpan? EffectiveOffset => PendingChanges.RemoveOffsetTimeOriginal ? null : PendingChanges.OffsetTimeOriginal ?? Original.Offset;
    [Browsable(false)] public PhotoMetadata EffectiveMetadata => Original with
    {
        CaptureDate = EffectiveCaptureDate,
        Offset = EffectiveOffset,
        Latitude = EffectiveLatitude,
        Longitude = EffectiveLongitude,
        Altitude = EffectiveAltitude
    };

    public void NotifyChanged() => OnPropertyChanged(string.Empty);
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    private static string FormatOffset(TimeSpan offset) => $"{(offset < TimeSpan.Zero ? "-" : "+")}{Math.Abs((int)offset.TotalHours):00}:{Math.Abs(offset.Minutes):00}";
}
