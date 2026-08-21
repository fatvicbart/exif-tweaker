using System.Collections.Concurrent;
using System.Drawing;

namespace ExifTweaker.Services;

public sealed class ThumbnailService : IDisposable
{
    private readonly ConcurrentDictionary<string, Image> _memory = new(StringComparer.OrdinalIgnoreCase);

    public Task<Image?> GetAsync(string path, int maximumSize, CancellationToken ct = default) => Task.Run(() =>
    {
        ct.ThrowIfCancellationRequested();
        if (_memory.TryGetValue(path, out var cached)) return (Image)cached.Clone();
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var source = Image.FromStream(stream);
            var image = new Bitmap(source, Scale(source.Size, maximumSize));
            _memory[path] = (Image)image.Clone();
            return (Image)image;
        }
        catch { return null; }
    }, ct);

    public void Invalidate(string path)
    {
        if (_memory.TryRemove(path, out var image)) image.Dispose();
    }

    public void Dispose()
    {
        foreach (var image in _memory.Values) image.Dispose();
        _memory.Clear();
    }

    private static Size Scale(Size size, int maximum)
    {
        if (size.Width <= maximum && size.Height <= maximum) return size;
        var ratio = Math.Min((double)maximum / size.Width, (double)maximum / size.Height);
        return new Size((int)(size.Width * ratio), (int)(size.Height * ratio));
    }
}
