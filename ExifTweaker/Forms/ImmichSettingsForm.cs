using ExifTweaker.Infrastructure;
using ExifTweaker.Services;

namespace ExifTweaker.Forms;

public sealed class ImmichSettingsForm : Form
{
    private readonly AppSettings _settings;
    private readonly ISecretStore _secrets;
    private readonly CheckBox _enabled = new() { Text = "Activer l’intégration Immich", AutoSize = true };
    private readonly TextBox _url = new() { Dock = DockStyle.Fill, PlaceholderText = "https://photos.exemple.fr/api" };
    private readonly TextBox _key = new() { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
    private readonly CheckBox _showKey = new() { Text = "Afficher", AutoSize = true };
    private readonly TextBox _album = new() { Dock = DockStyle.Fill, PlaceholderText = "Facultatif" };
    private readonly ComboBox _visibility = new() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _concurrency = new() { Minimum = 1, Maximum = 8, Dock = DockStyle.Left };
    private readonly Button _test = new() { Text = "Tester la connexion", AutoSize = true };
    private readonly Label _status = new() { Text = "Connexion non testée", AutoSize = true, Padding = new Padding(0, 8, 0, 0) };

    public ImmichSettingsForm(AppSettings settings, ISecretStore? secrets = null)
    {
        _settings = settings;
        _secrets = secrets ?? new WindowsSecretStore();
        _visibility.Items.AddRange(new object[] { "Chronologie", "Archivé", "Masqué", "Verrouillé" });
        _enabled.Checked = settings.ImmichEnabled;
        _url.Text = settings.ImmichServerUrl;
        _key.Text = _secrets.Read("immich-api-key") ?? string.Empty;
        _album.Text = settings.ImmichDefaultAlbumName ?? string.Empty;
        _visibility.SelectedIndex = settings.ImmichDefaultVisibility switch { "archive" => 1, "hidden" => 2, "locked" => 3, _ => 0 };
        _concurrency.Value = Math.Clamp(settings.ImmichUploadConcurrency, 1, 8);
        BuildLayout();
        _enabled.CheckedChanged += (_, _) => UpdateEnabledState();
        _showKey.CheckedChanged += (_, _) => _key.UseSystemPasswordChar = !_showKey.Checked;
        _test.Click += async (_, _) => await TestConnectionAsync();
        UpdateEnabledState();
        ThemeService.Apply(this);
    }

    private void BuildLayout()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 3, RowCount = 9 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 175));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        for (var row = 0; row < 8; row++) root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.Controls.Add(_enabled, 0, 0);
        root.SetColumnSpan(_enabled, 3);
        AddRow(root, 1, "Adresse du serveur", _url);
        AddRow(root, 2, "Clé API", _key);
        root.Controls.Add(_showKey, 2, 2);
        AddRow(root, 3, "Album par défaut", _album);
        AddRow(root, 4, "Visibilité par défaut", _visibility);
        AddRow(root, 5, "Envois simultanés", _concurrency);
        root.Controls.Add(_test, 1, 6);
        root.Controls.Add(_status, 1, 7);
        var save = new Button { Text = "Enregistrer", AutoSize = true };
        var cancel = new Button { Text = "Annuler", AutoSize = true, DialogResult = DialogResult.Cancel };
        save.Click += (_, _) => SaveSettings();
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        buttons.Controls.AddRange(new Control[] { cancel, save });
        root.Controls.Add(buttons, 0, 8);
        root.SetColumnSpan(buttons, 3);
        Controls.Add(root);
        AcceptButton = save;
        CancelButton = cancel;
        ClientSize = new Size(700, 430);
        MinimumSize = new Size(600, 400);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Configuration Immich";
    }

    private static void AddRow(TableLayoutPanel root, int row, string label, Control control)
    {
        root.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, row);
        root.Controls.Add(control, 1, row);
    }

    private void UpdateEnabledState()
    {
        foreach (Control control in new Control[] { _url, _key, _showKey, _album, _visibility, _concurrency, _test })
            control.Enabled = _enabled.Checked;
    }

    private async Task TestConnectionAsync()
    {
        _test.Enabled = false;
        _status.Text = "Connexion…";
        try
        {
            using var client = new ImmichClient(_url.Text, _key.Text.Trim());
            var info = await client.GetServerInfoAsync(CancellationToken.None);
            var albums = await client.GetAlbumsAsync(CancellationToken.None);
            _status.Text = $"Connecté à {info.Name} — {info.Version} — {albums.Count} album(s)";
        }
        catch (Exception ex)
        {
            _status.Text = "Connexion impossible";
            AppLogger.Error("Immich connection test failed.", ex);
            ThemedMessageBox.Show(ex.Message, "Connexion Immich", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally { if (!IsDisposed) _test.Enabled = _enabled.Checked; }
    }

    private void SaveSettings()
    {
        if (_enabled.Checked)
        {
            try { _url.Text = ImmichClient.NormalizeServerUrl(_url.Text); }
            catch (ArgumentException ex)
            {
                ThemedMessageBox.Show(ex.Message, "Configuration Immich", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(_key.Text))
            {
                ThemedMessageBox.Show("Saisissez une clé API Immich.", "Configuration Immich", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }
        _settings.ImmichEnabled = _enabled.Checked;
        _settings.ImmichServerUrl = _url.Text.Trim();
        _settings.ImmichDefaultAlbumId = null;
        _settings.ImmichDefaultAlbumName = string.IsNullOrWhiteSpace(_album.Text) ? null : _album.Text.Trim();
        _settings.ImmichDefaultVisibility = _visibility.SelectedIndex switch { 1 => "archive", 2 => "hidden", 3 => "locked", _ => "timeline" };
        _settings.ImmichUploadConcurrency = (int)_concurrency.Value;
        _secrets.Write("immich-api-key", string.IsNullOrWhiteSpace(_key.Text) ? null : _key.Text.Trim());
        _settings.Save();
        DialogResult = DialogResult.OK;
    }
}
