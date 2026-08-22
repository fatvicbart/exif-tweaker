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
}
