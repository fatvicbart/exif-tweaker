using System.Reflection;
using ExifTweaker.Infrastructure;
using Velopack;
using Velopack.Sources;

namespace ExifTweaker.Services;

public sealed class UpdateService
{
    public const string RepositoryUrl = "https://github.com/fatvicbart/exif-tweaker";
    private readonly AppSettings _settings;

    public UpdateService(AppSettings settings) => _settings = settings;

    public string DisplayVersion => typeof(UpdateService).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
        ?.Split('+')[0] ?? Application.ProductVersion;

    private UpdateManager CreateManager() => new(
        new GithubSource(RepositoryUrl, accessToken: null, prerelease: _settings.IncludePrereleaseUpdates));

    public async Task CheckAndPromptAsync(IWin32Window owner, bool manual, CancellationToken cancellationToken = default)
    {
        try
        {
            var manager = CreateManager();
            if (!manager.IsInstalled)
            {
                if (manual)
                    MessageBox.Show(owner,
                        "La recherche de mises à jour est disponible après installation avec le Setup ExifTweaker.\n\n" +
                        "Une exécution depuis Visual Studio ou depuis l’ancienne archive ZIP n’est pas une installation Velopack.",
                        "Mises à jour ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var update = await manager.CheckForUpdatesAsync();
            cancellationToken.ThrowIfCancellationRequested();
            if (update is null)
            {
                if (manual)
                    MessageBox.Show(owner, $"ExifTweaker {DisplayVersion} est à jour.", "Mises à jour ExifTweaker",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var target = update.TargetFullRelease;
            var notes = string.IsNullOrWhiteSpace(target.NotesMarkdown)
                ? "Aucune note de version n’est disponible."
                : target.NotesMarkdown.Trim();
            if (notes.Length > 1800) notes = notes[..1800] + "\n…";

            var answer = MessageBox.Show(owner,
                $"Une nouvelle version d’ExifTweaker est disponible.\n\n" +
                $"Version installée : {manager.CurrentVersion ?? SemanticVersion.Parse(DisplayVersion)}\n" +
                $"Nouvelle version : {target.Version}\n" +
                $"Téléchargement : {FormatSize(target.Size)}\n\n" +
                $"Notes de version :\n{notes}\n\n" +
                "Télécharger et installer cette mise à jour maintenant ?",
                "Mise à jour ExifTweaker", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (answer != DialogResult.Yes) return;

            using var progress = new UpdateProgressForm(target.Version.ToString());
            ThemeService.Apply(progress, _settings.Theme);
            progress.Show(owner);
            progress.Refresh();

            try
            {
                await manager.DownloadUpdatesAsync(update, value =>
                {
                    if (progress.IsDisposed) return;
                    progress.BeginInvoke(() => progress.SetProgress(value));
                }, cancellationToken);
            }
            finally
            {
                if (!progress.IsDisposed) progress.Close();
            }

            var restart = MessageBox.Show(owner,
                "La mise à jour est téléchargée et vérifiée.\n\nExifTweaker doit maintenant redémarrer pour terminer l’installation.",
                "Mise à jour prête", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (restart == DialogResult.OK)
                manager.ApplyUpdatesAndRestart(update.TargetFullRelease);
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            AppLogger.Error("Update check failed.", ex);
            if (manual)
                MessageBox.Show(owner, "Impossible de rechercher ou télécharger la mise à jour.\n\n" + ex.Message,
                    "Mises à jour ExifTweaker", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private static string FormatSize(long bytes)
    {
        if (bytes <= 0) return "taille inconnue";
        var mb = bytes / 1024d / 1024d;
        return mb >= 1 ? $"{mb:0.0} Mo" : $"{bytes / 1024d:0} Ko";
    }
}

internal sealed class UpdateProgressForm : Form
{
    private readonly ProgressBar _bar = new() { Dock = DockStyle.Top, Height = 24, Minimum = 0, Maximum = 100 };
    private readonly Label _label = new() { Dock = DockStyle.Top, Height = 34, TextAlign = ContentAlignment.MiddleLeft };

    public UpdateProgressForm(string version)
    {
        Text = "Téléchargement de la mise à jour";
        ClientSize = new Size(440, 90);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ControlBox = false;
        _label.Text = $"Téléchargement d’ExifTweaker {version}…";
        Controls.Add(_bar);
        Controls.Add(_label);
        Padding = new Padding(12);
    }

    public void SetProgress(int value)
    {
        value = Math.Clamp(value, 0, 100);
        _bar.Value = value;
        _label.Text = $"Téléchargement de la mise à jour… {value}%";
    }
}
