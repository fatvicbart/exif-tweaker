using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExifTweaker.Models;

public sealed class PhotoItem : INotifyPropertyChanged
{
    private PhotoMetadata _original = new();
    private string? _error;
    private string? _importNotice;
    private bool _isSelected;
    private string _resolvedLocation = string.Empty;
    private double? _resolvedLatitude;
    private double? _resolvedLongitude;

    public PhotoItem(string filePath) => FilePath = filePath;

    [Browsable(false)] public string FilePath { get; }
    [Browsable(false)] public PhotoMetadata Original { get => _original; set { _original = value; OnPropertyChanged(string.Empty); } }
    [Browsable(false)] public MetadataPatch PendingChanges { get; } = new();
    [Browsable(false)] public string? Error { get => _error; set { _error = value; OnPropertyChanged(); OnPropertyChanged(nameof(Status)); OnPropertyChanged(nameof(Details)); } }
    [Browsable(false)] public string? ImportNotice { get => _importNotice; set { _importNotice = value; OnPropertyChanged(); OnPropertyChanged(nameof(Status)); OnPropertyChanged(nameof(Details)); } }
    public bool IsSelected { get => _isSelected; set { if (_isSelected == value) return; _isSelected = value; OnPropertyChanged(); } }

    public string Name => Path.GetFileNameWithoutExtension(FilePath);
    public string FileName => Path.GetFileName(FilePath);
    public string Date => EffectiveCaptureDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty;
    public string Timezone => EffectiveOffset is TimeSpan offset ? FormatOffset(offset) : string.Empty;
    public string Latitude => EffectiveLatitude?.ToString("F6") ?? string.Empty;
    public string Longitude => EffectiveLongitude?.ToString("F6") ?? string.Empty;
    public string Altitude => EffectiveAltitude?.ToString("F2") ?? string.Empty;
    public string Location => HasResolvedEffectiveLocation
        ? _resolvedLocation
        : EffectiveLatitude.HasValue && EffectiveLongitude.HasValue ? "Identification…" : string.Empty;
    public string Device => string.Join(" ", new[] { Original.CameraMake, Original.CameraModel }.Where(value => !string.IsNullOrWhiteSpace(value)));
    public string Dimensions => Original.Width.HasValue && Original.Height.HasValue ? $"{Original.Width}×{Original.Height}" : string.Empty;
    public string Status => Error is not null ? "Error" : PendingChanges.HasChanges ? "Modified" : ImportNotice is not null ? "Metadata missing" : !Original.CaptureDate.HasValue ? "Metadata issue" : "Unchanged";
    public string Details => Error ?? (PendingChanges.HasChanges
        ? DescribePendingChanges()
        : ImportNotice ?? (!Original.CaptureDate.HasValue ? "Aucune date de prise de vue n’a été trouvée." : string.Empty));
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

    public void SetResolvedLocation(double latitude, double longitude, string address)
    {
        _resolvedLatitude = latitude;
        _resolvedLongitude = longitude;
        _resolvedLocation = address.Trim();
        OnPropertyChanged(nameof(Location));
    }

    private bool HasResolvedEffectiveLocation =>
        EffectiveLatitude is double latitude && EffectiveLongitude is double longitude &&
        _resolvedLatitude is double resolvedLatitude && _resolvedLongitude is double resolvedLongitude &&
        Math.Abs(latitude - resolvedLatitude) < 0.0000005 && Math.Abs(longitude - resolvedLongitude) < 0.0000005 &&
        !string.IsNullOrWhiteSpace(_resolvedLocation);

    private string DescribePendingChanges()
    {
        var changes = new List<string>();
        if (PendingChanges.HasDateChange)
            changes.Add($"Date : {FormatDate(Original.CaptureDate)} → {FormatDate(EffectiveCaptureDate)}");
        if (PendingChanges.HasOffsetChange)
            changes.Add($"Fuseau : {FormatNullableOffset(Original.Offset)} → {FormatNullableOffset(EffectiveOffset)}");
        if (PendingChanges.RemoveLocation)
            changes.Add("Localisation GPS supprimée");
        else if (PendingChanges.HasLocationChange)
            changes.Add($"GPS : {FormatGps(Original.Latitude, Original.Longitude, Original.Altitude)} → {FormatGps(EffectiveLatitude, EffectiveLongitude, EffectiveAltitude)}");
        return string.Join(" ; ", changes);
    }

    private static string FormatDate(DateTime? value) => value?.ToString("yyyy-MM-dd HH:mm:ss") ?? "absente";
    private static string FormatNullableOffset(TimeSpan? value) => value.HasValue ? FormatOffset(value.Value) : "absent";

    private static string FormatGps(double? latitude, double? longitude, double? altitude)
    {
        if (!latitude.HasValue || !longitude.HasValue) return "absent";
        var coordinates = $"{latitude.Value:F6}, {longitude.Value:F6}";
        return altitude.HasValue ? $"{coordinates}, {altitude.Value:F2} m" : coordinates;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    private static string FormatOffset(TimeSpan offset) => $"{(offset < TimeSpan.Zero ? "-" : "+")}{Math.Abs((int)offset.TotalHours):00}:{Math.Abs(offset.Minutes):00}";
}
