using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Windows_form_game_V1._0.Forms
{
    internal enum BayanihanMessageType
    {
        Info,
        Warning,
        Error,
        Success,
        Confirmation,
        QuestDialogue
    }

    internal static class BayanihanMessageBox
    {
        public static DialogResult Show(IWin32Window owner, string message, string title, BayanihanMessageType type, MessageBoxButtons buttons)
        {
            try
            {
                using (var box = new BayanihanMessageBoxForm(message, title, type, buttons, null))
                {
                    return owner != null ? box.ShowDialog(owner) : box.ShowDialog();
                }
            }
            catch
            {
                return MessageBox.Show(owner, message, title, buttons, ResolveFallbackIcon(type));
            }
        }

        public static DialogResult ShowQuestDialogue(IWin32Window owner, string speaker, string message, string title)
        {
            try
            {
                using (var box = new BayanihanMessageBoxForm(message, title, BayanihanMessageType.QuestDialogue, MessageBoxButtons.OK, speaker))
                {
                    return owner != null ? box.ShowDialog(owner) : box.ShowDialog();
                }
            }
            catch
            {
                var fullMessage = string.IsNullOrWhiteSpace(speaker)
                    ? message
                    : speaker + "\n\n" + message;

                return MessageBox.Show(owner, fullMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private static MessageBoxIcon ResolveFallbackIcon(BayanihanMessageType type)
        {
            if (type == BayanihanMessageType.Warning)
            {
                return MessageBoxIcon.Warning;
            }

            if (type == BayanihanMessageType.Error)
            {
                return MessageBoxIcon.Error;
            }

            if (type == BayanihanMessageType.Success)
            {
                return MessageBoxIcon.Asterisk;
            }

            if (type == BayanihanMessageType.Confirmation)
            {
                return MessageBoxIcon.Question;
            }

            return MessageBoxIcon.Information;
        }
    }

    internal sealed class BayanihanMessageBoxForm : Form
    {
        private sealed class ButtonSpec
        {
            public string Text { get; set; }
            public DialogResult Result { get; set; }
            public bool IsPrimary { get; set; }
        }

        private sealed class ColorScheme
        {
            public Color Accent { get; set; }
            public Color Surface { get; set; }
            public Color IconBackground { get; set; }
            public Color IconForeground { get; set; }
            public string Icon { get; set; }
        }

        private readonly string message;
        private readonly string speaker;
        private readonly string titleText;
        private readonly BayanihanMessageType type;
        private readonly MessageBoxButtons buttons;

        public BayanihanMessageBoxForm(string message, string title, BayanihanMessageType type, MessageBoxButtons buttons, string speaker)
        {
            this.message = message ?? string.Empty;
            this.titleText = string.IsNullOrWhiteSpace(title) ? "Bayanihan Quest" : title;
            this.type = type;
            this.buttons = buttons;
            this.speaker = speaker;

            BuildUi();
        }

        private void BuildUi()
        {
            SuspendLayout();

            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            BackColor = Color.FromArgb(6, 10, 18);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            TopMost = true;
            KeyPreview = true;
            ClientSize = new Size(620, type == BayanihanMessageType.QuestDialogue ? 430 : 310);

            var scheme = GetScheme(type);

            var root = new Panel
            {
                BackColor = scheme.Surface,
                Dock = DockStyle.Fill,
                Padding = new Padding(0)
            };
            Controls.Add(root);

            var accent = new Panel
            {
                Dock = DockStyle.Top,
                Height = 7,
                BackColor = scheme.Accent
            };

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                Padding = new Padding(20, 14, 20, 12),
                BackColor = Color.Transparent
            };

            var iconPanel = new Panel
            {
                Size = new Size(42, 42),
                Location = new Point(20, 13),
                BackColor = scheme.IconBackground
            };
            header.Controls.Add(iconPanel);
            ApplyRoundedRegion(iconPanel, 20);

            var iconLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = scheme.Icon,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = scheme.IconForeground,
                BackColor = Color.Transparent
            };
            iconPanel.Controls.Add(iconLabel);

            var titleLabel = new Label
            {
                AutoSize = false,
                Location = new Point(76, 12),
                Size = new Size(ClientSize.Width - 156, 46),
                Text = titleText,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold)
            };
            header.Controls.Add(titleLabel);

            var closeButton = new Button
            {
                FlatStyle = FlatStyle.Flat,
                Text = "✕",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(185, 193, 207),
                BackColor = Color.Transparent,
                Size = new Size(34, 30),
                Location = new Point(ClientSize.Width - 48, 18),
                TabStop = false
            };
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(58, 66, 84);
            closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(71, 81, 104);
            closeButton.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };
            header.Controls.Add(closeButton);

            var content = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(22, 12, 22, 12),
                BackColor = Color.Transparent
            };

            if (type == BayanihanMessageType.QuestDialogue)
            {
                var speakerLabel = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 36,
                    Text = string.IsNullOrWhiteSpace(speaker) ? "Barangay Captain" : speaker,
                    ForeColor = scheme.Accent,
                    Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                    BackColor = Color.Transparent
                };
                content.Controls.Add(speakerLabel);

                var spacer = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 10,
                    BackColor = Color.Transparent
                };
                content.Controls.Add(spacer);
            }

            var messageBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Text = message,
                ForeColor = Color.FromArgb(233, 239, 250),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                BackColor = scheme.Surface,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                WordWrap = true,
                DetectUrls = false
            };
            messageBox.Padding = new Padding(0, type == BayanihanMessageType.QuestDialogue ? 8 : 2, 0, 0);
            messageBox.Margin = new Padding(0);
            messageBox.TabStop = false;
            messageBox.Cursor = Cursors.Default;
            messageBox.HideSelection = true;
            messageBox.SelectionStart = 0;
            messageBox.SelectionLength = 0;
            messageBox.ScrollToCaret();
            content.Controls.Add(messageBox);

            var buttonBar = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 72,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(14, 12, 14, 12),
                BackColor = Color.Transparent,
                WrapContents = false
            };
            root.Controls.Add(content);
            root.Controls.Add(buttonBar);
            root.Controls.Add(header);
            root.Controls.Add(accent);

            BuildButtons(buttonBar, scheme);

            Resize += (s, e) =>
            {
                ApplyRoundedRegion(this, 18);
                closeButton.Location = new Point(ClientSize.Width - 48, 18);
                titleLabel.Size = new Size(ClientSize.Width - 156, 46);
            };
            ApplyRoundedRegion(this, 18);

            KeyDown += BayanihanMessageBoxForm_KeyDown;

            ResumeLayout(false);
        }

        private void BayanihanMessageBoxForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
                e.Handled = true;
            }
        }

        private void BuildButtons(FlowLayoutPanel buttonBar, ColorScheme scheme)
        {
            var specs = ResolveButtonSpecs(buttons);
            for (var i = 0; i < specs.Count; i++)
            {
                var spec = specs[i];
                var button = CreateButton(spec.Text, spec.IsPrimary ? scheme.Accent : Color.FromArgb(56, 69, 93), spec.IsPrimary, spec.Result);
                buttonBar.Controls.Add(button);

                if (spec.Result == DialogResult.OK || spec.Result == DialogResult.Yes)
                {
                    AcceptButton = button;
                }

                if (spec.Result == DialogResult.Cancel || spec.Result == DialogResult.No)
                {
                    CancelButton = button;
                }
            }

            if (AcceptButton == null && buttonBar.Controls.Count > 0)
            {
                AcceptButton = buttonBar.Controls[buttonBar.Controls.Count - 1] as IButtonControl;
            }
        }

        private Button CreateButton(string text, Color baseColor, bool primary, DialogResult result)
        {
            var button = new Button
            {
                Text = text,
                DialogResult = result,
                FlatStyle = FlatStyle.Flat,
                Width = 132,
                Height = 40,
                Margin = new Padding(8, 0, 0, 0),
                BackColor = baseColor,
                ForeColor = primary ? Color.FromArgb(19, 24, 37) : Color.White,
                Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            button.FlatAppearance.BorderSize = 0;
            var hover = ChangeColor(baseColor, 14);
            var pressed = ChangeColor(baseColor, -14);

            button.MouseEnter += (s, e) => button.BackColor = hover;
            button.MouseLeave += (s, e) => button.BackColor = baseColor;
            button.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    button.BackColor = pressed;
                }
            };
            button.MouseUp += (s, e) =>
            {
                var localPoint = button.PointToClient(Cursor.Position);
                button.BackColor = button.ClientRectangle.Contains(localPoint) ? hover : baseColor;
            };

            button.Resize += (s, e) => ApplyRoundedRegion(button, 14);
            ApplyRoundedRegion(button, 14);

            return button;
        }

        private static List<ButtonSpec> ResolveButtonSpecs(MessageBoxButtons buttons)
        {
            var specs = new List<ButtonSpec>();

            if (buttons == MessageBoxButtons.OK)
            {
                specs.Add(new ButtonSpec { Text = "Okay", Result = DialogResult.OK, IsPrimary = true });
                return specs;
            }

            if (buttons == MessageBoxButtons.OKCancel)
            {
                specs.Add(new ButtonSpec { Text = "Cancel", Result = DialogResult.Cancel, IsPrimary = false });
                specs.Add(new ButtonSpec { Text = "Okay", Result = DialogResult.OK, IsPrimary = true });
                return specs;
            }

            if (buttons == MessageBoxButtons.YesNo)
            {
                specs.Add(new ButtonSpec { Text = "No", Result = DialogResult.No, IsPrimary = false });
                specs.Add(new ButtonSpec { Text = "Yes", Result = DialogResult.Yes, IsPrimary = true });
                return specs;
            }

            if (buttons == MessageBoxButtons.YesNoCancel)
            {
                specs.Add(new ButtonSpec { Text = "Cancel", Result = DialogResult.Cancel, IsPrimary = false });
                specs.Add(new ButtonSpec { Text = "No", Result = DialogResult.No, IsPrimary = false });
                specs.Add(new ButtonSpec { Text = "Yes", Result = DialogResult.Yes, IsPrimary = true });
                return specs;
            }

            if (buttons == MessageBoxButtons.RetryCancel)
            {
                specs.Add(new ButtonSpec { Text = "Cancel", Result = DialogResult.Cancel, IsPrimary = false });
                specs.Add(new ButtonSpec { Text = "Retry", Result = DialogResult.Retry, IsPrimary = true });
                return specs;
            }

            specs.Add(new ButtonSpec { Text = "Ignore", Result = DialogResult.Ignore, IsPrimary = false });
            specs.Add(new ButtonSpec { Text = "Retry", Result = DialogResult.Retry, IsPrimary = false });
            specs.Add(new ButtonSpec { Text = "Abort", Result = DialogResult.Abort, IsPrimary = true });
            return specs;
        }

        private static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null || control.Width <= 1 || control.Height <= 1)
            {
                return;
            }

            var maxRadius = (Math.Min(control.Width, control.Height) - 1) / 2;
            var finalRadius = Math.Max(2, Math.Min(radius, maxRadius));
            if (finalRadius < 2)
            {
                return;
            }

            using (var path = CreateRoundedPath(new Rectangle(0, 0, control.Width, control.Height), finalRadius))
            {
                if (path == null)
                {
                    return;
                }

                if (control.Region != null)
                {
                    control.Region.Dispose();
                }

                control.Region = new Region(path);
            }
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            if (radius < 2 || rect.Width <= 0 || rect.Height <= 0)
            {
                return null;
            }

            var diameter = radius * 2;

            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private static Color ChangeColor(Color color, int delta)
        {
            return Color.FromArgb(color.A, Clamp(color.R + delta), Clamp(color.G + delta), Clamp(color.B + delta));
        }

        private static int Clamp(int value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > 255)
            {
                return 255;
            }

            return value;
        }

        private static ColorScheme GetScheme(BayanihanMessageType type)
        {
            var scheme = new ColorScheme
            {
                Surface = Color.FromArgb(16, 25, 42),
                IconBackground = Color.FromArgb(35, 46, 70),
                IconForeground = Color.White,
                Accent = Color.FromArgb(56, 189, 248),
                Icon = "i"
            };

            switch (type)
            {
                case BayanihanMessageType.Warning:
                    scheme.Accent = Color.FromArgb(245, 158, 11);
                    scheme.IconBackground = Color.FromArgb(94, 64, 20);
                    scheme.IconForeground = Color.FromArgb(255, 234, 194);
                    scheme.Icon = "!";
                    break;
                case BayanihanMessageType.Error:
                    scheme.Accent = Color.FromArgb(239, 68, 68);
                    scheme.IconBackground = Color.FromArgb(92, 28, 28);
                    scheme.IconForeground = Color.FromArgb(255, 220, 220);
                    scheme.Icon = "✕";
                    break;
                case BayanihanMessageType.Success:
                    scheme.Accent = Color.FromArgb(34, 197, 94);
                    scheme.IconBackground = Color.FromArgb(23, 74, 47);
                    scheme.IconForeground = Color.FromArgb(220, 255, 226);
                    scheme.Icon = "✓";
                    break;
                case BayanihanMessageType.Confirmation:
                    scheme.Accent = Color.FromArgb(167, 139, 250);
                    scheme.IconBackground = Color.FromArgb(58, 46, 90);
                    scheme.IconForeground = Color.FromArgb(235, 229, 255);
                    scheme.Icon = "?";
                    break;
                case BayanihanMessageType.QuestDialogue:
                    scheme.Accent = Color.FromArgb(251, 192, 45);
                    scheme.Surface = Color.FromArgb(25, 35, 56);
                    scheme.IconBackground = Color.FromArgb(95, 73, 26);
                    scheme.IconForeground = Color.FromArgb(255, 248, 210);
                    scheme.Icon = "✦";
                    break;
                default:
                    break;
            }

            return scheme;
        }
    }
}
