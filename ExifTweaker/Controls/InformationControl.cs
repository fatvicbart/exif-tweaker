using ExifTweaker.Models;

namespace ExifTweaker.Controls;

public sealed class InformationControl : UserControl
{
    private readonly Label _fileName = new() { AutoEllipsis = true, Dock = DockStyle.Top, Height = 26, Padding = new Padding(6, 5, 6, 0) };
    private readonly TextBox _filter = new() { Dock = DockStyle.Top, PlaceholderText = "Filtrer les métadonnées…" };
    private readonly DataGridView _grid = new()
    {
        AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false,
        AutoGenerateColumns = false, BackgroundColor = SystemColors.Window, BorderStyle = BorderStyle.Fixed3D,
        ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText, Dock = DockStyle.Fill,
        MultiSelect = true, ReadOnly = true, RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect
    };
    private readonly Label _status = new()
    {
        AutoSize = false, BackColor = SystemColors.Window, Dock = DockStyle.Fill,
        Text = "Sélectionnez une image pour afficher ses métadonnées.", TextAlign = ContentAlignment.MiddleCenter
    };
    private IReadOnlyList<ExifTagInfo> _tags = Array.Empty<ExifTagInfo>();
    private int _updateDepth;

    public InformationControl()
    {
        Dock = DockStyle.Fill;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ExifTagInfo.Group), HeaderText = "Groupe", Width = 110 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(ExifTagInfo.Name), HeaderText = "Propriété", Width = 180 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, DataPropertyName = nameof(ExifTagInfo.Value),
            HeaderText = "Valeur", MinimumWidth = 180
        });
        Controls.Add(_grid);
        Controls.Add(_status);
        Controls.Add(_filter);
        Controls.Add(_fileName);
        _filter.TextChanged += (_, _) => ApplyFilter();
    }

    public void BeginUpdate()
    {
        if (_updateDepth++ > 0) return;
        SuspendLayout();
        if (_grid.IsHandleCreated)
            SendMessage(_grid.Handle, WmSetRedraw, IntPtr.Zero, IntPtr.Zero);
    }

    public void EndUpdate()
    {
        if (_updateDepth == 0 || --_updateDepth > 0) return;
        if (_grid.IsHandleCreated)
            SendMessage(_grid.Handle, WmSetRedraw, new IntPtr(1), IntPtr.Zero);
        ResumeLayout(false);
        _grid.Invalidate(true);
        Invalidate(true);
    }

    public void ShowEmpty()
    {
        _fileName.Text = string.Empty;
        _tags = Array.Empty<ExifTagInfo>();
        _grid.DataSource = null;
        ShowStatus("Sélectionnez une image pour afficher ses métadonnées.");
    }

    public void ShowLoading(string filePath)
    {
        _fileName.Text = Path.GetFileName(filePath);
        _tags = Array.Empty<ExifTagInfo>();
        _grid.DataSource = null;
        ShowStatus("Lecture des métadonnées…");
    }

    public void ShowTags(string filePath, IReadOnlyList<ExifTagInfo> tags)
    {
        _fileName.Text = Path.GetFileName(filePath);
        _tags = tags;
        ApplyFilter();
    }

    public void ShowError(string filePath, string message)
    {
        _fileName.Text = Path.GetFileName(filePath);
        _tags = Array.Empty<ExifTagInfo>();
        _grid.DataSource = null;
        ShowStatus(message);
    }

    private void ApplyFilter()
    {
        var query = _filter.Text.Trim();
        var rows = string.IsNullOrWhiteSpace(query)
            ? _tags
            : _tags.Where(tag =>
                    tag.Group.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    tag.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                    tag.Value.Contains(query, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        _grid.DataSource = rows.ToList();
        _grid.Visible = rows.Count > 0;
        _status.Visible = rows.Count == 0;
        if (rows.Count == 0)
            _status.Text = _tags.Count == 0 ? "Aucune métadonnée disponible." : "Aucune métadonnée ne correspond au filtre.";
    }

    private void ShowStatus(string text)
    {
        _status.Text = text;
        _status.Visible = true;
        _status.BringToFront();
        _grid.Visible = false;
    }

    private const int WmSetRedraw = 0x000B;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}
