using ExifTweaker.Models;
using ExifTweaker.Services;

namespace ExifTweaker.Tests;

[TestClass]
public sealed class ImmichUploadTests
{
    [TestMethod]
    public async Task UploadAsync_ClassifiesCreatedAndDuplicate_AndAddsBothToAlbum()
    {
        using var client = new FakeImmichClient
        {
            Uploads =
            {
                ["one.jpg"] = ("asset-1", false),
                ["two.jpg"] = ("asset-2", true)
            }
        };
        var service = new ImmichUploadService(client);
        var request = new ImmichUploadRequest(["one.jpg", "two.jpg"], "album-1", null, ImmichAssetVisibility.Timeline, 2);

        var result = await service.UploadAsync(request, null, CancellationToken.None);

        Assert.AreEqual(1, result.Uploaded);
        Assert.AreEqual(1, result.Duplicates);
        Assert.AreEqual(0, result.Failed);
        CollectionAssert.AreEquivalent(new[] { "asset-1", "asset-2" }, client.AlbumAssetIds.ToArray());
    }

    [TestMethod]
    public async Task UploadAsync_CreatesRequestedAlbum_AndRetainsItForRetry()
    {
        using var client = new FakeImmichClient();
        client.Uploads["photo.jpg"] = ("asset-1", false);
        var service = new ImmichUploadService(client);
        var request = new ImmichUploadRequest(["photo.jpg"], null, "Voyage", ImmichAssetVisibility.Archive, 3);

        var result = await service.UploadAsync(request, null, CancellationToken.None);

        Assert.AreEqual("Voyage", client.CreatedAlbumName);
        Assert.AreEqual("new-album", service.LastResolvedAlbumId);
        Assert.AreEqual(1, result.Uploaded);
        CollectionAssert.AreEqual(new[] { "asset-1" }, client.AlbumAssetIds.ToArray());
    }

    [TestMethod]
    public async Task UploadAsync_IsolatesIndividualFailures()
    {
        using var client = new FakeImmichClient();
        client.Uploads["ok.jpg"] = ("asset-ok", false);
        client.Failures.Add("bad.jpg");
        var service = new ImmichUploadService(client);
        var request = new ImmichUploadRequest(["ok.jpg", "bad.jpg"], null, null, ImmichAssetVisibility.Hidden, 2);

        var result = await service.UploadAsync(request, null, CancellationToken.None);

        Assert.AreEqual(1, result.Uploaded);
        Assert.AreEqual(1, result.Failed);
        StringAssert.Contains(result.Files.Single(file => file.FilePath == "bad.jpg").Error, "simulated");
    }

    [TestMethod]
    public void NormalizeServerUrl_AddsApiPathAndTrailingSlash()
    {
        Assert.AreEqual("https://photos.example.test/api/", ImmichClient.NormalizeServerUrl("https://photos.example.test"));
        Assert.AreEqual("http://localhost:2283/api/", ImmichClient.NormalizeServerUrl("http://localhost:2283/api/"));
        Assert.ThrowsExactly<ArgumentException>(() => ImmichClient.NormalizeServerUrl("not-an-url"));
    }

    private sealed class FakeImmichClient : IImmichClient
    {
        public Dictionary<string, (string AssetId, bool Duplicate)> Uploads { get; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> Failures { get; } = new(StringComparer.OrdinalIgnoreCase);
        public List<string> AlbumAssetIds { get; } = [];
        public string? CreatedAlbumName { get; private set; }

        public Task<ImmichServerInfo> GetServerInfoAsync(CancellationToken ct) =>
            Task.FromResult(new ImmichServerInfo("Test", "v3"));

        public Task<IReadOnlyList<ImmichAlbum>> GetAlbumsAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<ImmichAlbum>>([]);

        public Task<ImmichAlbum> CreateAlbumAsync(string name, CancellationToken ct)
        {
            CreatedAlbumName = name;
            return Task.FromResult(new ImmichAlbum("new-album", name));
        }

        public Task<(string AssetId, bool Duplicate)> UploadAssetAsync(string filePath, ImmichAssetVisibility visibility, CancellationToken ct)
        {
            if (Failures.Contains(filePath)) throw new InvalidOperationException("simulated upload failure");
            return Task.FromResult(Uploads[filePath]);
        }

        public Task AddAssetsToAlbumAsync(string albumId, IReadOnlyList<string> assetIds, CancellationToken ct)
        {
            AlbumAssetIds.AddRange(assetIds);
            return Task.CompletedTask;
        }

        public void Dispose() { }
    }
}
