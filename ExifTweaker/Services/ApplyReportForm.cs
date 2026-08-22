namespace ExifTweaker.Services;

public sealed partial class ApplyReportForm : Form
{
    public ApplyReportForm(MetadataApplyResult result)
    {
        InitializeComponent();
        var restorable = result.Files.Count(file => file.BackupAvailable);
        var warnings = result.Files.Count(file => !string.IsNullOrWhiteSpace(file.Warning));
        summary.Text = $"Succeeded: {result.SucceededCount}   Warnings: {warnings}   Failed: {result.FailedCount}   Cancelled: {result.CancelledCount}   Restorable: {restorable}";
        reportGrid.DataSource = result.Files.ToList();
    }
}
