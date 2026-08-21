using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using ExifTweaker.Infrastructure;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed class ExifToolService
{
    private const int ReadBatchSize = 100;
    private readonly string _executable;

    public ExifToolService(string? executable = null) => _executable = executable ?? ResolveExecutable();
    public bool IsAvailable => ResolveAvailable(_executable);

    public async Task<IReadOnlyDictionary<string, PhotoMetadata>> ReadAsync(IEnumerable<string> files, CancellationToken ct = default)
    {
        var filesToRead = files.Distinct(StringComparer.OrdinalIgnoreCase).Select(Path.GetFullPath).ToList();
        if (filesToRead.Count == 0) return new Dictionary<string, PhotoMetadata>();
        EnsureAvailable();

        var result = new Dictionary<string, PhotoMetadata>(StringComparer.OrdinalIgnoreCase);
        foreach (var batch in filesToRead.Chunk(ReadBatchSize))
        {
            ct.ThrowIfCancellationRequested();
            foreach (var pair in await ReadBatchAsync(batch, ct)) result[pair.Key] = pair.Value;
        }
        return result;
    }

    public async Task WriteAsync(PhotoItem item, bool backupOriginal = true, CancellationToken ct = default)
    {
        EnsureAvailable();
        var patch = item.PendingChanges;
        if (!patch.HasChanges) return;

        var args = new List<string>();
        if (!backupOriginal) args.Add("-overwrite_original");

        if (patch.HasDateChange)
        {
            if (item.EffectiveCaptureDate is not DateTime date)
                throw new InvalidOperationException("A date patch requires a capture date.");
            var value = date.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
            args.Add($"-DateTimeOriginal={value}");
            args.Add($"-CreateDate={value}");
            args.Add($"-ModifyDate={value}");
        }

        if (patch.RemoveOffsetTimeOriginal) args.Add("-OffsetTimeOriginal=");
        else if (patch.OffsetTimeOriginal is TimeSpan offset)
            args.Add($"-OffsetTimeOriginal={FormatExifOffset(offset)}");

        if (patch.RemoveLocation)
        {
            args.Add("-GPSLatitude="); args.Add("-GPSLongitude=");
            args.Add("-GPSLatitudeRef="); args.Add("-GPSLongitudeRef="); args.Add("-GPSAltitude=");
        }
        else if (patch.HasLocationChange && item.EffectiveLatitude is double lat && item.EffectiveLongitude is double lon)
        {
            ValidateCoordinates(lat, lon);
            args.Add($"-GPSLatitude={lat.ToString(CultureInfo.InvariantCulture)}");
            args.Add($"-GPSLongitude={lon.ToString(CultureInfo.InvariantCulture)}");
            args.Add($"-GPSLatitudeRef={(lat < 0 ? "S" : "N")}");
            args.Add($"-GPSLongitudeRef={(lon < 0 ? "W" : "E")}");
        }

        args.Add(item.FilePath);
        await RunAsync(args, ct);
    }

    public Task RestoreBackupAsync(PhotoItem item, CancellationToken ct = default) => Task.Run(() =>
    {
        var backup = item.FilePath + "_original";
        if (!File.Exists(backup)) throw new FileNotFoundException("ExifTool backup was not found.", backup);
        ct.ThrowIfCancellationRequested();
        File.Copy(backup, item.FilePath, overwrite: true);
    }, ct);

    private async Task<IReadOnlyDictionary<string, PhotoMetadata>> ReadBatchAsync(IEnumerable<string> files, CancellationToken ct)
    {
        var args = new List<string>
        {
            "-json", "-n", "-charset", "filename=UTF8",
            "-DateTimeOriginal", "-CreateDate", "-OffsetTimeOriginal", "-GPSLatitude", "-GPSLongitude", "-GPSAltitude",
            "-Make", "-Model", "-LensModel", "-ImageWidth", "-ImageHeight", "-FileType", "-MIMEType",
            "-FileCreateDate", "-FileModifyDate", "-City", "-Country"
        };
        args.AddRange(files);
        var output = await RunAsync(args, ct);
        using var document = JsonDocument.Parse(output);
        var result = new Dictionary<string, PhotoMetadata>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in document.RootElement.EnumerateArray())
        {
            var source = GetString(item, "SourceFile");
            if (string.IsNullOrWhiteSpace(source)) continue;
            result[Path.GetFullPath(source)] = new PhotoMetadata
            {
                CaptureDate = ParseExifDate(GetString(item, "DateTimeOriginal") ?? GetString(item, "CreateDate")),
                Offset = ParseOffset(GetString(item, "OffsetTimeOriginal")),
                Latitude = GetDouble(item, "GPSLatitude"), Longitude = GetDouble(item, "GPSLongitude"), Altitude = GetDouble(item, "GPSAltitude"),
                CameraMake = GetString(item, "Make"), CameraModel = GetString(item, "Model"), Lens = GetString(item, "LensModel"),
                Width = GetInt(item, "ImageWidth"), Height = GetInt(item, "ImageHeight"), FileType = GetString(item, "FileType"), MimeType = GetString(item, "MIMEType"),
                FileCreateDate = ParseExifDate(GetString(item, "FileCreateDate")), FileModifyDate = ParseExifDate(GetString(item, "FileModifyDate")),
                City = GetString(item, "City"), Country = GetString(item, "Country")
            };
        }
        return result;
    }

    private async Task<string> RunAsync(IEnumerable<string> arguments, CancellationToken ct)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _executable,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        foreach (var argument in arguments) startInfo.ArgumentList.Add(argument);

        using var process = new Process { StartInfo = startInfo };
        try
        {
            if (!process.Start()) throw new InvalidOperationException("ExifTool could not be started.");
        }
        catch (Exception ex)
        {
            AppLogger.Error("Unable to start ExifTool.", ex);
            throw;
        }

        using var registration = ct.Register(() =>
        {
            try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { }
        });
        var stdout = process.StandardOutput.ReadToEndAsync(ct);
        var stderr = process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);
        var output = await stdout;
        var error = await stderr;
        ct.ThrowIfCancellationRequested();
        if (process.ExitCode != 0)
        {
            var exception = new InvalidOperationException($"ExifTool failed ({process.ExitCode}): {error}");
            AppLogger.Error("ExifTool execution failed.", exception);
            throw exception;
        }
        return output;
    }

    private void EnsureAvailable()
    {
        if (!IsAvailable)
            throw new FileNotFoundException("ExifTool was not found. Configure its path in settings, put exiftool.exe in an exiftool folder next to ExifTweaker.exe, or add it to PATH.", _executable);
    }

    private static string ResolveExecutable()
    {
        var configured = AppSettings.Load().ExifToolPath;
        if (!string.IsNullOrWhiteSpace(configured)) return configured;
        var local = Path.Combine(AppContext.BaseDirectory, "exiftool", "exiftool.exe");
        return File.Exists(local) ? local : "exiftool";
    }

    private static bool ResolveAvailable(string executable)
    {
        if (Path.IsPathRooted(executable)) return File.Exists(executable);
        try
        {
            using var process = Process.Start(new ProcessStartInfo(executable, "-ver")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
            process?.WaitForExit(1500);
            return process?.ExitCode == 0;
        }
        catch { return false; }
    }

    private static void ValidateCoordinates(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(latitude), "GPS coordinates are outside their valid ranges.");
    }

    private static string FormatExifOffset(TimeSpan offset) => $"{(offset < TimeSpan.Zero ? "-" : "+")}{Math.Abs(offset.Hours):00}:{Math.Abs(offset.Minutes):00}";
    private static string? GetString(JsonElement element, string name) => element.TryGetProperty(name, out var property) ? property.ToString() : null;
    private static double? GetDouble(JsonElement element, string name) => element.TryGetProperty(name, out var property) && property.TryGetDouble(out var value) ? value : null;
    private static int? GetInt(JsonElement element, string name) => element.TryGetProperty(name, out var property) && property.TryGetInt32(out var value) ? value : null;
    private static DateTime? ParseExifDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var clean = value.Length >= 19 ? value[..19] : value;
        return DateTime.TryParseExact(clean, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed
            : DateTime.TryParse(value, out parsed) ? parsed : null;
    }
    private static TimeSpan? ParseOffset(string? value) => TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out var offset) ? offset : null;
}
