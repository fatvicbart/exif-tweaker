namespace ExifTweaker.Infrastructure;

public static class ThemedMessageBox
{
    public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) =>
        Show(null, text, caption, buttons, icon);

    public static DialogResult Show(IWin32Window? owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
    {
        using var dialog = new MessageDialog(text, caption, buttons, icon);
        ThemeService.Apply(dialog);
        return owner is null ? dialog.ShowDialog() : dialog.ShowDialog(owner);
    }

    private sealed class MessageDialog : Form
    {
        public MessageDialog(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            const int messageWidth = 510;
            var normalizedMessage = NormalizeMessage(message);
            var measured = TextRenderer.MeasureText(normalizedMessage, SystemFonts.MessageBoxFont,
                new Size(messageWidth, int.MaxValue), TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

            Text = caption;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(620, Math.Clamp(measured.Height + 118, 190, 480));

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 2, RowCount = 2 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

            var iconBox = new PictureBox
            {
                Dock = DockStyle.Top, Height = 40, SizeMode = PictureBoxSizeMode.CenterImage,
                Image = IconFor(icon)?.ToBitmap()
            };
            var messageLabel = new Label
            {
                AutoSize = true, MaximumSize = new Size(messageWidth, 0),
                Text = normalizedMessage, Font = SystemFonts.MessageBoxFont,
                Location = Point.Empty, UseMnemonic = false
            };
            var messageHost = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(0, 2, 4, 2) };
            messageHost.Controls.Add(messageLabel);

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 8, 0, 0)
            };
            foreach (var definition in ButtonDefinitions(buttons))
            {
                var button = new Button
                {
                    Text = definition.Text, DialogResult = definition.Result, AutoSize = true, MinimumSize = new Size(92, 30)
                };
                buttonPanel.Controls.Add(button);
                if (definition.IsDefault) AcceptButton = button;
                if (definition.Result == DialogResult.Cancel) CancelButton = button;
            }

            root.Controls.Add(iconBox, 0, 0);
            root.Controls.Add(messageHost, 1, 0);
            root.Controls.Add(buttonPanel, 0, 1);
            root.SetColumnSpan(buttonPanel, 2);
            Controls.Add(root);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            ThemeService.Apply(this);
        }

        private static string NormalizeMessage(string message)
        {
            var lines = message.Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\r", "\n", StringComparison.Ordinal)
                .Split((char)10).Select(line => line.TrimEnd()).ToList();
            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[0])) lines.RemoveAt(0);
            while (lines.Count > 0 && string.IsNullOrWhiteSpace(lines[^1])) lines.RemoveAt(lines.Count - 1);
            for (var index = lines.Count - 1; index > 0; index--)
                if (string.IsNullOrWhiteSpace(lines[index]) && string.IsNullOrWhiteSpace(lines[index - 1])) lines.RemoveAt(index);
            return string.Join(Environment.NewLine, lines);
        }

        private static Icon? IconFor(MessageBoxIcon icon) => icon switch
        {
            MessageBoxIcon.Error => SystemIcons.Error,
            MessageBoxIcon.Warning => SystemIcons.Warning,
            MessageBoxIcon.Information => SystemIcons.Information,
            MessageBoxIcon.Question => SystemIcons.Question,
            _ => null
        };

        private static IReadOnlyList<(string Text, DialogResult Result, bool IsDefault)> ButtonDefinitions(MessageBoxButtons buttons) => buttons switch
        {
            MessageBoxButtons.OKCancel => new[] { ("Annuler", DialogResult.Cancel, false), ("OK", DialogResult.OK, true) },
            MessageBoxButtons.YesNo => new[] { ("Non", DialogResult.No, false), ("Oui", DialogResult.Yes, true) },
            MessageBoxButtons.YesNoCancel => new[] { ("Annuler", DialogResult.Cancel, false), ("Non", DialogResult.No, false), ("Oui", DialogResult.Yes, true) },
            MessageBoxButtons.RetryCancel => new[] { ("Annuler", DialogResult.Cancel, false), ("Réessayer", DialogResult.Retry, true) },
            _ => new[] { ("OK", DialogResult.OK, true) }
        };
    }
}
