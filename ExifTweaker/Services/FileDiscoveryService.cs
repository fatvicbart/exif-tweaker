namespace ExifTweaker.Services;

public sealed record FileDiscoveryResult(IReadOnlyList<string> Files, IReadOnlyList<string> Errors);

public sealed class FileDiscoveryService
{
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".heic", ".heif", ".png", ".tif", ".tiff", ".dng",
        ".cr2", ".cr3", ".nef", ".arw", ".raf", ".orf", ".rw2", ".raw", ".mov", ".mp4"
    };

    public Task<FileDiscoveryResult> DiscoverAsync(IEnumerable<string> paths, bool recursive, CancellationToken ct = default) =>
        Task.Run(() => DiscoverCore(paths, recursive, ct), ct);

    public IReadOnlyList<string> Discover(IEnumerable<string> paths) => DiscoverCore(paths, true, CancellationToken.None).Files;
    public bool IsSupported(string path) => Extensions.Contains(Path.GetExtension(path));
    public string DialogFilter => "Photos & videos|" + string.Join(";", Extensions.Select(extension => $"*{extension}")) + "|All files|*.*";

    private FileDiscoveryResult DiscoverCore(IEnumerable<string> paths, bool recursive, CancellationToken ct)
    {
        var files = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();
        foreach (var path in paths)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                if (File.Exists(path)) { AddIfSupported(path, files); continue; }
                if (!Directory.Exists(path)) { errors.Add($"Path not found: {path}"); continue; }
                var pending = new Stack<string>();
                pending.Push(path);
                while (pending.Count > 0)
                {
                    ct.ThrowIfCancellationRequested();
                    var directory = pending.Pop();
                    try
                    {
                        foreach (var file in Directory.EnumerateFiles(directory)) { ct.ThrowIfCancellationRequested(); AddIfSupported(file, files); }
                        if (recursive) foreach (var child in Directory.EnumerateDirectories(directory)) pending.Push(child);
                    }
                    catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or System.Security.SecurityException)
                    { errors.Add($"Cannot read {directory}: {ex.Message}"); }
                }
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or IOException or System.Security.SecurityException)
            { errors.Add($"Cannot access {path}: {ex.Message}"); }
        }
        return new FileDiscoveryResult(files.ToList(), errors);
    }

    private void AddIfSupported(string path, ISet<string> files)
    {
        if (IsSupported(path)) files.Add(Path.GetFullPath(path));
    }
}
