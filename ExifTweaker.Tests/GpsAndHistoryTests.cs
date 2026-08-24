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
    public void DateAndGpsPreparationAreIndependent()
    {
        var newDate = new DateTime(2024, 5, 1, 12, 30, 0);
        var item = new PhotoItem("sample.jpg") { Original = new PhotoMetadata { CaptureDate = new DateTime(2024, 1, 1) } };
        var session = new ImportSession();
        session.AddRange(new[] { item });
        var controller = new SessionController(session, new EditHistory());

        controller.StageDate(new[] { item }, newDate);
        Assert.AreEqual(newDate, item.PendingChanges.CaptureDate);
        Assert.IsNull(item.PendingChanges.Latitude);

        controller.SetLocation(new[] { item }, 48.8566, 2.3522, 35, new LocationEditorService());
        Assert.AreEqual(newDate, item.PendingChanges.CaptureDate);
        Assert.AreEqual(48.8566, item.PendingChanges.Latitude);
        Assert.AreEqual(2.3522, item.PendingChanges.Longitude);
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

    [TestMethod]
    public void ModifiedDetailsDescribeEveryPreparedMetadataChange()
    {
        var item = new PhotoItem("sample.jpg")
        {
            Original = new PhotoMetadata
            {
                CaptureDate = new DateTime(2024, 1, 1, 8, 0, 0),
                Offset = TimeSpan.FromHours(1),
                Latitude = 1,
                Longitude = 2,
                Altitude = 10
            }
        };
        item.PendingChanges.CaptureDate = new DateTime(2024, 2, 3, 9, 30, 0);
        item.PendingChanges.OffsetTimeOriginal = TimeSpan.FromHours(2);
        item.PendingChanges.Latitude = 48.8566;
        item.PendingChanges.Longitude = 2.3522;
        item.PendingChanges.RemoveAltitude = true;
        item.NotifyChanged();

        Assert.AreEqual("Modified", item.Status);
        StringAssert.Contains(item.Details, "Date : 2024-01-01 08:00:00 → 2024-02-03 09:30:00");
        StringAssert.Contains(item.Details, "Fuseau : +01:00 → +02:00");
        StringAssert.Contains(item.Details, "GPS : 1.000000, 2.000000, 10.00 m → 48.856600, 2.352200");
    }

    [TestMethod]
    public void ResolvedLocationOnlyAppliesToMatchingEffectiveCoordinates()
    {
        var item = new PhotoItem("sample.jpg")
        {
            Original = new PhotoMetadata { Latitude = 48.8566, Longitude = 2.3522 }
        };

        Assert.AreEqual("Identification…", item.Location);
        item.SetResolvedLocation(48.8566, 2.3522, "Paris, France");
        Assert.AreEqual("Paris, France", item.Location);

        item.PendingChanges.Latitude = 45.764;
        item.PendingChanges.Longitude = 4.8357;
        item.NotifyChanged();

        Assert.AreEqual("Identification…", item.Location);
    }

    [TestMethod]
    public void RemovedGpsIsExplicitInModifiedDetails()
    {
        var item = new PhotoItem("sample.jpg")
        {
            Original = new PhotoMetadata { Latitude = 48.8566, Longitude = 2.3522 }
        };

        new LocationEditorService().RemoveLocation(new[] { item });

        Assert.AreEqual("Modified", item.Status);
        StringAssert.Contains(item.Details, "Localisation GPS supprimée");
        Assert.AreEqual(string.Empty, item.Location);
    }
    [TestMethod]
    public void IdenticalPreparedValuesDoNotCreateAModification()
    {
        var originalDate = new DateTime(2024, 1, 1, 8, 0, 0);
        var item = new PhotoItem("sample.jpg")
        {
            Original = new PhotoMetadata { CaptureDate = originalDate, Latitude = 48.8566, Longitude = 2.3522, Altitude = 35 }
        };
        var session = new ImportSession();
        session.AddRange(new[] { item });
        var controller = new SessionController(session, new EditHistory());

        controller.StageDate(new[] { item }, originalDate);
        controller.SetLocation(new[] { item }, 48.8566, 2.3522, 35, new LocationEditorService());

        Assert.IsFalse(item.PendingChanges.HasChanges);
        Assert.AreEqual("Unchanged", item.Status);
        Assert.AreEqual(string.Empty, item.Details);
        Assert.AreEqual(0, session.PendingChangeCount);
        Assert.IsFalse(controller.History.CanUndo);
    }

    [TestMethod]
    public void ModifiedDetailsUseOneLinePerChange()
    {
        var item = new PhotoItem("sample.jpg")
        {
            Original = new PhotoMetadata { CaptureDate = new DateTime(2024, 1, 1), Latitude = 1, Longitude = 2 }
        };
        item.PendingChanges.CaptureDate = new DateTime(2024, 1, 2);
        item.PendingChanges.Latitude = 3;
        item.PendingChanges.Longitude = 4;

        StringAssert.Contains(item.Details, Environment.NewLine);
        Assert.AreEqual(2, item.Details.Split(Environment.NewLine).Length);
    }
    [TestMethod]
    public void GpsPreparationRaisesOneItemNotification()
    {
        var item = new PhotoItem("sample.jpg");
        var session = new ImportSession();
        session.AddRange(new[] { item });
        var notifications = 0;
        item.PropertyChanged += (_, _) => notifications++;

        new SessionController(session, new EditHistory()).SetLocation(
            new[] { item }, 48.8566, 2.3522, 35, new LocationEditorService());

        Assert.AreEqual(1, notifications);
    }

    [TestMethod]
    public void RemoveRangeRaisesOneSessionNotification()
    {
        var first = new PhotoItem("first.jpg");
        var second = new PhotoItem("second.jpg");
        var session = new ImportSession();
        session.AddRange(new[] { first, second });
        var notifications = 0;
        session.PropertyChanged += (_, _) => notifications++;

        session.RemoveRange(new[] { first, second });

        Assert.AreEqual(0, session.Media.Count);
        Assert.AreEqual(1, notifications);
    }
}
