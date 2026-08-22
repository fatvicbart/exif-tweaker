using System.Security.Cryptography;
using ExifTweaker.Infrastructure;
using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker.Tests;

[TestClass]
public sealed class ExifToolIntegrationTests
{
    private static string BundledExecutable =>
        Path.Combine(AppContext.BaseDirectory, "exiftool", "exiftool.exe");

    [TestMethod]
    public async Task BundledExifToolStartsAndReturnsAVersion()
    {
        RequireWindows();
        Assert.IsTrue(File.Exists(BundledExecutable), $"Bundled ExifTool was not copied to {BundledExecutable}.");

        var service = new ExifToolService(BundledExecutable);
        var version = await service.GetVersionAsync();

        Assert.IsTrue(service.IsAvailable);
        Assert.IsTrue(Version.TryParse(version, out _), $"Unexpected ExifTool version: {version}");
    }

    [TestMethod]
    public async Task RealExifToolWritesReadsBackBacksUpAndRestoresUnicodePath()
    {
        RequireWindows();
        Assert.IsTrue(File.Exists(BundledExecutable), $"Bundled ExifTool was not copied to {BundledExecutable}.");

        var directory = Path.Combine(Path.GetTempPath(), "ExifTweaker intégration espace " + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var mediaPath = Path.Combine(directory, "média test.tif");
            CreateMinimalTiff(mediaPath);
            var originalCaptureDate = new DateTime(2019, 6, 15, 10, 20, 30);
            await SeedCameraMetadataAsync(mediaPath, originalCaptureDate);
            var originalHash = SHA256.HashData(await File.ReadAllBytesAsync(mediaPath));
            var service = new ExifToolService(BundledExecutable);
            var metadata = new MetadataService(service, new AppSettings
            {
                ExifToolPath = BundledExecutable,
                BackupStrategy = BackupStrategy.ExifToolOriginal,
                MaxParallelism = 1
            });
            var item = (await metadata.LoadAsync(new[] { mediaPath })).Single();
            Assert.IsNull(item.Error, item.Details);
            Assert.IsNull(item.ImportNotice, item.Details);
            Assert.AreEqual(originalCaptureDate, item.Original.CaptureDate);
            Assert.AreEqual("ExifTweaker Test Camera", item.Original.CameraMake);
            Assert.AreEqual("Digital Camera 1000", item.Original.CameraModel);
            Assert.AreEqual("Unchanged", item.Status);
            var expectedDate = new DateTime(2026, 8, 22, 14, 35, 47);
            item.PendingChanges.CaptureDate = expectedDate;
            item.PendingChanges.OffsetTimeOriginal = TimeSpan.FromHours(2);
            item.PendingChanges.Latitude = 48.8566;
            item.PendingChanges.Longitude = 2.3522;
            item.PendingChanges.Altitude = 35.5;

            var applyResult = await metadata.ApplyPendingChangesAsync(new[] { item });

            var applyErrors = string.Join(" | ", applyResult.Files
                .Where(result => !result.Succeeded).Select(result => result.Error ?? "Cancelled"));
            Assert.AreEqual(1, applyResult.SucceededCount, applyErrors);
            Assert.AreEqual(0, applyResult.FailedCount);
            Assert.IsFalse(item.PendingChanges.HasChanges, "The verified patch was not cleared.");
            Assert.IsTrue(File.Exists(mediaPath + "_original"), "ExifTool did not create its original backup.");
            Assert.AreEqual(expectedDate, item.Original.CaptureDate);
            Assert.AreEqual(TimeSpan.FromHours(2), item.Original.Offset);
            Assert.AreEqual(48.8566, item.Original.Latitude!.Value, 0.00001);
            Assert.AreEqual(2.3522, item.Original.Longitude!.Value, 0.00001);
            Assert.AreEqual(35.5, item.Original.Altitude!.Value, 0.5);

            await metadata.RestoreBackupAsync(item);

            var restoredHash = SHA256.HashData(await File.ReadAllBytesAsync(mediaPath));
            CollectionAssert.AreEqual(originalHash, restoredHash, "The restored media differs from the original bytes.");
            Assert.AreEqual(originalCaptureDate, item.Original.CaptureDate, "Restored camera metadata was not reloaded from the original.");
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }

    private static void RequireWindows()
    {
        if (!OperatingSystem.IsWindows())
            Assert.Inconclusive("The bundled exiftool.exe integration test requires Windows.");
    }

    private static async Task SeedCameraMetadataAsync(string path, DateTime captureDate)
    {
        var argumentPath = Path.Combine(Path.GetTempPath(), $"exiftweaker-seed-{Guid.NewGuid():N}.args");
        try
        {
            var value = captureDate.ToString("yyyy:MM:dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            var arguments = new[]
            {
                "-overwrite_original",
                $"-DateTimeOriginal={value}",
                $"-CreateDate={value}",
                "-Make=ExifTweaker Test Camera",
                "-Model=Digital Camera 1000",
                path
            };
            await File.WriteAllBytesAsync(argumentPath, ExifToolService.EncodeArgumentFile(arguments));

            var startInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = BundledExecutable,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8,
                StandardErrorEncoding = System.Text.Encoding.UTF8,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("-charset");
            startInfo.ArgumentList.Add("filename=UTF8");
            startInfo.ArgumentList.Add("-@");
            startInfo.ArgumentList.Add(argumentPath);

            using var process = System.Diagnostics.Process.Start(startInfo) ?? throw new InvalidOperationException("ExifTool seed process did not start.");
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            Assert.AreEqual(0, process.ExitCode, $"ExifTool seed failed: {await stderr} {await stdout}");
        }
        finally
        {
            if (File.Exists(argumentPath)) File.Delete(argumentPath);
        }
    }

    private static void CreateMinimalTiff(string path)
    {
        using var stream = File.Create(path);
        using var writer = new BinaryWriter(stream);
        writer.Write((ushort)0x4949);
        writer.Write((ushort)42);
        writer.Write((uint)8);
        writer.Write((ushort)8);
        WriteEntry(writer, 256, 3, 1, 1);
        WriteEntry(writer, 257, 3, 1, 1);
        WriteEntry(writer, 258, 3, 1, 8);
        WriteEntry(writer, 259, 3, 1, 1);
        WriteEntry(writer, 262, 3, 1, 1);
        WriteEntry(writer, 273, 4, 1, 110);
        WriteEntry(writer, 278, 4, 1, 1);
        WriteEntry(writer, 279, 4, 1, 1);
        writer.Write((uint)0);
        writer.Write((byte)0);
    }

    private static void WriteEntry(BinaryWriter writer, ushort tag, ushort type, uint count, uint value)
    {
        writer.Write(tag);
        writer.Write(type);
        writer.Write(count);
        writer.Write(value);
    }
}
