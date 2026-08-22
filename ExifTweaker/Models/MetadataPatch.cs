namespace ExifTweaker.Models;

public sealed class MetadataPatch
{
    public DateTime? CaptureDate { get; set; }
    public TimeSpan? DateShift { get; set; }
    public int DateShiftYears { get; set; }
    public int DateShiftMonths { get; set; }
    public TimeSpan? OffsetTimeOriginal { get; set; }
    public bool RemoveOffsetTimeOriginal { get; set; }
    public bool ConvertToOffset { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? Altitude { get; set; }
    public bool RemoveAltitude { get; set; }
    public bool RemoveLocation { get; set; }

    public bool HasDateChange => CaptureDate.HasValue || DateShift.HasValue || DateShiftYears != 0 || DateShiftMonths != 0 || ConvertToOffset;
    public bool HasLocationChange => (Latitude.HasValue && Longitude.HasValue) || Altitude.HasValue || RemoveAltitude || RemoveLocation;
    public bool HasOffsetChange => OffsetTimeOriginal.HasValue || RemoveOffsetTimeOriginal;
    public bool HasChanges => HasDateChange || HasOffsetChange || HasLocationChange;

    public MetadataPatch Clone() => new()
    {
        CaptureDate = CaptureDate, DateShift = DateShift, DateShiftYears = DateShiftYears, DateShiftMonths = DateShiftMonths,
        OffsetTimeOriginal = OffsetTimeOriginal, RemoveOffsetTimeOriginal = RemoveOffsetTimeOriginal, ConvertToOffset = ConvertToOffset,
        Latitude = Latitude, Longitude = Longitude, Altitude = Altitude, RemoveAltitude = RemoveAltitude, RemoveLocation = RemoveLocation
    };

    public void CopyFrom(MetadataPatch source)
    {
        CaptureDate = source.CaptureDate; DateShift = source.DateShift; DateShiftYears = source.DateShiftYears; DateShiftMonths = source.DateShiftMonths;
        OffsetTimeOriginal = source.OffsetTimeOriginal; RemoveOffsetTimeOriginal = source.RemoveOffsetTimeOriginal; ConvertToOffset = source.ConvertToOffset;
        Latitude = source.Latitude; Longitude = source.Longitude; Altitude = source.Altitude; RemoveAltitude = source.RemoveAltitude; RemoveLocation = source.RemoveLocation;
    }

    public void Clear()
    {
        CaptureDate = null;
        DateShift = null;
        DateShiftYears = 0;
        DateShiftMonths = 0;
        OffsetTimeOriginal = null;
        RemoveOffsetTimeOriginal = false;
        ConvertToOffset = false;
        Latitude = null;
        Longitude = null;
        Altitude = null;
        RemoveAltitude = false;
        RemoveLocation = false;
    }
}
