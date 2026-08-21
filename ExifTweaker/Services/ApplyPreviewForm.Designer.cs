namespace ExifTweaker.Services;

partial class ApplyPreviewForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null)) components.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        summary = new Label();
        previewGrid = new DataGridView();
        buttonsPanel = new Panel();
        cancelButton = new Button();
        applyButton = new Button();
        ((System.ComponentModel.ISupportInitialize)previewGrid).BeginInit();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        // 
        // summary
        // 
        summary.Dock = DockStyle.Top;
        summary.Location = new Point(0, 0);
        summary.Name = "summary";
        summary.Padding = new Padding(8, 0, 8, 0);
        summary.Size = new Size(980, 48);
        summary.TabIndex = 0;
        summary.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // previewGrid
        // 
        previewGrid.AllowUserToAddRows = false;
        previewGrid.AllowUserToDeleteRows = false;
        previewGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        previewGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        previewGrid.Dock = DockStyle.Fill;
        previewGrid.Location = new Point(0, 48);
        previewGrid.Name = "previewGrid";
        previewGrid.ReadOnly = true;
        previewGrid.Size = new Size(980, 432);
        previewGrid.TabIndex = 1;
        // 
        // buttonsPanel
        // 
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Controls.Add(applyButton);
        buttonsPanel.Dock = DockStyle.Bottom;
        buttonsPanel.Location = new Point(0, 480);
        buttonsPanel.Name = "buttonsPanel";
        buttonsPanel.Padding = new Padding(8);
        buttonsPanel.Size = new Size(980, 48);
        buttonsPanel.TabIndex = 2;
        // 
        // cancelButton
        // 
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Dock = DockStyle.Right;
        cancelButton.Location = new Point(772, 8);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(100, 32);
        cancelButton.TabIndex = 1;
        cancelButton.Text = "Cancel";
        cancelButton.UseVisualStyleBackColor = true;
        cancelButton.Click += cancelButton_Click;
        // 
        // applyButton
        // 
        applyButton.DialogResult = DialogResult.OK;
        applyButton.Dock = DockStyle.Right;
        applyButton.Location = new Point(872, 8);
        applyButton.Name = "applyButton";
        applyButton.Size = new Size(100, 32);
        applyButton.TabIndex = 0;
        applyButton.Text = "Apply";
        applyButton.UseVisualStyleBackColor = true;
        applyButton.Click += applyButton_Click;
        // 
        // ApplyPreviewForm
        // 
        AcceptButton = applyButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(980, 528);
        Controls.Add(previewGrid);
        Controls.Add(buttonsPanel);
        Controls.Add(summary);
        MinimizeBox = false;
        Name = "ApplyPreviewForm";
        ShowIcon = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Apply preview";
        ((System.ComponentModel.ISupportInitialize)previewGrid).EndInit();
        buttonsPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Label summary;
    private System.Windows.Forms.DataGridView previewGrid;
    private System.Windows.Forms.Panel buttonsPanel;
    private System.Windows.Forms.Button cancelButton;
    private System.Windows.Forms.Button applyButton;
}
