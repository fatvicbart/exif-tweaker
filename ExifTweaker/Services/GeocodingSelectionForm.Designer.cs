namespace ExifTweaker.Services;

partial class GeocodingSelectionForm
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
        resultsList = new ListBox();
        buttonsPanel = new Panel();
        cancelButton = new Button();
        useLocationButton = new Button();
        buttonsPanel.SuspendLayout();
        SuspendLayout();
        // 
        // resultsList
        // 
        resultsList.DisplayMember = "Name";
        resultsList.Dock = DockStyle.Fill;
        resultsList.FormattingEnabled = true;
        resultsList.ItemHeight = 15;
        resultsList.Location = new Point(0, 0);
        resultsList.Name = "resultsList";
        resultsList.Size = new Size(700, 318);
        resultsList.TabIndex = 0;
        // 
        // buttonsPanel
        // 
        buttonsPanel.Controls.Add(cancelButton);
        buttonsPanel.Controls.Add(useLocationButton);
        buttonsPanel.Dock = DockStyle.Bottom;
        buttonsPanel.Location = new Point(0, 318);
        buttonsPanel.Name = "buttonsPanel";
        buttonsPanel.Padding = new Padding(8);
        buttonsPanel.Size = new Size(700, 42);
        buttonsPanel.TabIndex = 1;
        // 
        // cancelButton
        // 
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Dock = DockStyle.Right;
        cancelButton.Location = new Point(572, 8);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(120, 26);
        cancelButton.TabIndex = 1;
        cancelButton.Text = "Cancel";
        cancelButton.UseVisualStyleBackColor = true;
        // 
        // useLocationButton
        // 
        useLocationButton.DialogResult = DialogResult.OK;
        useLocationButton.Dock = DockStyle.Right;
        useLocationButton.Location = new Point(452, 8);
        useLocationButton.Name = "useLocationButton";
        useLocationButton.Size = new Size(120, 26);
        useLocationButton.TabIndex = 0;
        useLocationButton.Text = "Use location";
        useLocationButton.UseVisualStyleBackColor = true;
        // 
        // GeocodingSelectionForm
        // 
        AcceptButton = useLocationButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(700, 360);
        Controls.Add(resultsList);
        Controls.Add(buttonsPanel);
        Name = "GeocodingSelectionForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Select location";
        buttonsPanel.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.ListBox resultsList;
    private System.Windows.Forms.Panel buttonsPanel;
    private System.Windows.Forms.Button useLocationButton;
    private System.Windows.Forms.Button cancelButton;
}
