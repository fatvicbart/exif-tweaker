using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker.Tests;

[TestClass]
public sealed class MetadataImportTests
{
    [TestMethod]
    public void MissingExifToolRecordUsesFileSystemFallbackInsteadOfError()
    {
        var directory = Path.Combine(Path.GetTempPath(), "ExifTweaker metadata fallback " + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var filePath = Path.Combine(directory, "image sans métadonnées.jpg");
        File.WriteAllBytes(filePath, new byte[] { 1, 2, 3 });

        try
        {
            var item = MetadataService.CreateImportedItem(
                filePath,
                new Dictionary<string, PhotoMetadata>(StringComparer.OrdinalIgnoreCase));

            Assert.IsNull(item.Error);
            Assert.IsNotNull(item.ImportNotice);
            Assert.AreEqual("Metadata missing", item.Status);
            Assert.AreEqual("JPG", item.Original.FileType);
            Assert.AreEqual("image/jpeg", item.Original.MimeType);
            Assert.IsNotNull(item.Original.FileCreateDate);
            Assert.IsNotNull(item.Original.FileModifyDate);
            StringAssert.Contains(item.Details, "reste modifiable");
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [TestMethod]
    public void ReturnedMetadataIsUsedWithoutFallbackNotice()
    {
        var filePath = Path.GetFullPath("image-with-metadata.jpg");
        var expectedDate = new DateTime(2025, 4, 3, 12, 30, 0);
        var metadata = new Dictionary<string, PhotoMetadata>(StringComparer.OrdinalIgnoreCase)
        {
            [filePath] = new PhotoMetadata { CaptureDate = expectedDate, FileType = "JPEG" }
        };

        var item = MetadataService.CreateImportedItem(filePath, metadata);

        Assert.IsNull(item.Error);
        Assert.IsNull(item.ImportNotice);
        Assert.AreEqual(expectedDate, item.Original.CaptureDate);
        Assert.AreEqual("Unchanged", item.Status);
        Assert.AreEqual(string.Empty, item.Details);
    }

    [TestMethod]
    public void RealErrorTakesPriorityOverImportNotice()
    {
        var item = new PhotoItem("broken.jpg")
        {
            ImportNotice = "Métadonnées absentes",
            Error = "Lecture impossible"
        };

        Assert.AreEqual("Error", item.Status);
        Assert.AreEqual("Lecture impossible", item.Details);
    }
}
