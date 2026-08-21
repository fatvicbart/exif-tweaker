namespace ExifTweaker.Services;

public sealed partial class ApplyReportForm : Form
{
    public ApplyReportForm(MetadataApplyResult result)
    {
        InitializeComponent();
        summary.Text = $"Succeeded: {result.SucceededCount}   Failed: {result.FailedCount}";
        reportGrid.DataSource = result.Files.ToList();
    }
}
