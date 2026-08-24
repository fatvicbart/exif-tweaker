using ExifTweaker.Infrastructure;
using ExifTweaker.Services;

namespace ExifTweaker.Tests;

[TestClass]
public sealed class ExifMetadataParsingTests
{
    [TestMethod]
    public void FullMetadataParserPreservesGroupsAndFormatsValues()
    {
        const string json = """
            [{
              "File:SourceFile": "photo.jpg",
              "EXIF:Make": "Camera Corp",
              "XMP:Subject": ["travel", "family"],
              "Composite:Rotation": 90
            }]
            """;

        var tags = ExifToolService.ParseAllMetadataJson(json);

        Assert.AreEqual("Camera Corp", tags.Single(tag => tag.Group == "EXIF" && tag.Name == "Make").Value);
        Assert.AreEqual("travel, family", tags.Single(tag => tag.Group == "XMP" && tag.Name == "Subject").Value);
        Assert.AreEqual("90", tags.Single(tag => tag.Group == "Composite" && tag.Name == "Rotation").Value);
    }

    [TestMethod]
    public void NewSettingsUseNominatimByDefault()
    {
        Assert.AreEqual("Nominatim", new AppSettings().GeocodingProvider);
        Assert.AreEqual(AppThemeMode.Automatic, new AppSettings().Theme);
    }
    [TestMethod]
    public void LogParserProducesReadableStructuredEntry()
    {
        const string json = """{"timestamp":"2026-08-24T12:34:56+02:00","level":"error","message":"Import failed","exceptionType":"System.IO.IOException","exception":"stack trace"}""";

        var entry = AppLogger.ParseLine(json);

        Assert.IsTrue(entry.IsValid);
        Assert.AreEqual("error", entry.Level);
        Assert.AreEqual("Import failed", entry.Message);
        Assert.AreEqual("System.IO.IOException", entry.ExceptionType);
        Assert.AreEqual("stack trace", entry.ExceptionText);
    }

    [TestMethod]
    public void LogParserKeepsMalformedSourceVisible()
    {
        var entry = AppLogger.ParseLine("not-json");

        Assert.IsFalse(entry.IsValid);
        Assert.AreEqual("invalid", entry.Level);
        Assert.AreEqual("not-json", entry.RawJson);
    }
}
