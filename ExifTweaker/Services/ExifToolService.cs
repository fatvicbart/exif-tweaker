using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using ExifTweaker.Infrastructure;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed class ExifToolService
{
    private const int ReadBatchSize = 100;
    private readonly string _executable;

    public ExifToolService(string? executable = null, string? applicationBaseDirectory = null) =>
        _executable = ResolveExecutable(executable ?? AppSettings.Load().ExifToolPath, applicationBaseDirectory);

    public string ExecutablePath => _executable;
    public bool IsAvailable => ResolveAvailable(_executable);

    public async Task<string> GetVersionAsync(CancellationToken ct = default)
    {
        var version = (await RunAsync(new[] { "-ver" }, ct)).Trim();
        if (string.IsNullOrWhiteSpace(version))
            throw new InvalidOperationException("ExifTool returned an empty version.");
        return version;
    }

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

    public async Task<string?> WriteAsync(PhotoItem item, bool backupOriginal = true, CancellationToken ct = default)
    {
        EnsureAvailable();
        var patch = item.PendingChanges;
        if (!patch.HasChanges) return null;

        var args = new List<string>();
        if (!backupOriginal || File.Exists(item.FilePath + "_original")) args.Add("-overwrite_original");
        var isVideo = IsVideo(item.FilePath);

        if (patch.HasDateChange)
        {
            if (item.EffectiveCaptureDate is not DateTime date)
                throw new InvalidOperationException("A date patch requires a capture date.");
            if (isVideo)
            {
                var utc = item.EffectiveOffset is TimeSpan offset
                    ? new DateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Unspecified), offset).UtcDateTime
                    : DateTime.SpecifyKind(date, DateTimeKind.Utc);
                var value = utc.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                args.Add("-api"); args.Add("QuickTimeUTC=1");
                args.Add($"-QuickTime:CreateDate={value}");
                args.Add($"-QuickTime:ModifyDate={value}");
                args.Add($"-TrackCreateDate={value}");
                args.Add($"-TrackModifyDate={value}");
                args.Add($"-MediaCreateDate={value}");
                args.Add($"-MediaModifyDate={value}");
            }
            else
            {
                var value = date.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                args.Add($"-DateTimeOriginal={value}");
                args.Add($"-CreateDate={value}");
                args.Add($"-ModifyDate={value}");
            }
        }

        if (patch.RemoveOffsetTimeOriginal) args.Add("-ExifIFD:OffsetTimeOriginal=");
        else if (patch.OffsetTimeOriginal is TimeSpan offset)
            args.Add($"-ExifIFD:OffsetTimeOriginal={FormatExifOffset(offset)}");

        if (patch.RemoveLocation)
        {
            if (isVideo) args.Add("-QuickTime:GPSCoordinates=");
            args.Add("-GPSLatitude="); args.Add("-GPSLongitude=");
            args.Add("-GPSLatitudeRef="); args.Add("-GPSLongitudeRef=");
            args.Add("-GPSAltitude="); args.Add("-GPSAltitudeRef=");
        }
        else if (patch.HasLocationChange && item.EffectiveLatitude is double lat && item.EffectiveLongitude is double lon)
        {
            ValidateCoordinates(lat, lon);
            if (isVideo)
            {
                var altitude = item.EffectiveAltitude ?? 0d;
                args.Add($"-QuickTime:GPSCoordinates={lat.ToString(CultureInfo.InvariantCulture)} {lon.ToString(CultureInfo.InvariantCulture)} {altitude.ToString(CultureInfo.InvariantCulture)}");
            }
            args.Add($"-GPSLatitude={lat.ToString(CultureInfo.InvariantCulture)}");
            args.Add($"-GPSLongitude={lon.ToString(CultureInfo.InvariantCulture)}");
            args.Add($"-GPSLatitudeRef={(lat < 0 ? "S" : "N")}");
            args.Add($"-GPSLongitudeRef={(lon < 0 ? "W" : "E")}");
            if (patch.RemoveAltitude)
            {
                args.Add("-GPSAltitude=");
                args.Add("-GPSAltitudeRef=");
            }
            else if (item.EffectiveAltitude is double altitude)
            {
                args.Add($"-GPSAltitude={Math.Abs(altitude).ToString(CultureInfo.InvariantCulture)}");
                args.Add($"-GPSAltitudeRef={(altitude < 0 ? 1 : 0)}");
            }
        }

        args.Add(Path.GetFullPath(item.FilePath));
        string? warning = null;
        await RunAsync(args, ct, value => warning = value);
        return warning;
    }

    public async Task<byte[]?> ExtractPreviewAsync(string filePath, CancellationToken ct = default)
    {
        EnsureAvailable();
        foreach (var tag in new[] { "-PreviewImage", "-JpgFromRaw", "-ThumbnailImage" })
        {
            var bytes = await RunBinaryAsync(new[] { "-b", tag, Path.GetFullPath(filePath) }, ct);
            if (bytes.Length > 0) return bytes;
        }
        return null;
    }

    public Task RestoreBackupAsync(PhotoItem item, CancellationToken ct = default) => Task.Run(() =>
    {
        var backup = item.FilePath + "_original";
        if (!File.Exists(backup)) throw new FileNotFoundException("ExifTool backup was not found.", backup);
        ct.ThrowIfCancellationRequested();
        var temporary = item.FilePath + ".restore-" + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            File.Copy(backup, temporary, overwrite: false);
            ct.ThrowIfCancellationRequested();
            File.Move(temporary, item.FilePath, overwrite: true);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }, ct);

    private async Task<IReadOnlyDictionary<string, PhotoMetadata>> ReadBatchAsync(IEnumerable<string> files, CancellationToken ct)
    {
        var args = new List<string>
        {
            "-json", "-n", "-api", "QuickTimeUTC=1",
            "-DateTimeOriginal", "-CreateDate", "-MediaCreateDate", "-TrackCreateDate", "-OffsetTimeOriginal",
            "-GPSLatitude", "-GPSLongitude", "-GPSAltitude", "-Make", "-Model", "-LensModel", "-Orientation",
            "-ImageWidth", "-ImageHeight", "-FileType", "-MIMEType", "-FileCreateDate", "-FileModifyDate", "-City", "-Country"
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
                CaptureDate = ParseExifDate(GetString(item, "DateTimeOriginal") ?? GetString(item, "CreateDate") ?? GetString(item, "MediaCreateDate") ?? GetString(item, "TrackCreateDate")),
                Offset = ParseOffset(GetString(item, "OffsetTimeOriginal")),
                Latitude = GetDouble(item, "GPSLatitude"), Longitude = GetDouble(item, "GPSLongitude"), Altitude = GetDouble(item, "GPSAltitude"),
                CameraMake = GetString(item, "Make"), CameraModel = GetString(item, "Model"), Lens = GetString(item, "LensModel"),
                Orientation = GetInt(item, "Orientation"),
                Width = GetInt(item, "ImageWidth"), Height = GetInt(item, "ImageHeight"), FileType = GetString(item, "FileType"), MimeType = GetString(item, "MIMEType"),
                FileCreateDate = ParseExifDate(GetString(item, "FileCreateDate")), FileModifyDate = ParseExifDate(GetString(item, "FileModifyDate")),
                City = GetString(item, "City"), Country = GetString(item, "Country")
            };
        }
        return result;
    }

    private async Task<byte[]> RunBinaryAsync(IEnumerable<string> arguments, CancellationToken ct)
    {
        using var argumentFile = CreateArgumentFile(arguments);
        var startInfo = CreateStartInfo(argumentFile);
        using var process = new Process { StartInfo = startInfo };
        if (!process.Start()) throw new InvalidOperationException("ExifTool could not be started.");
        using var registration = ct.Register(() =>
        {
            try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { }
        });
        await using var memory = new MemoryStream();
        var outputTask = process.StandardOutput.BaseStream.CopyToAsync(memory, ct);
        var errorTask = process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);
        await outputTask;
        var error = await errorTask;
        ct.ThrowIfCancellationRequested();
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"ExifTool preview extraction failed ({process.ExitCode}): {error}");
        return memory.ToArray();
    }

    private async Task<string> RunAsync(IEnumerable<string> arguments, CancellationToken ct, Action<string>? warningSink = null)
    {
        using var argumentFile = CreateArgumentFile(arguments);
        var startInfo = CreateStartInfo(argumentFile);
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
        if (!string.IsNullOrWhiteSpace(error))
        {
            var warning = error.Trim();
            AppLogger.Info($"ExifTool warning: {warning}");
            warningSink?.Invoke(warning);
        }
        return output;
    }

    private ProcessStartInfo CreateStartInfo(ArgumentFile argumentFile)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _executable,
            WorkingDirectory = argumentFile.DirectoryPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("-charset");
        startInfo.ArgumentList.Add("filename=UTF8");
        startInfo.ArgumentList.Add("-@");
        startInfo.ArgumentList.Add(argumentFile.FileName);
        return startInfo;
    }

    private static ArgumentFile CreateArgumentFile(IEnumerable<string> arguments)
    {
        var directory = Path.Combine(Path.GetTempPath(), "ExifTweaker", "arguments");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, $"args-{Guid.NewGuid():N}.txt");
        File.WriteAllBytes(path, EncodeArgumentFile(arguments));
        return new ArgumentFile(path);
    }

    internal static byte[] EncodeArgumentFile(IEnumerable<string> arguments)
    {
        var values = arguments.ToList();
        if (values.Any(value => value.Contains('\r') || value.Contains('\n')))
            throw new ArgumentException("ExifTool arguments cannot contain line breaks.", nameof(arguments));
        return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
            .GetBytes(string.Join("\n", values) + "\n");
    }

    private sealed class ArgumentFile : IDisposable
    {
        public ArgumentFile(string path) => Path = path;
        public string Path { get; }
        public string DirectoryPath => System.IO.Path.GetDirectoryName(Path)!;
        public string FileName => System.IO.Path.GetFileName(Path);

        public void Dispose()
        {
            try { File.Delete(Path); }
            catch (IOException ex) { AppLogger.Info($"ExifTool argument file cleanup skipped: {ex.Message}"); }
            catch (UnauthorizedAccessException ex) { AppLogger.Info($"ExifTool argument file cleanup skipped: {ex.Message}"); }
        }
    }

    private void EnsureAvailable()
    {
        if (!IsAvailable)
            throw new FileNotFoundException($"ExifTool could not be executed from '{_executable}'. Configure its path in settings, use the bundled distribution, or add it to PATH.", _executable);
    }

    internal static string ResolveExecutable(string? configuredPath = null, string? applicationBaseDirectory = null)
    {
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            var configured = Environment.ExpandEnvironmentVariables(configuredPath.Trim());
            if (Directory.Exists(configured))
                return Path.Combine(configured, "exiftool.exe");
            return configured;
        }

        var baseDirectory = applicationBaseDirectory ?? AppContext.BaseDirectory;
        var bundled = Path.Combine(baseDirectory, "exiftool", "exiftool.exe");
        return File.Exists(bundled)
            ? bundled
            : OperatingSystem.IsWindows() ? "exiftool.exe" : "exiftool";
    }

    internal static bool ResolveAvailable(string executable)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = executable,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("-ver");
            using var process = Process.Start(startInfo);
            if (process is null) return false;
            if (!process.WaitForExit(5000))
            {
                try { process.Kill(entireProcessTree: true); } catch (InvalidOperationException) { }
                return false;
            }
            var version = process.StandardOutput.ReadToEnd().Trim();
            _ = process.StandardError.ReadToEnd();
            return process.ExitCode == 0 && !string.IsNullOrWhiteSpace(version);
        }
        catch { return false; }
    }

    private static void ValidateCoordinates(double latitude, double longitude)
    {
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            throw new ArgumentOutOfRangeException(nameof(latitude), "GPS coordinates are outside their valid ranges.");
    }

    private static bool IsVideo(string path) => Path.GetExtension(path) is var extension && (extension.Equals(".mov", StringComparison.OrdinalIgnoreCase) || extension.Equals(".mp4", StringComparison.OrdinalIgnoreCase));
    private static string FormatExifOffset(TimeSpan offset) => $"{(offset < TimeSpan.Zero ? "-" : "+")}{Math.Abs((int)offset.TotalHours):00}:{Math.Abs(offset.Minutes):00}";
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
    internal static TimeSpan? ParseOffset(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.StartsWith('+')) normalized = normalized[1..];
        return TimeSpan.TryParse(normalized, CultureInfo.InvariantCulture, out var offset) ? offset : null;
    }
}
