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

    [TestMethod]
    public void PrepareVisibleValuesStagesDateAndGpsAsOneUndoableAction()
    {
        var newDate = new DateTime(2024, 5, 1, 12, 30, 0);
        var item = new PhotoItem("sample.jpg")
        {
            Original = new PhotoMetadata { CaptureDate = new DateTime(2024, 1, 1, 8, 0, 0) }
        };
        var session = new ImportSession();
        session.AddRange(new[] { item });
        var history = new EditHistory();
        var controller = new SessionController(session, history);

        controller.StageVisibleValues(
            new[] { item },
            newDate,
            new GpsCoordinate(48.8566, 2.3522, 35),
            new LocationEditorService());

        Assert.AreEqual(newDate, item.PendingChanges.CaptureDate);
        Assert.AreEqual(48.8566, item.PendingChanges.Latitude);
        Assert.AreEqual(2.3522, item.PendingChanges.Longitude);
        Assert.AreEqual(35d, item.PendingChanges.Altitude);
        Assert.IsTrue(history.Undo(session.Media));
        Assert.IsFalse(item.PendingChanges.HasChanges);
        Assert.IsFalse(history.CanUndo);
    }

    [TestMethod]
    public void EmptySelectionDoesNotCreateHistoryEntry()
    {
        var session = new ImportSession();
        var history = new EditHistory();
        var controller = new SessionController(session, history);

        controller.SetLocation(Array.Empty<PhotoItem>(), 48.8566, 2.3522, null, new LocationEditorService());
        controller.ShiftDate(Array.Empty<PhotoItem>(), TimeSpan.FromHours(1));

        Assert.IsFalse(history.CanUndo);
    }
}
