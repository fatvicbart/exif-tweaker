namespace ExifTweaker.Services;

public sealed partial class ApplyReportForm : Form
{
    public ApplyReportForm(MetadataApplyResult result)
    {
        InitializeComponent();
        var restorable = result.Files.Count(file => file.BackupAvailable);
        summary.Text = $"Succeeded: {result.SucceededCount}   Failed: {result.FailedCount}   Restorable: {restorable}";
        reportGrid.DataSource = result.Files.ToList();
    }
}
