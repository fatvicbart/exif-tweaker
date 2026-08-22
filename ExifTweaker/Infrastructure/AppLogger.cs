using System.Diagnostics;
using System.Text.Json;

namespace ExifTweaker.Infrastructure;

public static class AppLogger
{
    private static readonly object Gate = new();
    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExifTweaker", "logs", "exiftweaker.jsonl");

    public static void Info(string message) => Write("info", message, null);
    public static void Error(string message, Exception exception) => Write("error", message, exception);

    private static void Write(string level, string message, Exception? exception)
    {
        var entry = new
        {
            timestamp = DateTimeOffset.Now,
            level,
            message,
            exceptionType = exception?.GetType().FullName,
            exception = exception?.ToString()
        };
        var line = JsonSerializer.Serialize(entry);
        Trace.WriteLine(line);
        try
        {
            lock (Gate)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
                File.AppendAllText(LogPath, line + Environment.NewLine);
            }
        }
        catch (Exception logError) { Trace.WriteLine($"ExifTweaker logging failure: {logError.Message}"); }
    }
}
