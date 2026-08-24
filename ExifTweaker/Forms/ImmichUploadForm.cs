using ExifTweaker.Infrastructure;
using ExifTweaker.Models;

namespace ExifTweaker.Forms;

public sealed class ImmichUploadForm : Form
{
    private const string NoAlbum = "Aucun album";
    private const string NewAlbum = "Créer un nouvel album…";
    private readonly IReadOnlyList<ImmichAlbum> _albums;
    private readonly ComboBox _album = new() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly TextBox _newAlbum = new() { Dock = DockStyle.Fill, PlaceholderText = "Nom du nouvel album" };
    private readonly ComboBox _visibility = new() { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly NumericUpDown _concurrency = new() { Minimum = 1, Maximum = 8, Dock = DockStyle.Left };
    private readonly CheckBox _applyFirst = new() { Text = "Appliquer les modifications EXIF avant l’envoi", AutoSize = true, Checked = true };

    public ImmichUploadRequest? Request { get; private set; }
    public bool ApplyBeforeUpload => _applyFirst.Checked;

    public ImmichUploadForm(IReadOnlyList<PhotoItem> photos, IReadOnlyList<ImmichAlbum> albums, AppSettings settings, ImmichServerInfo server)
    {
        _albums = albums;
        _album.Items.Add(NoAlbum);
        foreach (var album in albums) _album.Items.Add(album);
        _album.Items.Add(NewAlbum);
        _album.Format += (_, e) => { if (e.ListItem is ImmichAlbum item) e.Value = item.Name; };
        _visibility.Items.AddRange(new object[] { "Chronologie", "Archivé", "Masqué", "Verrouillé" });
        _visibility.SelectedIndex = settings.ImmichDefaultVisibility switch { "archive" => 1, "hidden" => 2, "locked" => 3, _ => 0 };
        _concurrency.Value = Math.Clamp(settings.ImmichUploadConcurrency, 1, 8);

        var defaultAlbum = albums.FirstOrDefault(album => album.Id == settings.ImmichDefaultAlbumId || album.Name.Equals(settings.ImmichDefaultAlbumName, StringComparison.CurrentCultureIgnoreCase));
        if (defaultAlbum is not null) _album.SelectedItem = defaultAlbum;
        else if (!string.IsNullOrWhiteSpace(settings.ImmichDefaultAlbumName))
        {
            _album.SelectedItem = NewAlbum;
            _newAlbum.Text = settings.ImmichDefaultAlbumName;
        }
        else _album.SelectedIndex = 0;

        var totalBytes = photos.Sum(photo => { try { return new FileInfo(photo.FilePath).Length; } catch { return 0; } });
        var pending = photos.Count(photo => photo.HasPendingChanges);
        BuildLayout($"{server.Name} — {server.Version}", photos.Count, totalBytes, pending);
        _album.SelectedIndexChanged += (_, _) => _newAlbum.Enabled = Equals(_album.SelectedItem, NewAlbum);
        _newAlbum.Enabled = Equals(_album.SelectedItem, NewAlbum);
        _applyFirst.Visible = pending > 0;
        ThemeService.Apply(this);
    }

    private void BuildLayout(string server, int count, long bytes, int pending)
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 2, RowCount = 8 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var row = 0; row < 7; row++) root.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        AddRow(root, 0, "Serveur", new Label { Text = server, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft });
        AddRow(root, 1, "Fichiers", new Label { Text = $"{count} image(s) — {FormatBytes(bytes)}", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft });
        AddRow(root, 2, "Album", _album);
        AddRow(root, 3, "Nouvel album", _newAlbum);
        AddRow(root, 4, "Visibilité", _visibility);
        AddRow(root, 5, "Envois simultanés", _concurrency);
        root.Controls.Add(_applyFirst, 1, 6);
        if (pending > 0) root.Controls.Add(new Label { Text = $"{pending} image(s) ont des modifications en attente.", AutoSize = true, ForeColor = Color.DarkOrange }, 1, 7);
        var send = new Button { Text = "Envoyer", AutoSize = true };
        var cancel = new Button { Text = "Annuler", AutoSize = true, DialogResult = DialogResult.Cancel };
        send.Click += (_, _) => Confirm();
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, Padding = new Padding(10), FlowDirection = FlowDirection.RightToLeft };
        buttons.Controls.AddRange(new Control[] { cancel, send });
        Controls.Add(root);
        Controls.Add(buttons);
        AcceptButton = send;
        CancelButton = cancel;
        ClientSize = new Size(680, 430);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Envoyer vers Immich";
    }

    private static void AddRow(TableLayoutPanel root, int row, string label, Control control)
    {
        root.Controls.Add(new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, row);
        root.Controls.Add(control, 1, row);
    }

    private void Confirm()
    {
        var selectedAlbum = _album.SelectedItem;
        var newName = Equals(selectedAlbum, NewAlbum) ? _newAlbum.Text.Trim() : null;
        if (Equals(selectedAlbum, NewAlbum) && string.IsNullOrWhiteSpace(newName))
        {
            ThemedMessageBox.Show("Saisissez le nom du nouvel album.", "Immich", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var albumId = selectedAlbum is ImmichAlbum album ? album.Id : null;
        var visibility = _visibility.SelectedIndex switch { 1 => ImmichAssetVisibility.Archive, 2 => ImmichAssetVisibility.Hidden, 3 => ImmichAssetVisibility.Locked, _ => ImmichAssetVisibility.Timeline };
        Request = new ImmichUploadRequest([], albumId, newName, visibility, (int)_concurrency.Value);
        DialogResult = DialogResult.OK;
    }

    private static string FormatBytes(long bytes)
    {
        string[] units = ["o", "Ko", "Mo", "Go", "To"];
        var value = (double)bytes;
        var unit = 0;
        while (value >= 1024 && unit < units.Length - 1) { value /= 1024; unit++; }
        return $"{value:0.#} {units[unit]}";
    }
}
