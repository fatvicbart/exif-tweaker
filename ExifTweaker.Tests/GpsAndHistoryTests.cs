using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker.Tests;

[TestClass]
public sealed class GpsAndHistoryTests
{
    [TestMethod]
    public void GpsPatchIsNonDestructiveUntilApply()
    {
        var item = new PhotoItem("sample.jpg") { Original = new PhotoMetadata { Latitude = 1, Longitude = 2 } };
        var session = new ImportSession();
        session.AddRange(new[] { item });
        var controller = new SessionController(session, new EditHistory());
        controller.SetLocation(new[] { item }, 48.8566, 2.3522, 35, new LocationEditorService());
        Assert.AreEqual(1d, item.Original.Latitude);
        Assert.AreEqual(48.8566, item.EffectiveLatitude);
        Assert.IsTrue(item.PendingChanges.HasChanges);
    }

    [TestMethod]
    public void UndoRedoRestoresCompletePatch()
    {
        var item = new PhotoItem("sample.jpg") { Original = new PhotoMetadata { CaptureDate = new DateTime(2024, 1, 1) } };
        var session = new ImportSession();
        session.AddRange(new[] { item });
        var history = new EditHistory();
        var controller = new SessionController(session, history);
        controller.ShiftDate(new[] { item }, TimeSpan.FromHours(2));
        Assert.IsTrue(history.Undo(session.Media));
        Assert.IsFalse(item.PendingChanges.HasChanges);
        Assert.IsTrue(history.Redo(session.Media));
        Assert.AreEqual(TimeSpan.FromHours(2), item.PendingChanges.DateShift);
    }

    [TestMethod]
    public void InvalidCoordinatesAreRejected()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => LocationEditorService.Validate(91, 0));
    }
}
