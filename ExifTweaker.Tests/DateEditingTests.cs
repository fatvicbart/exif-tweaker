using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker.Tests;

[TestClass]
public sealed class DateEditingTests
{
    [TestMethod]
    public void ShiftAfterSetUsesTheStagedDate()
    {
        var item = Item(new DateTime(2020, 1, 1, 10, 0, 0));
        var controller = Controller();
        controller.StageDate(new[] { item }, new DateTime(2024, 5, 1, 12, 0, 0));
        controller.ShiftDate(new[] { item }, TimeSpan.FromMinutes(30));
        Assert.AreEqual(new DateTime(2024, 5, 1, 12, 30, 0), item.EffectiveCaptureDate);
    }

    [TestMethod]
    public void CalendarShiftSupportsYearsMonthsDaysAndSeconds()
    {
        var item = Item(new DateTime(2020, 1, 31, 10, 0, 0));
        Controller().EditDate(new[] { item }, new DateEditRequest
        {
            Mode = DateEditMode.Shift, Years = 1, Months = 1, Days = 1, Seconds = 15
        });
        Assert.AreEqual(new DateTime(2021, 3, 1, 10, 0, 15), item.EffectiveCaptureDate);
    }

    [TestMethod]
    public void TimezoneConversionPreservesInstant()
    {
        var item = Item(new DateTime(2024, 1, 1, 12, 0, 0), TimeSpan.FromHours(2));
        Controller().EditDate(new[] { item }, new DateEditRequest
        {
            Mode = DateEditMode.Shift,
            ChangeTimezone = true,
            TimezoneOffset = TimeSpan.FromHours(-5),
            TimezoneMode = TimezoneChangeMode.ConvertInstant
        });
        Assert.AreEqual(new DateTime(2024, 1, 1, 5, 0, 0), item.EffectiveCaptureDate);
        Assert.AreEqual(TimeSpan.FromHours(-5), item.EffectiveOffset);
    }

    private static PhotoItem Item(DateTime date, TimeSpan? offset = null)
    {
        var item = new PhotoItem("sample.jpg") { Original = new PhotoMetadata { CaptureDate = date, Offset = offset } };
        return item;
    }

    private static SessionController Controller() => new(new ImportSession(), new EditHistory());
}
