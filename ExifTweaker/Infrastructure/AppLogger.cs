using System.Diagnostics;
using System.Text.Json;

namespace ExifTweaker.Infrastructure;

public sealed record LogEntry(DateTimeOffset Timestamp, string Level, string Message, string? ExceptionType, string? ExceptionText, string RawJson, bool IsValid = true, long Sequence = 0);

public static class AppLogger
{
    private static readonly object Gate = new();
    public static string LogPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExifTweaker", "logs", "exiftweaker.jsonl");

    public static void Info(string message) => Write("info", message, null);
    public static void Error(string message, Exception exception) => Write("error", message, exception);

    public static async Task<IReadOnlyList<LogEntry>> ReadRecentAsync(int maximum = 2000, CancellationToken cancellationToken = default)
    {
        if (maximum <= 0 || !File.Exists(LogPath)) return Array.Empty<LogEntry>();
        var entries = new Queue<LogEntry>(maximum);
        await using var stream = new FileStream(LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.Asynchronous);
        using var reader = new StreamReader(stream);
        long sequence = 0;
        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            sequence++;
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (entries.Count == maximum) entries.Dequeue();
            entries.Enqueue(ParseLine(line) with { Sequence = sequence });
        }
        return entries.Reverse().ToList();
    }

    internal static LogEntry ParseLine(string line)
    {
        try
        {
            using var document = JsonDocument.Parse(line);
            var root = document.RootElement;
            var timestamp = root.TryGetProperty("timestamp", out var time) && time.TryGetDateTimeOffset(out var parsed) ? parsed : DateTimeOffset.MinValue;
            return new LogEntry(
                timestamp,
                root.TryGetProperty("level", out var level) ? level.GetString() ?? "info" : "info",
                root.TryGetProperty("message", out var message) ? message.GetString() ?? string.Empty : string.Empty,
                root.TryGetProperty("exceptionType", out var type) && type.ValueKind != JsonValueKind.Null ? type.GetString() : null,
                root.TryGetProperty("exception", out var exception) && exception.ValueKind != JsonValueKind.Null ? exception.GetString() : null,
                line);
        }
        catch (JsonException)
        {
            return new LogEntry(DateTimeOffset.MinValue, "invalid", "Entrée de journal illisible", null, line, line, false);
        }
    }

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
