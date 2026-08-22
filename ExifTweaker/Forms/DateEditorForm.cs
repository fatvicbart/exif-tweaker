using ExifTweaker.Models;

namespace ExifTweaker.Forms;

public sealed partial class DateEditorForm : Form
{
    public DateEditRequest? Request { get; private set; }

    public DateEditorForm(DateTime? initialDate, TimeSpan? initialOffset, bool hasMultipleValues = false)
    {
        InitializeComponent();
        if (hasMultipleValues) Text += " — <multiple values>";
        dateValue.Value = initialDate ?? DateTime.Now;
        if (initialOffset.HasValue) offsetValue.Value = Math.Clamp((decimal)initialOffset.Value.TotalHours, offsetValue.Minimum, offsetValue.Maximum);
        mode.SelectedIndex = 0;
        timezoneMode.SelectedIndex = 0;
        UpdateMode();
    }

    private void mode_SelectedIndexChanged(object? sender, EventArgs e) => UpdateMode();

    private void UpdateMode()
    {
        var set = mode.SelectedIndex == 0;
        dateValue.Enabled = set;
        shiftPanel.Enabled = !set;
    }

    private void applyButton_Click(object? sender, EventArgs e)
    {
        var offset = TimeSpan.FromMinutes((double)(offsetValue.Value * 60m));
        Request = new DateEditRequest
        {
            Mode = mode.SelectedIndex == 0 ? DateEditMode.Set : DateEditMode.Shift,
            Date = dateValue.Value,
            Years = (int)yearsValue.Value,
            Months = (int)monthsValue.Value,
            Days = (int)daysValue.Value,
            Hours = (int)hoursValue.Value,
            Minutes = (int)minutesValue.Value,
            Seconds = (int)secondsValue.Value,
            ChangeTimezone = changeTimezone.Checked,
            RemoveTimezone = changeTimezone.Checked && removeTimezone.Checked,
            TimezoneOffset = offset,
            TimezoneMode = timezoneMode.SelectedIndex == 1 ? TimezoneChangeMode.ConvertInstant : TimezoneChangeMode.KeepLocalTime
        };
        DialogResult = DialogResult.OK;
    }
}
