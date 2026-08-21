namespace ExifTweaker.Services;

public sealed class FileDiscoveryService
{
    private static readonly HashSet<string> Extensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".heic", ".heif", ".png", ".tif", ".tiff", ".dng",
        ".cr2", ".cr3", ".nef", ".arw", ".raf", ".orf", ".rw2", ".raw",
        ".mov", ".mp4"
    };

    public IReadOnlyList<string> Discover(IEnumerable<string> paths)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in paths)
        {
            if (File.Exists(path) && IsSupported(path)) result.Add(Path.GetFullPath(path));
            else if (Directory.Exists(path))
            {
                try
                {
                    foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
                        if (IsSupported(file)) result.Add(Path.GetFullPath(file));
                }
                catch (UnauthorizedAccessException) { }
            }
        }
        return result.ToList();
    }

    public bool IsSupported(string path) => Extensions.Contains(Path.GetExtension(path));
    public string DialogFilter => "Photos & videos|" + string.Join(';', Extensions.Select(e => $"*{e}")) + "|All files|*.*";
}
