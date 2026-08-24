using System.Security.Cryptography;
using System.Text;

namespace ExifTweaker.Infrastructure;

public interface ISecretStore
{
    string? Read(string name);
    void Write(string name, string? value);
}

public sealed class WindowsSecretStore : ISecretStore
{
    private static string SecretDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ExifTweaker", "secrets");

    public string? Read(string name)
    {
        var environmentValue = name == "immich-api-key" ? Environment.GetEnvironmentVariable("EXIFTWEAKER_IMMICH_API_KEY") : null;
        if (!string.IsNullOrWhiteSpace(environmentValue)) return environmentValue;
        var path = GetPath(name);
        if (!File.Exists(path)) return null;
        try
        {
            var clear = ProtectedData.Unprotect(File.ReadAllBytes(path), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(clear);
        }
        catch (Exception ex)
        {
            AppLogger.Error($"Unable to read protected secret '{name}'.", ex);
            return null;
        }
    }

    public void Write(string name, string? value)
    {
        if (name == "immich-api-key" && Environment.GetEnvironmentVariable("EXIFTWEAKER_IMMICH_API_KEY") is not null) return;
        var path = GetPath(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            if (File.Exists(path)) File.Delete(path);
            return;
        }
        Directory.CreateDirectory(SecretDirectory);
        var encrypted = ProtectedData.Protect(Encoding.UTF8.GetBytes(value), null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(path, encrypted);
    }

    private static string GetPath(string name)
    {
        var safeName = string.Concat(name.Select(character => char.IsLetterOrDigit(character) || character == '-' ? character : '_'));
        return Path.Combine(SecretDirectory, $"{safeName}.bin");
    }
}
