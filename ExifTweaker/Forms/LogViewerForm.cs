using System.Text.Json;
using ExifTweaker.Infrastructure;

namespace ExifTweaker.Forms;

public sealed class LogViewerForm : Form
{
    private readonly TextBox _search = new() { PlaceholderText = "Rechercher dans les journaux…", Width = 280 };
    private readonly ComboBox _level = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 130 };
    private readonly CheckBox _follow = new() { Text = "Suivre", Checked = true, AutoSize = true, Padding = new Padding(4, 6, 4, 0) };
    private readonly DataGridView _grid = new()
    {
        AllowUserToAddRows = false, AllowUserToDeleteRows = false, AutoGenerateColumns = false,
        Dock = DockStyle.Fill, ReadOnly = true, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect
    };
    private readonly RichTextBox _details = DetailBox();
    private readonly RichTextBox _json = DetailBox();
    private readonly Label _status = new() { AutoSize = true, Padding = new Padding(6, 8, 0, 0) };
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 2000 };
    private IReadOnlyList<LogEntry> _entries = Array.Empty<LogEntry>();
    private bool _loading;

    public LogViewerForm()
    {
        Text = "Journaux ExifTweaker";
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(850, 560);
        ClientSize = new Size(1050, 700);
        _level.Items.AddRange(new object[] { "Tous les niveaux", "Informations", "Erreurs", "Illisibles" });
        _level.SelectedIndex = 0;

        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Date", DataPropertyName = nameof(LogEntry.Timestamp), Width = 165 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Niveau", DataPropertyName = nameof(LogEntry.Level), Width = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Message", DataPropertyName = nameof(LogEntry.Message), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        _grid.CellFormatting += GridCellFormatting;
        _grid.SelectionChanged += (_, _) => ShowSelectedEntry();

        var refresh = new Button { Text = "Actualiser", AutoSize = true };
        var copy = new Button { Text = "Copier", AutoSize = true };
        var export = new Button { Text = "Exporter…", AutoSize = true };
        var close = new Button { Text = "Fermer", AutoSize = true, DialogResult = DialogResult.Cancel };
        refresh.Click += async (_, _) => await ReloadAsync();
        copy.Click += (_, _) => { if (!string.IsNullOrWhiteSpace(_details.Text)) Clipboard.SetText(_details.Text); };
        export.Click += async (_, _) => await ExportAsync();
        CancelButton = close;

        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(6), WrapContents = false };
        toolbar.Controls.AddRange(new Control[] { _search, _level, _follow, refresh, copy, export, _status });
        var tabs = new TabControl { Dock = DockStyle.Fill };
        var detailTab = new TabPage("Détails lisibles");
        var jsonTab = new TabPage("JSON source");
        detailTab.Controls.Add(_details);
        jsonTab.Controls.Add(_json);
        tabs.TabPages.AddRange(new[] { detailTab, jsonTab });
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 350 };
        split.Panel1.Controls.Add(_grid);
        split.Panel2.Controls.Add(tabs);
        var footer = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6) };
        footer.Controls.Add(close);
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1 };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.Controls.Add(toolbar, 0, 0);
        root.Controls.Add(split, 0, 1);
        root.Controls.Add(footer, 0, 2);
        Controls.Add(root);

        _search.TextChanged += (_, _) => ApplyFilter();
        _level.SelectedIndexChanged += (_, _) => ApplyFilter();
        _timer.Tick += async (_, _) => { if (_follow.Checked) await ReloadAsync(keepSelection: true); };
        Shown += async (_, _) => { ThemeService.Apply(this); await ReloadAsync(); _timer.Start(); };
        FormClosed += (_, _) => { _timer.Stop(); _timer.Dispose(); };
    }

    private async Task ReloadAsync(bool keepSelection = false)
    {
        if (_loading) return;
        _loading = true;
        var selectedRaw = keepSelection && _grid.CurrentRow?.DataBoundItem is LogEntry selected ? selected.RawJson : null;
        try
        {
            _entries = await AppLogger.ReadRecentAsync();
            ApplyFilter();
            if (selectedRaw is not null)
            {
                var row = _grid.Rows.Cast<DataGridViewRow>().FirstOrDefault(candidate => (candidate.DataBoundItem as LogEntry)?.RawJson == selectedRaw);
                if (row is not null) _grid.CurrentCell = row.Cells[0];
            }
        }
        catch (Exception ex)
        {
            _status.Text = "Lecture impossible";
            AppLogger.Error("Unable to read the log viewer data.", ex);
        }
        finally { _loading = false; }
    }

    private void ApplyFilter()
    {
        var query = _search.Text.Trim();
        var filtered = _entries.Where(entry => _level.SelectedIndex switch
            {
                1 => entry.Level.Equals("info", StringComparison.OrdinalIgnoreCase),
                2 => entry.Level.Equals("error", StringComparison.OrdinalIgnoreCase),
                3 => !entry.IsValid,
                _ => true
            })
            .Where(entry => string.IsNullOrWhiteSpace(query) ||
                entry.Message.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                (entry.ExceptionType?.Contains(query, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                (entry.ExceptionText?.Contains(query, StringComparison.CurrentCultureIgnoreCase) ?? false))
            .ToList();
        _grid.DataSource = filtered;
        _status.Text = $"{filtered.Count} / {_entries.Count}";
        if (filtered.Count == 0) { _details.Clear(); _json.Clear(); }
    }

    private void ShowSelectedEntry()
    {
        if (_grid.CurrentRow?.DataBoundItem is not LogEntry entry) return;
        var timestamp = entry.Timestamp == DateTimeOffset.MinValue ? "Date inconnue" : entry.Timestamp.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        _details.Text = $"Date : {timestamp}{Environment.NewLine}Niveau : {entry.Level}{Environment.NewLine}Message : {entry.Message}" +
            (string.IsNullOrWhiteSpace(entry.ExceptionType) ? string.Empty : $"{Environment.NewLine}Type : {entry.ExceptionType}") +
            (string.IsNullOrWhiteSpace(entry.ExceptionText) ? string.Empty : $"{Environment.NewLine}{Environment.NewLine}{entry.ExceptionText}");
        _json.Text = PrettyJson(entry.RawJson);
    }

    private async Task ExportAsync()
    {
        var rows = _grid.Rows.Cast<DataGridViewRow>().Select(row => row.DataBoundItem).OfType<LogEntry>().ToList();
        if (rows.Count == 0) return;
        using var dialog = new SaveFileDialog { Filter = "JSON|*.json", FileName = $"exiftweaker-logs-{DateTime.Now:yyyyMMdd-HHmmss}.json" };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        var export = rows.Select(entry => new { entry.Timestamp, entry.Level, entry.Message, entry.ExceptionType, exception = entry.ExceptionText, entry.IsValid });
        await File.WriteAllTextAsync(dialog.FileName, JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true }));
        _status.Text = "Export terminé";
    }

    private void GridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 || _grid.Rows[e.RowIndex].DataBoundItem is not LogEntry entry) return;
        if (e.ColumnIndex == 0)
        {
            e.Value = entry.Timestamp == DateTimeOffset.MinValue
                ? "Date inconnue"
                : entry.Timestamp.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            e.FormattingApplied = true;
        }
        if (entry.Level.Equals("error", StringComparison.OrdinalIgnoreCase)) _grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.IndianRed;
        else if (!entry.IsValid) _grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.DarkOrange;
    }

    private static string PrettyJson(string value)
    {
        try
        {
            using var document = JsonDocument.Parse(value);
            return JsonSerializer.Serialize(document.RootElement, new JsonSerializerOptions { WriteIndented = true });
        }
        catch (JsonException) { return value; }
    }

    private static RichTextBox DetailBox() => new()
    {
        Dock = DockStyle.Fill, ReadOnly = true, BorderStyle = BorderStyle.None, Font = new Font("Consolas", 9F), WordWrap = false
    };
}
