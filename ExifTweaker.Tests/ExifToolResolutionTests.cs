using ExifTweaker.Services;

namespace ExifTweaker.Tests;

[TestClass]
public sealed class ExifToolResolutionTests
{
    private string _temporaryDirectory = null!;

    [TestInitialize]
    public void Initialize()
    {
        _temporaryDirectory = Path.Combine(Path.GetTempPath(), "ExifTweaker resolver é " + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_temporaryDirectory);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_temporaryDirectory))
            Directory.Delete(_temporaryDirectory, recursive: true);
    }

    [TestMethod]
    public void BundledExecutableIsPreferredOverPathFallback()
    {
        var bundledDirectory = Path.Combine(_temporaryDirectory, "exiftool");
        Directory.CreateDirectory(bundledDirectory);
        var bundledExecutable = Path.Combine(bundledDirectory, "exiftool.exe");
        File.WriteAllBytes(bundledExecutable, new byte[] { 0 });

        var resolved = ExifToolService.ResolveExecutable(applicationBaseDirectory: _temporaryDirectory);

        Assert.AreEqual(bundledExecutable, resolved);
    }

    [TestMethod]
    public void ConfiguredDirectoryResolvesItsExecutable()
    {
        var configuredDirectory = Path.Combine(_temporaryDirectory, "custom tool");
        Directory.CreateDirectory(configuredDirectory);

        var resolved = ExifToolService.ResolveExecutable(configuredDirectory, _temporaryDirectory);

        Assert.AreEqual(Path.Combine(configuredDirectory, "exiftool.exe"), resolved);
    }

    [TestMethod]
    public void ConfiguredExecutableAlwaysHasPriority()
    {
        var configuredExecutable = Path.Combine(_temporaryDirectory, "configured", "my-exiftool.exe");

        var resolved = ExifToolService.ResolveExecutable(configuredExecutable, _temporaryDirectory);

        Assert.AreEqual(configuredExecutable, resolved);
    }

    [TestMethod]
    public void MissingExecutableIsNotReportedAsAvailable()
    {
        var missing = Path.Combine(_temporaryDirectory, "missing-exiftool.exe");

        Assert.IsFalse(ExifToolService.ResolveAvailable(missing));
    }

    [TestMethod]
    public void ArgumentFileIsUtf8WithoutBomAndPreservesUnicode()
    {
        var unicodePath = @"C:\Photos\été\média test.tif";

        var bytes = ExifToolService.EncodeArgumentFile(new[] { "-json", unicodePath });

        Assert.IsFalse(bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF);
        Assert.AreEqual($"-json\n{unicodePath}\n", System.Text.Encoding.UTF8.GetString(bytes));
    }

    [TestMethod]
    public void ArgumentFileRejectsLineBreaks()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
            ExifToolService.EncodeArgumentFile(new[] { "-json", "invalid\nargument" }));
    }

    [TestMethod]
    public void PositiveExifOffsetIsParsed()
    {
        var offset = ExifToolService.ParseOffset("+02:00");

        Assert.AreEqual(TimeSpan.FromHours(2), offset);
    }

    [TestMethod]
    public void NegativeExifOffsetIsParsed()
    {
        var offset = ExifToolService.ParseOffset("-05:30");

        Assert.AreEqual(TimeSpan.FromHours(-5.5), offset);
    }

    [TestMethod]
    public void CameraMetadataJsonIsFullyParsed()
    {
        var filePath = Path.GetFullPath("camera-photo.jpg");
        var json = System.Text.Json.JsonSerializer.Serialize(new[]
        {
            new Dictionary<string, object>
            {
                ["SourceFile"] = filePath,
                ["DateTimeOriginal"] = "2024:07:18 14:35:42",
                ["Make"] = "Canon",
                ["Model"] = "EOS R6",
                ["LensModel"] = "RF24-105mm F4 L IS USM",
                ["ImageWidth"] = 6000,
                ["ImageHeight"] = 4000,
                ["FileType"] = "JPEG",
                ["MIMEType"] = "image/jpeg",
                ["GPSLatitude"] = 48.8566,
                ["GPSLongitude"] = 2.3522
            }
        });

        var parsed = ExifToolService.ParseMetadataJson(json, new[] { filePath });
        var metadata = parsed[filePath];

        Assert.AreEqual(new DateTime(2024, 7, 18, 14, 35, 42), metadata.CaptureDate);
        Assert.AreEqual("Canon", metadata.CameraMake);
        Assert.AreEqual("EOS R6", metadata.CameraModel);
        Assert.AreEqual("RF24-105mm F4 L IS USM", metadata.Lens);
        Assert.AreEqual(6000, metadata.Width);
        Assert.AreEqual(4000, metadata.Height);
        Assert.AreEqual(48.8566, metadata.Latitude!.Value, 0.00001);
        Assert.AreEqual(2.3522, metadata.Longitude!.Value, 0.00001);
    }

    [TestMethod]
    public void GroupedJsonWithoutSourceFileUsesRequestedFilePosition()
    {
        var filePath = Path.GetFullPath("grouped-camera-photo.jpg");
        const string json = """
            [{
              "EXIF:SubSecDateTimeOriginal": "2023:11:05 09:08:07.42",
              "EXIF:Make": "NIKON CORPORATION",
              "EXIF:CameraModelName": "NIKON Z 6",
              "File:ImageWidth": "6048",
              "File:ImageHeight": "4024"
            }]
            """;

        var parsed = ExifToolService.ParseMetadataJson(json, new[] { filePath });
        var metadata = parsed[filePath];

        Assert.AreEqual(new DateTime(2023, 11, 5, 9, 8, 7), metadata.CaptureDate);
        Assert.AreEqual("NIKON CORPORATION", metadata.CameraMake);
        Assert.AreEqual("NIKON Z 6", metadata.CameraModel);
        Assert.AreEqual(6048, metadata.Width);
        Assert.AreEqual(4024, metadata.Height);
    }

}
