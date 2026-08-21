using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed class ExifToolService
{
    private readonly string _executable;
    public ExifToolService(string? executable = null) => _executable = executable ?? ResolveExecutable();
    public bool IsAvailable => ResolveAvailable(_executable);

    public async Task<IReadOnlyDictionary<string, PhotoMetadata>> ReadAsync(IEnumerable<string> files, CancellationToken ct = default)
    {
        var list = files.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (list.Count == 0) return new Dictionary<string, PhotoMetadata>();
        EnsureAvailable();
        var args = new List<string> { "-json", "-n", "-charset", "filename=UTF8",
            "-DateTimeOriginal", "-CreateDate", "-OffsetTimeOriginal", "-GPSLatitude", "-GPSLongitude", "-GPSAltitude",
            "-Make", "-Model", "-LensModel", "-ImageWidth", "-ImageHeight", "-FileType", "-MIMEType", "-FileCreateDate", "-FileModifyDate" };
        args.AddRange(list);
        var output = await RunAsync(args, ct);
        using var doc = JsonDocument.Parse(output);
        var result = new Dictionary<string, PhotoMetadata>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in doc.RootElement.EnumerateArray())
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
                FileCreateDate = ParseExifDate(GetString(item, "FileCreateDate")), FileModifyDate = ParseExifDate(GetString(item, "FileModifyDate"))
            };
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
        if (item.EffectiveCaptureDate is DateTime date)
        {
            var value = date.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
            args.Add($"-DateTimeOriginal={value}"); args.Add($"-CreateDate={value}"); args.Add($"-ModifyDate={value}");
        }
        if (patch.RemoveLocation)
        {
            args.Add("-GPSLatitude="); args.Add("-GPSLongitude="); args.Add("-GPSLatitudeRef="); args.Add("-GPSLongitudeRef="); args.Add("-GPSAltitude=");
        }
        else if (item.EffectiveLatitude is double lat && item.EffectiveLongitude is double lon)
        {
            args.Add($"-GPSLatitude={lat.ToString(CultureInfo.InvariantCulture)}"); args.Add($"-GPSLongitude={lon.ToString(CultureInfo.InvariantCulture)}");
            args.Add($"-GPSLatitudeRef={(lat < 0 ? "S" : "N")}"); args.Add($"-GPSLongitudeRef={(lon < 0 ? "W" : "E")}");
        }
        args.Add(item.FilePath);
        await RunAsync(args, ct);
    }

    private async Task<string> RunAsync(IEnumerable<string> arguments, CancellationToken ct)
    {
        var psi = new ProcessStartInfo { FileName = _executable, UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true };
        foreach (var arg in arguments) psi.ArgumentList.Add(arg);
        using var process = new Process { StartInfo = psi };
        process.Start();
        var stdout = process.StandardOutput.ReadToEndAsync(ct); var stderr = process.StandardError.ReadToEndAsync(ct);
        await process.WaitForExitAsync(ct);
        var output = await stdout; var error = await stderr;
        if (process.ExitCode != 0) throw new InvalidOperationException($"ExifTool failed ({process.ExitCode}): {error}");
        return output;
    }

    private void EnsureAvailable() { if (!IsAvailable) throw new FileNotFoundException("ExifTool was not found. Put exiftool.exe in an 'exiftool' folder next to ExifTweaker.exe, or add exiftool to PATH.", _executable); }
    private static string ResolveExecutable()
    {
        var local = Path.Combine(AppContext.BaseDirectory, "exiftool", "exiftool.exe");
        return File.Exists(local) ? local : "exiftool";
    }
    private static bool ResolveAvailable(string exe)
    {
        if (Path.IsPathRooted(exe)) return File.Exists(exe);
        try { using var p = Process.Start(new ProcessStartInfo(exe, "-ver") { UseShellExecute = false, RedirectStandardOutput = true, CreateNoWindow = true }); p?.WaitForExit(1500); return p?.ExitCode == 0; } catch { return false; }
    }
    private static string? GetString(JsonElement e, string n) => e.TryGetProperty(n, out var p) ? p.ToString() : null;
    private static double? GetDouble(JsonElement e, string n) => e.TryGetProperty(n, out var p) && p.TryGetDouble(out var v) ? v : null;
    private static int? GetInt(JsonElement e, string n) => e.TryGetProperty(n, out var p) && p.TryGetInt32(out var v) ? v : null;
    private static DateTime? ParseExifDate(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        var clean = s.Length >= 19 ? s[..19] : s;
        return DateTime.TryParseExact(clean, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d) ? d : DateTime.TryParse(s, out d) ? d : null;
    }
    private static TimeSpan? ParseOffset(string? s) => TimeSpan.TryParse(s, CultureInfo.InvariantCulture, out var o) ? o : null;
}
