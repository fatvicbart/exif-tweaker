namespace ExifTweaker.Services;

public sealed class ApplyReportForm : Form
{
    public ApplyReportForm(MetadataApplyResult result)
    {
        Text = "Apply report";
        Width = 900; Height = 500;
        var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = true, DataSource = result.Files.ToList() };
        var summary = new Label { Dock = DockStyle.Top, Height = 32, Text = $"Succeeded: {result.SucceededCount}   Failed: {result.FailedCount}", TextAlign = ContentAlignment.MiddleLeft };
        Controls.Add(grid); Controls.Add(summary);
    }
}
