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
}
