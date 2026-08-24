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

    public static bool IsDark(AppThemeMode mode) => mode == AppThemeMode.Dark || mode == AppThemeMode.Automatic && IsWindowsDarkMode();

    public static void Apply(Control root, AppThemeMode mode)
    {
        var dark = IsDark(mode);
        ApplyControl(root, dark);
        if (root is Form form && OperatingSystem.IsWindows() && form.IsHandleCreated)
        {
            var enabled = dark ? 1 : 0;
            _ = DwmSetWindowAttribute(form.Handle, 20, ref enabled, sizeof(int));
        }
        root.Invalidate(true);
    }

    private static void ApplyControl(Control control, bool dark)
    {
        var background = dark ? DarkBackground : SystemColors.Control;
        var surface = dark ? DarkSurface : SystemColors.Control;
        var input = dark ? DarkInput : SystemColors.Window;
        var foreground = dark ? DarkText : SystemColors.ControlText;
        control.ForeColor = foreground;
        control.BackColor = control is TextBoxBase or ComboBox or ListBox or NumericUpDown ? input : background;

        if (control is Button button)
        {
            button.UseVisualStyleBackColor = false;
            button.BackColor = surface;
            button.FlatStyle = dark ? FlatStyle.Flat : FlatStyle.Standard;
            button.FlatAppearance.BorderColor = dark ? DarkBorder : SystemColors.ControlDark;
        }
        else if (control is DataGridView grid)
        {
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
        }
        else if (control is ToolStrip strip)
        {
            strip.BackColor = surface;
            strip.ForeColor = foreground;
            strip.Renderer = dark ? new ToolStripProfessionalRenderer(new DarkColorTable()) : new ToolStripProfessionalRenderer();
            ApplyItems(strip.Items, foreground);
        }

        if (OperatingSystem.IsWindows() && control.IsHandleCreated)
            _ = SetWindowTheme(control.Handle, dark ? "DarkMode_Explorer" : "Explorer", null);
        foreach (Control child in control.Controls) ApplyControl(child, dark);
    }

    private static void ApplyItems(ToolStripItemCollection items, Color foreground)
    {
        foreach (ToolStripItem item in items)
        {
            item.ForeColor = foreground;
            if (item is ToolStripDropDownItem dropDown) ApplyItems(dropDown.DropDownItems, foreground);
        }
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
}
