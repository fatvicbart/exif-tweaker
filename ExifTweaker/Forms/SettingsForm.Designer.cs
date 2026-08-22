namespace ExifTweaker.Forms;

partial class SettingsForm
{
    private System.ComponentModel.IContainer components = null;
    private ComboBox provider;
    private TextBox apiKey;
    private TextBox exifToolPath;
    private ComboBox backup;
    private NumericUpDown parallelism;
    private CheckBox recursive;
    private CheckBox diskCache;
    private TextBox mapTiles;
    private Button browseButton;
    private Button saveButton;
    private Button cancelButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        provider = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        apiKey = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
        exifToolPath = new TextBox { Dock = DockStyle.Fill };
        backup = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
        backup.Items.AddRange(new object[] { "Keep ExifTool original backup", "Overwrite original without backup" });
        parallelism = new NumericUpDown { Minimum = 1, Maximum = 16, Dock = DockStyle.Left };
        recursive = new CheckBox { Text = "Recursive folder import", AutoSize = true };
        diskCache = new CheckBox { Text = "Enable thumbnail disk cache", AutoSize = true };
        mapTiles = new TextBox { Dock = DockStyle.Fill };
        browseButton = new Button { Text = "Browse…", AutoSize = true };
        saveButton = new Button { Text = "Save", AutoSize = true };
        cancelButton = new Button { Text = "Cancel", AutoSize = true, DialogResult = DialogResult.Cancel };
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(10), ColumnCount = 3, RowCount = 9 };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 165));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        for (var i = 0; i < 8; i++) root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        browseButton.Click += browseButton_Click;
        saveButton.Click += saveButton_Click;

        AddRow(root, 0, "Geocoding provider", provider);
        AddRow(root, 1, "Maps.co API key", apiKey);
        AddRow(root, 2, "ExifTool executable", exifToolPath);
        root.Controls.Add(browseButton, 2, 2);
        AddRow(root, 3, "Backup strategy", backup);
        AddRow(root, 4, "Parallel operations", parallelism);
        AddRow(root, 5, "Import", recursive);
        AddRow(root, 6, "Thumbnail cache", diskCache);
        AddRow(root, 7, "Map tile URL", mapTiles);
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        buttons.Controls.AddRange(new Control[] { cancelButton, saveButton });
        root.Controls.Add(buttons, 0, 8);
        root.SetColumnSpan(buttons, 3);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
        ClientSize = new Size(720, 360);
        Controls.Add(root);
        MinimizeBox = false;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "ExifTweaker settings";
    }

    private static void AddRow(TableLayoutPanel root, int row, string label, Control control)
    {
        root.Controls.Add(new Label { Text = label, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, row);
        root.Controls.Add(control, 1, row);
    }
}
