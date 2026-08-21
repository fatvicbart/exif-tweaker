using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExifTweaker.Models;

public sealed class PhotoItem : INotifyPropertyChanged
{
    private PhotoMetadata _original = new();
    private string? _error;

    public PhotoItem(string filePath) => FilePath = filePath;

    [Browsable(false)] public string FilePath { get; }
    [Browsable(false)] public PhotoMetadata Original { get => _original; set { _original = value; OnPropertyChanged(string.Empty); } }
    [Browsable(false)] public MetadataPatch PendingChanges { get; } = new();
    [Browsable(false)] public string? Error { get => _error; set { _error = value; OnPropertyChanged(); OnPropertyChanged(nameof(Status)); } }

    public string Name => Path.GetFileNameWithoutExtension(FilePath);
    public string FileName => Path.GetFileName(FilePath);
    public string Date => EffectiveCaptureDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
    public string City => Original.City ?? string.Empty;
    public string Country => Original.Country ?? string.Empty;
    public string Latitude => EffectiveLatitude?.ToString("F6") ?? string.Empty;
    public string Longitude => EffectiveLongitude?.ToString("F6") ?? string.Empty;
    public string Altitude => EffectiveAltitude?.ToString("F2") ?? string.Empty;
    public string Status => Error is not null ? "Error" : PendingChanges.HasChanges ? "Modified" : "OK";

    [Browsable(false)] public DateTime? EffectiveCaptureDate => PendingChanges.CaptureDate ??
        (Original.CaptureDate.HasValue && PendingChanges.DateShift.HasValue ? Original.CaptureDate.Value + PendingChanges.DateShift.Value : Original.CaptureDate);
    [Browsable(false)] public double? EffectiveLatitude => PendingChanges.RemoveLocation ? null : PendingChanges.Latitude ?? Original.Latitude;
    [Browsable(false)] public double? EffectiveLongitude => PendingChanges.RemoveLocation ? null : PendingChanges.Longitude ?? Original.Longitude;
    [Browsable(false)] public double? EffectiveAltitude => PendingChanges.RemoveLocation || PendingChanges.RemoveAltitude ? null : PendingChanges.Altitude ?? Original.Altitude;
    [Browsable(false)] public TimeSpan? EffectiveOffset => PendingChanges.RemoveOffsetTimeOriginal ? null : PendingChanges.OffsetTimeOriginal ?? Original.Offset;

    public void NotifyChanged() => OnPropertyChanged(string.Empty);
    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
