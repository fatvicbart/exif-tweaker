using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ExifTweaker.Infrastructure;

public static class ThemeService
{
    private static readonly Color DarkBackground = Color.FromArgb(30, 30, 30);
    private static readonly Color DarkSurface = Color.FromArgb(45, 45, 48);
    private static readonly Color DarkInput = Color.FromArgb(37, 37, 38);
    private static readonly Color DarkBorder = Color.FromArgb(75, 75, 78);
    private static readonly Color DarkText = Color.FromArgb(241, 241, 241);
    private static readonly ConditionalWeakTable<ComboBox, object> StyledComboBoxes = new();
    private static bool _systemEventsHooked;

    public static AppThemeMode CurrentMode { get; private set; } = AppThemeMode.Automatic;
    public static event EventHandler? ThemeChanged;

    public static bool IsDark(AppThemeMode mode) => mode == AppThemeMode.Dark || mode == AppThemeMode.Automatic && IsWindowsDarkMode();
    public static bool IsDarkNow => IsDark(CurrentMode);

    public static void SetMode(AppThemeMode mode)
    {
        CurrentMode = mode;
        Application.SetColorMode(mode switch
        {
            AppThemeMode.Dark => SystemColorMode.Dark,
            AppThemeMode.Light => SystemColorMode.Classic,
            _ => SystemColorMode.System
        });
        EnsureSystemEvents();
        ApplyOpenForms();
        ThemeChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void Apply(Control root, AppThemeMode mode) => Apply(root, IsDark(mode));

    public static void Apply(Control root) => Apply(root, IsDarkNow);

    private static void Apply(Control root, bool dark)
    {
        ApplyControl(root, dark);
        if (root is Form form && OperatingSystem.IsWindows() && form.IsHandleCreated)
        {
            var enabled = dark ? 1 : 0;
            _ = DwmSetWindowAttribute(form.Handle, 20, ref enabled, sizeof(int));
        }
        root.Invalidate(true);
    }

    private static void ApplyOpenForms()
    {
        foreach (Form form in Application.OpenForms) Apply(form);
    }

    private static void ApplyControl(Control control, bool dark)
    {
        var background = dark ? DarkBackground : SystemColors.Control;
        var surface = dark ? DarkSurface : SystemColors.Control;
        var input = dark ? DarkInput : SystemColors.Window;
        var foreground = dark ? DarkText : SystemColors.ControlText;
        control.ForeColor = foreground;
        control.BackColor = control is TextBoxBase or ComboBox or ListBox or NumericUpDown ? input : background;

        switch (control)
        {
            case Button button:
                button.UseVisualStyleBackColor = false;
                button.BackColor = surface;
                button.FlatStyle = dark ? FlatStyle.Flat : FlatStyle.Standard;
                button.FlatAppearance.BorderColor = dark ? DarkBorder : SystemColors.ControlDark;
                break;
            case ComboBox combo:
                ConfigureComboBox(combo);
                break;
            case DateTimePicker picker:
                picker.CalendarForeColor = foreground;
                picker.CalendarMonthBackground = input;
                picker.CalendarTitleBackColor = surface;
                picker.CalendarTitleForeColor = foreground;
                picker.CalendarTrailingForeColor = dark ? Color.Silver : SystemColors.GrayText;
                break;
            case DataGridView grid:
                grid.EnableHeadersVisualStyles = false;
                grid.BackgroundColor = background;
                grid.GridColor = dark ? DarkBorder : SystemColors.ControlDark;
                grid.DefaultCellStyle.BackColor = input;
                grid.DefaultCellStyle.ForeColor = foreground;
                grid.DefaultCellStyle.SelectionBackColor = dark ? Color.FromArgb(0, 90, 158) : SystemColors.Highlight;
                grid.DefaultCellStyle.SelectionForeColor = dark ? Color.White : SystemColors.HighlightText;
                grid.ColumnHeadersDefaultCellStyle.BackColor = surface;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = foreground;
                grid.RowHeadersDefaultCellStyle.BackColor = surface;
                grid.RowHeadersDefaultCellStyle.ForeColor = foreground;
                break;
            case ProgressBar progress when progress.IsHandleCreated:
                _ = SendMessage(progress.Handle, 0x2001, IntPtr.Zero, (IntPtr)ColorTranslator.ToWin32(dark ? DarkInput : SystemColors.Control));
                _ = SendMessage(progress.Handle, 0x0410, IntPtr.Zero, (IntPtr)ColorTranslator.ToWin32(dark ? Color.FromArgb(0, 122, 204) : SystemColors.Highlight));
                break;
            case ToolStrip strip:
                strip.BackColor = surface;
                strip.ForeColor = foreground;
                strip.Renderer = dark ? new ToolStripProfessionalRenderer(new DarkColorTable()) : new ToolStripProfessionalRenderer();
                ApplyItems(strip.Items, foreground);
                break;
        }

        if (OperatingSystem.IsWindows() && control.IsHandleCreated)
        {
            _ = SetWindowTheme(control.Handle, dark ? "DarkMode_Explorer" : "Explorer", null);
            _ = SendMessage(control.Handle, 0x031A, IntPtr.Zero, IntPtr.Zero);
        }
        foreach (Control child in control.Controls) ApplyControl(child, dark);
    }

    private static void ConfigureComboBox(ComboBox combo)
    {
        combo.DrawMode = DrawMode.OwnerDrawFixed;
        if (StyledComboBoxes.TryGetValue(combo, out _)) return;
        StyledComboBoxes.Add(combo, new object());
        combo.DrawItem += (_, e) =>
        {
            e.DrawBackground();
            if (e.Index < 0) return;
            var dark = IsDarkNow;
            var selected = (e.State & DrawItemState.Selected) != 0;
            var back = selected ? (dark ? Color.FromArgb(0, 90, 158) : SystemColors.Highlight) : (dark ? DarkInput : SystemColors.Window);
            var fore = selected ? Color.White : (dark ? DarkText : SystemColors.ControlText);
            using var brush = new SolidBrush(back);
            e.Graphics.FillRectangle(brush, e.Bounds);
            TextRenderer.DrawText(e.Graphics, combo.GetItemText(combo.Items[e.Index]), e.Font, e.Bounds, fore, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            e.DrawFocusRectangle();
        };
    }

    private static void ApplyItems(ToolStripItemCollection items, Color foreground)
    {
        foreach (ToolStripItem item in items)
        {
            item.ForeColor = foreground;
            if (item is ToolStripDropDownItem dropDown) ApplyItems(dropDown.DropDownItems, foreground);
        }
    }

    private static void EnsureSystemEvents()
    {
        if (_systemEventsHooked) return;
        SystemEvents.UserPreferenceChanged += (_, _) =>
        {
            if (CurrentMode != AppThemeMode.Automatic) return;
            var dispatcher = Application.OpenForms.Cast<Form>()
                .FirstOrDefault(form => form.IsHandleCreated && !form.IsDisposed);
            dispatcher?.BeginInvoke(() =>
            {
                ApplyOpenForms();
                ThemeChanged?.Invoke(null, EventArgs.Empty);
            });
        };
        _systemEventsHooked = true;
    }

    private static bool IsWindowsDarkMode()
    {
        if (!OperatingSystem.IsWindows()) return false;
        try
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1) is int value && value == 0;
        }
        catch { return false; }
    }

    private sealed class DarkColorTable : ProfessionalColorTable
    {
        public override Color ToolStripGradientBegin => DarkSurface;
        public override Color ToolStripGradientMiddle => DarkSurface;
        public override Color ToolStripGradientEnd => DarkSurface;
        public override Color MenuStripGradientBegin => DarkSurface;
        public override Color MenuStripGradientEnd => DarkSurface;
        public override Color ToolStripDropDownBackground => DarkSurface;
        public override Color ImageMarginGradientBegin => DarkSurface;
        public override Color ImageMarginGradientMiddle => DarkSurface;
        public override Color ImageMarginGradientEnd => DarkSurface;
        public override Color MenuItemSelected => Color.FromArgb(62, 62, 64);
        public override Color MenuItemBorder => DarkBorder;
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(62, 62, 64);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(62, 62, 64);
        public override Color MenuItemPressedGradientBegin => DarkInput;
        public override Color MenuItemPressedGradientEnd => DarkInput;
        public override Color SeparatorDark => DarkBorder;
        public override Color SeparatorLight => DarkBorder;
    }

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    private static extern int SetWindowTheme(IntPtr handle, string? subAppName, string? subIdList);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr handle, int attribute, ref int value, int size);

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr handle, int message, IntPtr wParam, IntPtr lParam);
}
