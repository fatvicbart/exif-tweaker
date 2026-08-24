using System.Globalization;

namespace ExifTweaker.Controls;

public sealed class ThemedDateTimeInput : MaskedTextBox
{
    private const string DisplayFormat = "yyyy-MM-dd HH:mm:ss";
    private DateTime _value = DateTime.Now;

    public ThemedDateTimeInput()
    {
        Mask = "0000-00-00 00:00:00";
        TextMaskFormat = MaskFormat.IncludePromptAndLiterals;
        PromptChar = (char)32;
        Value = DateTime.Now;
    }

    public DateTime Value
    {
        get
        {
            if (TryReadValue(out var parsed)) _value = parsed;
            return _value;
        }
        set
        {
            _value = value;
            Text = value.ToString(DisplayFormat, CultureInfo.InvariantCulture);
        }
    }

    public DateTimePickerFormat Format { get; set; } = DateTimePickerFormat.Custom;
    public string CustomFormat { get; set; } = DisplayFormat;
    public bool ShowUpDown { get; set; } = true;

    protected override void OnLeave(EventArgs e)
    {
        if (TryReadValue(out var parsed)) _value = parsed;
        Text = _value.ToString(DisplayFormat, CultureInfo.InvariantCulture);
        base.OnLeave(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyCode is Keys.Up or Keys.Down)
        {
            var direction = e.KeyCode == Keys.Up ? 1 : -1;
            var caret = SelectionStart;
            Value = Adjust(Value, caret, direction);
            SelectionStart = Math.Min(caret, Text.Length);
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        base.OnKeyDown(e);
    }

    private bool TryReadValue(out DateTime value) => DateTime.TryParseExact(
        Text, DisplayFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);

    private static DateTime Adjust(DateTime value, int caret, int direction)
    {
        try
        {
            if (caret <= 4) return value.AddYears(direction);
            if (caret <= 7) return value.AddMonths(direction);
            if (caret <= 10) return value.AddDays(direction);
            if (caret <= 13) return value.AddHours(direction);
            if (caret <= 16) return value.AddMinutes(direction);
            return value.AddSeconds(direction);
        }
        catch (ArgumentOutOfRangeException) { return value; }
    }
}
