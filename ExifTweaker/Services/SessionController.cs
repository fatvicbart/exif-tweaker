using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed class SessionController
{
    public SessionController(ImportSession session, EditHistory history)
    {
        Session = session;
        History = history;
    }

    public ImportSession Session { get; }
    public EditHistory History { get; }

    public void StageDate(IEnumerable<PhotoItem> items, DateTime date)
    {
        EditDate(items, new DateEditRequest { Mode = DateEditMode.Set, Date = date });
    }

    public void StageVisibleValues(
        IEnumerable<PhotoItem> items,
        DateTime date,
        GpsCoordinate? location,
        LocationEditorService locations)
    {
        var selected = items.ToList();
        if (selected.Count == 0) return;
        if (location is not null)
            LocationEditorService.Validate(location.Latitude, location.Longitude, location.Altitude);

        History.Capture(selected);
        foreach (var item in selected)
        {
            item.PendingChanges.CaptureDate = date;
            item.PendingChanges.DateShift = null;
            item.PendingChanges.DateShiftYears = 0;
            item.PendingChanges.DateShiftMonths = 0;
            item.NotifyChanged();
        }

        if (location is not null)
            locations.SetLocation(selected, location.Latitude, location.Longitude, location.Altitude);

        Session.NotifyChanged();
    }

    public void ShiftDate(IEnumerable<PhotoItem> items, TimeSpan shift)
    {
        EditDate(items, new DateEditRequest
        {
            Mode = DateEditMode.Shift,
            Days = shift.Days,
            Hours = shift.Hours,
            Minutes = shift.Minutes,
            Seconds = shift.Seconds
        });
    }

    public void EditDate(IEnumerable<PhotoItem> items, DateEditRequest request)
    {
        var selected = items.ToList();
        if (selected.Count == 0) return;
        History.Capture(selected);
        foreach (var item in selected)
        {
            var patch = item.PendingChanges;
            if (request.Mode == DateEditMode.Set)
            {
                patch.CaptureDate = request.Date ?? throw new ArgumentException("A date is required in Set mode.", nameof(request));
                patch.DateShift = null;
                patch.DateShiftYears = 0;
                patch.DateShiftMonths = 0;
            }
            else
            {
                patch.DateShiftYears += request.Years;
                patch.DateShiftMonths += request.Months;
                patch.DateShift = (patch.DateShift ?? TimeSpan.Zero) + request.ClockShift;
            }

            if (request.ChangeTimezone)
            {
                patch.OffsetTimeOriginal = request.RemoveTimezone ? null : request.TimezoneOffset;
                patch.RemoveOffsetTimeOriginal = request.RemoveTimezone;
                patch.ConvertToOffset = !request.RemoveTimezone && request.TimezoneMode == TimezoneChangeMode.ConvertInstant;
            }
            item.NotifyChanged();
        }
        Session.NotifyChanged();
    }

    public void SetLocation(IEnumerable<PhotoItem> items, double latitude, double longitude, double? altitude, LocationEditorService locations)
    {
        var selected = items.ToList();
        if (selected.Count == 0) return;
        History.Capture(selected);
        locations.SetLocation(selected, latitude, longitude, altitude);
        Session.NotifyChanged();
    }

    public void RemoveLocation(IEnumerable<PhotoItem> items, LocationEditorService locations)
    {
        var selected = items.ToList();
        if (selected.Count == 0) return;
        History.Capture(selected);
        locations.RemoveLocation(selected);
        Session.NotifyChanged();
    }

    public void Reset(IEnumerable<PhotoItem> items)
    {
        var selected = items.ToList();
        if (selected.Count == 0) return;
        History.Capture(selected);
        foreach (var item in selected) { item.PendingChanges.Clear(); item.NotifyChanged(); }
        Session.NotifyChanged();
    }
}
