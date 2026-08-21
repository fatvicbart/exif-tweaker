namespace ExifTweaker.Services;

public sealed partial class ApplyPreviewForm : Form
{
    public ApplyPreviewForm(MetadataApplyPreview preview)
    {
        InitializeComponent();
        summary.Text = $"Apply metadata changes to {preview.FileCount} file(s) | Dates: {preview.DateChanges} | Locations: {preview.LocationChanges} changed / {preview.LocationRemovals} removed | Offsets: {preview.OffsetChanges} | Types: {preview.FileTypeSummary} | Backup originals: {(preview.BackupOriginals ? "Yes" : "No")}";
        previewGrid.DataSource = preview.Files.ToList();
    }

    public bool Confirmed { get; private set; }

    private void applyButton_Click(object sender, EventArgs e) { Confirmed = true; DialogResult = DialogResult.OK; }
    private void cancelButton_Click(object sender, EventArgs e) { Confirmed = false; DialogResult = DialogResult.Cancel; }
}
