namespace ExifTweaker.Forms;

partial class DateEditorForm
{
    private System.ComponentModel.IContainer components = null;
    private ComboBox mode;
    private DateTimePicker dateValue;
    private FlowLayoutPanel shiftPanel;
    private NumericUpDown yearsValue;
    private NumericUpDown monthsValue;
    private NumericUpDown daysValue;
    private NumericUpDown hoursValue;
    private NumericUpDown minutesValue;
    private NumericUpDown secondsValue;
    private CheckBox changeTimezone;
    private CheckBox removeTimezone;
    private NumericUpDown offsetValue;
    private ComboBox timezoneMode;
    private Button applyButton;
    private Button cancelButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        mode = new ComboBox();
        dateValue = new DateTimePicker();
        shiftPanel = new FlowLayoutPanel();
        yearsValue = Number(-99, 99, "Years");
        monthsValue = Number(-120, 120, "Months");
        daysValue = Number(-9999, 9999, "Days");
        hoursValue = Number(-9999, 9999, "Hours");
        minutesValue = Number(-9999, 9999, "Minutes");
        secondsValue = Number(-9999, 9999, "Seconds");
        changeTimezone = new CheckBox();
        removeTimezone = new CheckBox();
        offsetValue = new NumericUpDown();
        timezoneMode = new ComboBox();
        applyButton = new Button();
        cancelButton = new Button();
        var root = new TableLayoutPanel();
        var buttons = new FlowLayoutPanel();
        ((System.ComponentModel.ISupportInitialize)offsetValue).BeginInit();
        SuspendLayout();

        root.ColumnCount = 2;
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        root.Dock = DockStyle.Fill;
        root.Padding = new Padding(10);
        root.RowCount = 6;
        for (var i = 0; i < 5; i++) root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        mode.DropDownStyle = ComboBoxStyle.DropDownList;
        mode.Items.AddRange(new object[] { "Set date and time", "Shift dates" });
        mode.Dock = DockStyle.Fill;
        mode.SelectedIndexChanged += mode_SelectedIndexChanged;

        dateValue.CustomFormat = "yyyy-MM-dd HH:mm:ss";
        dateValue.Format = DateTimePickerFormat.Custom;
        dateValue.ShowUpDown = true;
        dateValue.Dock = DockStyle.Fill;

        shiftPanel.Dock = DockStyle.Fill;
        shiftPanel.WrapContents = false;
        shiftPanel.Controls.AddRange(new Control[]
        {
            ShiftLabel("Years"), yearsValue,
            ShiftLabel("Months"), monthsValue,
            ShiftLabel("Days"), daysValue,
            ShiftLabel("Hours"), hoursValue,
            ShiftLabel("Minutes"), minutesValue,
            ShiftLabel("Seconds"), secondsValue
        });

        changeTimezone.Text = "Change timezone";
        changeTimezone.AutoSize = true;
        removeTimezone.Text = "Remove offset";
        removeTimezone.AutoSize = true;

        offsetValue.Minimum = -14;
        offsetValue.Maximum = 14;
        offsetValue.DecimalPlaces = 2;
        offsetValue.Increment = 0.25m;
        offsetValue.Width = 90;

        timezoneMode.DropDownStyle = ComboBoxStyle.DropDownList;
        timezoneMode.Items.AddRange(new object[] { "Keep local clock time", "Convert the same instant" });
        timezoneMode.Width = 190;

        var timezonePanel = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        timezonePanel.Controls.AddRange(new Control[] { changeTimezone, new Label { Text = "UTC", AutoSize = true, Padding = new Padding(0, 5, 0, 0) }, offsetValue, timezoneMode, removeTimezone });

        applyButton.Text = "Apply to selection";
        applyButton.AutoSize = true;
        applyButton.Click += applyButton_Click;
        cancelButton.Text = "Cancel";
        cancelButton.AutoSize = true;
        cancelButton.DialogResult = DialogResult.Cancel;
        buttons.Dock = DockStyle.Fill;
        buttons.FlowDirection = FlowDirection.RightToLeft;
        buttons.Controls.AddRange(new Control[] { cancelButton, applyButton });

        root.Controls.Add(new Label { Text = "Operation", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 0);
        root.Controls.Add(mode, 1, 0);
        root.Controls.Add(new Label { Text = "Date and time", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 1);
        root.Controls.Add(dateValue, 1, 1);
        root.Controls.Add(new Label { Text = "Relative shift", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 2);
        root.Controls.Add(shiftPanel, 1, 2);
        root.Controls.Add(new Label { Text = "Timezone", TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 0, 3);
        root.Controls.Add(timezonePanel, 1, 3);
        root.Controls.Add(new Label { Text = "Timezone modes: keeping local time only changes the offset; conversion preserves the represented instant.", AutoSize = true, Dock = DockStyle.Fill }, 0, 4);
        root.SetColumnSpan(root.GetControlFromPosition(0, 4), 2);
        root.Controls.Add(buttons, 0, 5);
        root.SetColumnSpan(buttons, 2);

        AcceptButton = applyButton;
        CancelButton = cancelButton;
        ClientSize = new Size(850, 280);
        Controls.Add(root);
        MinimizeBox = false;
        MaximizeBox = false;
        Name = "DateEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Batch date and timezone editor";
        ((System.ComponentModel.ISupportInitialize)offsetValue).EndInit();
        ResumeLayout(false);
    }

    private static NumericUpDown Number(decimal min, decimal max, string label)
    {
        return new NumericUpDown { Minimum = min, Maximum = max, Width = 72, Tag = label, AccessibleName = label };
    }

    private static Label ShiftLabel(string text)
    {
        return new Label { Text = text, AutoSize = true, Padding = new Padding(0, 5, 0, 0) };
    }
}
