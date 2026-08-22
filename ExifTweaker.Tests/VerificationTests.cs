using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker.Tests;

[TestClass]
public sealed class VerificationTests
{
    [TestMethod]
    public void ReadBackVerificationDetectsGpsMismatch()
    {
        var patch = new MetadataPatch { Latitude = 48.8, Longitude = 2.3 };
        var expected = new PhotoMetadata { Latitude = 48.8, Longitude = 2.3 };
        var actual = new PhotoMetadata { Latitude = 40, Longitude = 2.3 };
        Assert.IsFalse(MetadataService.VerifyCriticalMetadata("sample.jpg", expected, patch, actual, out var error));
        StringAssert.Contains(error, "GPS coordinates");
    }

    [TestMethod]
    public void ReadBackVerificationAcceptsSmallGpsRounding()
    {
        var patch = new MetadataPatch { Latitude = 48.8, Longitude = 2.3 };
        var expected = new PhotoMetadata { Latitude = 48.8, Longitude = 2.3 };
        var actual = new PhotoMetadata { Latitude = 48.800001, Longitude = 2.300001 };
        Assert.IsTrue(MetadataService.VerifyCriticalMetadata("sample.jpg", expected, patch, actual, out _));
    }
}
