using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using ExifTweaker.Infrastructure;

namespace ExifTweaker.Services;

public sealed class ThumbnailService : IDisposable
{
    private readonly ConcurrentDictionary<string, Image> _memory = new(StringComparer.OrdinalIgnoreCase);
    private readonly ExifToolService _exifTool;
    private readonly AppSettings _settings;

    public ThumbnailService(ExifToolService exifTool, AppSettings settings)
    {
        _exifTool = exifTool;
        _settings = settings;
    }

    public async Task<Image> GetAsync(string path, int maximumSize, CancellationToken ct = default)
    {
        var key = CacheKey(path, maximumSize);
        if (_memory.TryGetValue(key, out var cached)) return (Image)cached.Clone();

        var image = await Task.Run(() => LoadDiskCache(key), ct);
        if (image is null) image = await LoadSourceAsync(path, maximumSize, ct);
        image ??= CreatePlaceholder(maximumSize, Path.GetExtension(path).TrimStart('.').ToUpperInvariant());

        _memory[key] = (Image)image.Clone();
        TrimMemoryCache(key);
        if (_settings.ThumbnailDiskCache) await Task.Run(() => SaveDiskCache(key, image), ct);
        return image;
    }

    public void Invalidate(string path)
    {
        var prefix = PathKey(path);
        foreach (var pair in _memory.Where(pair => pair.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)).ToList())
            if (_memory.TryRemove(pair.Key, out var image)) image.Dispose();
        if (!Directory.Exists(AppSettings.CacheDirectory)) return;
        foreach (var file in Directory.EnumerateFiles(AppSettings.CacheDirectory, prefix + "-*.png"))
            try { File.Delete(file); } catch (IOException) { }
    }

    public void Dispose()
    {
        foreach (var image in _memory.Values) image.Dispose();
        _memory.Clear();
    }

    private void TrimMemoryCache(string retainedKey)
    {
        const int maximumEntries = 500;
        if (_memory.Count <= maximumEntries) return;
        foreach (var key in _memory.Keys.Where(key => !key.Equals(retainedKey, StringComparison.OrdinalIgnoreCase)).Take(_memory.Count - maximumEntries))
            if (_memory.TryRemove(key, out var image)) image.Dispose();
    }

    private async Task<Image?> LoadSourceAsync(string path, int maximumSize, CancellationToken ct)
    {
        try
        {
            return await Task.Run(() =>
            {
                using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var source = Image.FromStream(stream);
                using var oriented = ApplyOrientation(source);
                return (Image)new Bitmap(oriented, Scale(oriented.Size, maximumSize));
            }, ct);
        }
        catch (Exception ex) when (ex is ArgumentException or OutOfMemoryException or IOException)
        {
            try
            {
                var bytes = await _exifTool.ExtractPreviewAsync(path, ct);
                if (bytes is null) return null;
                using var stream = new MemoryStream(bytes);
                using var source = Image.FromStream(stream);
                return new Bitmap(source, Scale(source.Size, maximumSize));
            }
            catch (Exception previewError) when (previewError is not OperationCanceledException)
            {
                AppLogger.Info($"No preview available for {path}: {previewError.Message}");
                return null;
            }
        }
    }

    private Image? LoadDiskCache(string key)
    {
        if (!_settings.ThumbnailDiskCache) return null;
        var file = Path.Combine(AppSettings.CacheDirectory, key + ".png");
        if (!File.Exists(file)) return null;
        try
        {
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var source = Image.FromStream(stream);
            return new Bitmap(source);
        }
        catch (Exception ex) when (ex is ArgumentException or IOException) { return null; }
    }

    private void SaveDiskCache(string key, Image image)
    {
        if (!_settings.ThumbnailDiskCache) return;
        try
        {
            Directory.CreateDirectory(AppSettings.CacheDirectory);
            var file = Path.Combine(AppSettings.CacheDirectory, key + ".png");
            if (!File.Exists(file)) image.Save(file, ImageFormat.Png);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Runtime.InteropServices.ExternalException)
        {
            AppLogger.Info($"Thumbnail cache write skipped: {ex.Message}");
        }
    }

    private static Image ApplyOrientation(Image source)
    {
        var clone = new Bitmap(source);
        const int orientationId = 0x0112;
        if (!source.PropertyIdList.Contains(orientationId)) return clone;
        var property = source.GetPropertyItem(orientationId);
        if (property?.Value is not { Length: > 0 } values) return clone;
        var orientation = values[0];
        var flip = orientation switch
        {
            2 => RotateFlipType.RotateNoneFlipX,
            3 => RotateFlipType.Rotate180FlipNone,
            4 => RotateFlipType.Rotate180FlipX,
            5 => RotateFlipType.Rotate90FlipX,
            6 => RotateFlipType.Rotate90FlipNone,
            7 => RotateFlipType.Rotate270FlipX,
            8 => RotateFlipType.Rotate270FlipNone,
            _ => RotateFlipType.RotateNoneFlipNone
        };
        clone.RotateFlip(flip);
        return clone;
    }

    private static Image CreatePlaceholder(int maximumSize, string label)
    {
        var side = Math.Clamp(maximumSize, 64, 640);
        var bitmap = new Bitmap(side, side);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.FromArgb(48, 52, 58));
        TextRenderer.DrawText(graphics, string.IsNullOrWhiteSpace(label) ? "MEDIA" : label, SystemFonts.MessageBoxFont,
            new Rectangle(0, 0, side, side), Color.Gainsboro, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        return bitmap;
    }

    private static string CacheKey(string path, int maximumSize)
    {
        var stamp = File.Exists(path) ? File.GetLastWriteTimeUtc(path).Ticks : 0;
        return $"{PathKey(path)}-{stamp}-{maximumSize}";
    }

    private static string PathKey(string path) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Path.GetFullPath(path))))[..20];

    private static Size Scale(Size size, int maximum)
    {
        if (size.Width <= maximum && size.Height <= maximum) return size;
        var ratio = Math.Min((double)maximum / size.Width, (double)maximum / size.Height);
        return new Size(Math.Max(1, (int)(size.Width * ratio)), Math.Max(1, (int)(size.Height * ratio)));
    }
}
