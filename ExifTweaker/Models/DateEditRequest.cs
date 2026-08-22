namespace ExifTweaker.Models;

public enum DateEditMode { Set, Shift }
public enum TimezoneChangeMode { KeepLocalTime, ConvertInstant }

public sealed record DateEditRequest
{
    public DateEditMode Mode { get; init; }
    public DateTime? Date { get; init; }
    public int Years { get; init; }
    public int Months { get; init; }
    public int Days { get; init; }
    public int Hours { get; init; }
    public int Minutes { get; init; }
    public int Seconds { get; init; }
    public bool ChangeTimezone { get; init; }
    public TimeSpan? TimezoneOffset { get; init; }
    public bool RemoveTimezone { get; init; }
    public TimezoneChangeMode TimezoneMode { get; init; } = TimezoneChangeMode.KeepLocalTime;

    public TimeSpan ClockShift => new(Days, Hours, Minutes, Seconds);
}
