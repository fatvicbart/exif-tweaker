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
        var selected = items.ToList();
        History.Capture(selected);
        foreach (var item in selected) { item.PendingChanges.CaptureDate = date; item.NotifyChanged(); }
        Session.NotifyChanged();
    }

    public void ShiftDate(IEnumerable<PhotoItem> items, TimeSpan shift)
    {
        var selected = items.ToList();
        History.Capture(selected);
        foreach (var item in selected) { item.PendingChanges.CaptureDate = null; item.PendingChanges.DateShift = (item.PendingChanges.DateShift ?? TimeSpan.Zero) + shift; item.NotifyChanged(); }
        Session.NotifyChanged();
    }

    public void SetLocation(IEnumerable<PhotoItem> items, double latitude, double longitude, double? altitude, LocationEditorService locations)
    {
        var selected = items.ToList();
        History.Capture(selected);
        locations.SetLocation(selected, latitude, longitude, altitude);
        Session.NotifyChanged();
    }

    public void RemoveLocation(IEnumerable<PhotoItem> items, LocationEditorService locations)
    {
        var selected = items.ToList();
        History.Capture(selected);
        locations.RemoveLocation(selected);
        Session.NotifyChanged();
    }

    public void Reset(IEnumerable<PhotoItem> items)
    {
        var selected = items.ToList();
        History.Capture(selected);
        foreach (var item in selected) { item.PendingChanges.Clear(); item.NotifyChanged(); }
        Session.NotifyChanged();
    }
}
