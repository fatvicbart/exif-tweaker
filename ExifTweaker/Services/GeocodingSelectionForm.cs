using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed class GeocodingSelectionForm : Form
{
    private readonly ListBox _results = new() { Dock = DockStyle.Fill, DisplayMember = nameof(Coordinates.Name) };
    public Coordinates? Selected => _results.SelectedItem as Coordinates;

    public GeocodingSelectionForm(IReadOnlyList<Coordinates> results)
    {
        Text = "Select location";
        Width = 700; Height = 360;
        _results.DataSource = results.ToList();
        var ok = new Button { Text = "Use location", DialogResult = DialogResult.OK, Dock = DockStyle.Right };
        var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Dock = DockStyle.Right };
        var buttons = new Panel { Dock = DockStyle.Bottom, Height = 42 };
        buttons.Controls.Add(cancel); buttons.Controls.Add(ok);
        Controls.Add(_results); Controls.Add(buttons);
        AcceptButton = ok; CancelButton = cancel;
    }
}
