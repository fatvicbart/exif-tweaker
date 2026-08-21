namespace ExifTweaker.Services;

partial class ApplyReportForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        summary = new Label();
        reportGrid = new DataGridView();
        ((System.ComponentModel.ISupportInitialize)reportGrid).BeginInit();
        SuspendLayout();
        // 
        // summary
        // 
        summary.Dock = DockStyle.Top;
        summary.Location = new Point(0, 0);
        summary.Name = "summary";
        summary.Size = new Size(900, 32);
        summary.TabIndex = 0;
        summary.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // reportGrid
        // 
        reportGrid.AllowUserToAddRows = false;
        reportGrid.AllowUserToDeleteRows = false;
        reportGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        reportGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        reportGrid.Dock = DockStyle.Fill;
        reportGrid.Location = new Point(0, 32);
        reportGrid.Name = "reportGrid";
        reportGrid.ReadOnly = true;
        reportGrid.Size = new Size(900, 468);
        reportGrid.TabIndex = 1;
        // 
        // ApplyReportForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(900, 500);
        Controls.Add(reportGrid);
        Controls.Add(summary);
        Name = "ApplyReportForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Apply report";
        ((System.ComponentModel.ISupportInitialize)reportGrid).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Label summary;
    private System.Windows.Forms.DataGridView reportGrid;
}
