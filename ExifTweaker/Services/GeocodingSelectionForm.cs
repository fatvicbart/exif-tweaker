using ExifTweaker.Models;

namespace ExifTweaker.Services;

public sealed partial class GeocodingSelectionForm : Form
{
    public Coordinates? Selected => resultsList.SelectedItem as Coordinates;

    public GeocodingSelectionForm(IReadOnlyList<Coordinates> results)
    {
        InitializeComponent();
        resultsList.DataSource = results.ToList();
    }
}
