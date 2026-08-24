namespace ExifTweaker.Models;

public enum ImmichAssetVisibility { Timeline, Archive, Hidden, Locked }
public enum ImmichUploadStatus { Pending, Uploading, Uploaded, Duplicate, Failed, Cancelled }

public sealed record ImmichAlbum(string Id, string Name);
public sealed record ImmichServerInfo(string Name, string Version);
public sealed record ImmichUploadRequest(
    IReadOnlyList<string> FilePaths,
    string? AlbumId,
    string? NewAlbumName,
    ImmichAssetVisibility Visibility,
    int Concurrency);

public sealed record ImmichUploadItemResult(
    string FilePath,
    ImmichUploadStatus Status,
    string? AssetId = null,
    string? Error = null);

public sealed record ImmichUploadProgress(
    string FilePath,
    ImmichUploadStatus Status,
    int Completed,
    int Total,
    string? Message = null);

public sealed record ImmichUploadResult(IReadOnlyList<ImmichUploadItemResult> Files)
{
    public int Uploaded => Files.Count(x => x.Status == ImmichUploadStatus.Uploaded);
    public int Duplicates => Files.Count(x => x.Status == ImmichUploadStatus.Duplicate);
    public int Failed => Files.Count(x => x.Status == ImmichUploadStatus.Failed);
    public int Cancelled => Files.Count(x => x.Status == ImmichUploadStatus.Cancelled);
}
