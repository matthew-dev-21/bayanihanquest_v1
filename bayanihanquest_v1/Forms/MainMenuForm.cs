using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace Windows_form_game_V1._0.Forms
{
    public partial class MainMenuForm : Form
    {
        private static readonly string[] MenuBackgroundCandidates =
        {
            "background-menu.png"
        };

        private static readonly Color PrimaryGreen = ColorTranslator.FromHtml("#2E7D32");
        private static readonly Color WarmYellow = ColorTranslator.FromHtml("#FBC02D");
        private static readonly Color EarthBrown = ColorTranslator.FromHtml("#8D6E63");

        public MainMenuForm()
        {
            InitializeComponent();
            InitializeFormAppearance();
            InitializeMenuCardStyle();
            InitializeLabelsAndButtons();
            InitializeHomeBackground();
            InitializeFormEvents();
        }

        #region Initialization

        private void InitializeFormAppearance()
        {
            // Basic form appearance and performance flags
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            BackColor = ColorTranslator.FromHtml("#0f172a");
            FormBorderStyle = FormBorderStyle.None;
            KeyPreview = true;
            DoubleBuffered = true;
            WindowState = FormWindowState.Maximized;
        }

        private void InitializeMenuCardStyle()
        {
            SuspendLayout();
            menuCard.SuspendLayout();

            menuCard.BorderStyle = BorderStyle.None;
            menuCard.BackColor = Color.FromArgb(102, 0, 0, 0);
            menuCard.Paint += MenuCard_Paint;
            ApplyRoundedRegion(menuCard, 26);
            menuCard.Resize += (s, e) => ApplyRoundedRegion(menuCard, 26);

            ResumeLayout(false);
            menuCard.ResumeLayout(false);
        }

        private void InitializeLabelsAndButtons()
        {
            // Labels
            lblTitle.Text = "BAYANIHAN\r\nQUEST";
            lblTitle.Font = new Font("Segoe UI", 38F, FontStyle.Bold);
            lblTitle.ForeColor = WarmYellow;
            lblTitle.AutoEllipsis = false;
            lblTitle.BackColor = Color.Transparent;

            lblSubtitle.Text = "Clean, help, and earn for your barangay.";
            lblSubtitle.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblSubtitle.ForeColor = Color.FromArgb(239, 245, 224);
            lblSubtitle.BackColor = Color.Transparent;

            lblFooter.BackColor = Color.FromArgb(92, 9, 29, 52);
            lblFooter.Padding = new Padding(8, 2, 8, 2);
            lblFooter.ForeColor = Color.FromArgb(240, 244, 248);

            // Buttons
            btnStartGame.Text = "▶ START GAME";
            btnLoadGame.Text = "▶ LOAD GAME";
            btnFeedback.Text = "✉ FEEDBACK";
            btnExit.Text = "❌ EXIT";

            StyleButton(btnStartGame, PrimaryGreen, Color.FromArgb(252, 252, 245));
            StyleButton(btnLoadGame, WarmYellow, Color.FromArgb(62, 42, 27));
            StyleButton(btnFeedback, Color.FromArgb(59, 130, 246), Color.FromArgb(240, 248, 255));
            StyleButton(btnExit, EarthBrown, Color.FromArgb(255, 247, 235));
        }

        private void InitializeHomeBackground()
        {
            var backgroundPath = FindAssetPath(MenuBackgroundCandidates);
            if (!string.IsNullOrWhiteSpace(backgroundPath))
            {
                TryLoadMenuBackground(backgroundPath);
            }
        }

        private void InitializeFormEvents()
        {
            Load += (s, e) =>
            {
                UpdateMenuLayout();
            };

            Resize += (s, e) =>
            {
                UpdateMenuLayout();
            };
        }

        #endregion

        #region Background Loading

        private bool TryLoadMenuBackground(string backgroundPath)
        {
            try
            {
                using (var fileStream = new FileStream(backgroundPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var image = Image.FromStream(fileStream))
                {
                    if (BackgroundImage != null)
                    {
                        BackgroundImage.Dispose();
                    }

                    BackgroundImage = new Bitmap(image);
                    BackgroundImageLayout = ImageLayout.Stretch;
                }

                return true;
            }
            catch
            {
                // Fall back to generated background
                return false;
            }
        }

        private string FindAssetPath(params string[] fileNames)
        {
            var startup = Application.StartupPath;
            var candidates = new System.Collections.Generic.List<string>();

            // Direct and /assets under startup
            foreach (var fileName in fileNames)
            {
                candidates.Add(Path.Combine(startup, fileName));
                candidates.Add(Path.Combine(startup, "assets", fileName));
            }

            // Walk up parent directories and check both root and /assets
            var current = new DirectoryInfo(startup);
            for (var i = 0; i < 6 && current != null; i++)
            {
                foreach (var fileName in fileNames)
                {
                    candidates.Add(Path.Combine(current.FullName, fileName));
                    candidates.Add(Path.Combine(current.FullName, "assets", fileName));
                }

                current = current.Parent;
            }

            return candidates.FirstOrDefault(File.Exists);
        }

        #endregion

        #region Layout

        private void UpdateMenuLayout()
        {
            if (ClientSize.Width <= 0 || ClientSize.Height <= 0)
            {
                return;
            }

            var cardSize = CalculateMenuCardSize(ClientSize);
            PositionMenuCard(cardSize);

            UpdateTitleLayout(cardSize);
            UpdateSubtitleLayout(cardSize);
            UpdateButtonsLayout(cardSize);

            ApplyRoundedRegion(menuCard, 26);
        }

        private static Size CalculateMenuCardSize(Size clientSize)
        {
            var horizontalMargin = Math.Max(28, clientSize.Width / 24);
            var verticalMargin = Math.Max(26, clientSize.Height / 20);

            var width = Math.Min(560, Math.Max(420, clientSize.Width - (horizontalMargin * 2)));
            var height = Math.Min(596, Math.Max(450, clientSize.Height - (verticalMargin * 2)));

            return new Size(width, height);
        }

        private void PositionMenuCard(Size cardSize)
        {
            menuCard.Size = cardSize;
            menuCard.Location = new Point(
                (ClientSize.Width - cardSize.Width) / 2,
                (ClientSize.Height - cardSize.Height) / 2);
        }

        private void UpdateTitleLayout(Size cardSize)
        {
            var titlePaddingTop = Math.Max(18, cardSize.Height / 34);
            var titleWidth = cardSize.Width - 60;

            var targetFontSize = GetTitleFontSize(cardSize.Width);
            if (Math.Abs(lblTitle.Font.Size - targetFontSize) > 0.1f)
            {
                lblTitle.Font = new Font("Segoe UI", targetFontSize, FontStyle.Bold);
            }

            var titleMeasured = TextRenderer.MeasureText(
                lblTitle.Text,
                lblTitle.Font,
                new Size(titleWidth, int.MaxValue),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);

            var titleHeight = Math.Min(180, Math.Max(120, titleMeasured.Height + 12));

            lblTitle.Location = new Point(30, titlePaddingTop);
            lblTitle.Size = new Size(titleWidth, titleHeight);
        }

        private static float GetTitleFontSize(int cardWidth)
        {
            if (cardWidth >= 540)
            {
                return 38F;
            }

            if (cardWidth >= 470)
            {
                return 34F;
            }

            return 30F;
        }

        private void UpdateSubtitleLayout(Size cardSize)
        {
            var subtitleHorizontalPadding = 44;
            lblSubtitle.Location = new Point(subtitleHorizontalPadding, lblTitle.Bottom + 10);
            lblSubtitle.Size = new Size(cardSize.Width - (subtitleHorizontalPadding * 2), 34);
        }

        private void UpdateButtonsLayout(Size cardSize)
        {
            var buttonWidth = Math.Min(335, cardSize.Width - 86);
            const int buttonHeight = 60;
            const int buttonSpacing = 16;

            var totalButtonsHeight = (buttonHeight * 4) + (buttonSpacing * 3);
            var bottomPadding = 70;

            var minimumButtonsTop = lblSubtitle.Bottom + 44;
            var bottomAlignedTop = cardSize.Height - (totalButtonsHeight + bottomPadding);
            var buttonsTop = Math.Max(minimumButtonsTop, bottomAlignedTop);

            var buttonX = (cardSize.Width - buttonWidth) / 2;

            btnStartGame.SetBounds(buttonX, buttonsTop, buttonWidth, buttonHeight);
            btnLoadGame.SetBounds(buttonX, btnStartGame.Bottom + buttonSpacing, buttonWidth, buttonHeight);
            btnFeedback.SetBounds(buttonX, btnLoadGame.Bottom + buttonSpacing, buttonWidth, buttonHeight);
            btnExit.SetBounds(buttonX, btnFeedback.Bottom + buttonSpacing, buttonWidth, buttonHeight);
        }

        #endregion

        #region Painting

        private void MenuCard_Paint(object sender, PaintEventArgs e)
        {
            var rect = new Rectangle(0, 0, menuCard.Width - 1, menuCard.Height - 1);
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            using (var border = new Pen(Color.FromArgb(88, 255, 255, 255), 1.2f))
            using (var roundedPath = CreateRoundedRectanglePath(rect, GetMenuCardRadius(rect)))
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                if (roundedPath == null)
                {
                    e.Graphics.DrawRectangle(border, rect);
                    return;
                }

                e.Graphics.DrawPath(border, roundedPath);
            }
        }

        private static int GetMenuCardRadius(Rectangle rect)
        {
            return Math.Min(26, (Math.Min(rect.Width, rect.Height) - 1) / 2);
        }

        #endregion

        #region Rounded Regions / Buttons

        private static void ApplyRoundedRegion(Control control, int radius)
        {
            if (control == null || control.Width <= 1 || control.Height <= 1)
            {
                return;
            }

            var maxAllowedRadius = (Math.Min(control.Width, control.Height) - 1) / 2;
            var actualRadius = Math.Max(2, Math.Min(radius, maxAllowedRadius));
            if (actualRadius < 2)
            {
                return;
            }

            using (var path = CreateRoundedRectanglePath(new Rectangle(0, 0, control.Width, control.Height), actualRadius))
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

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            if (radius < 2 || rect.Width <= 0 || rect.Height <= 0)
            {
                return null;
            }

            var diameter = radius;

            var path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void StyleButton(Button button, Color baseColor, Color textColor)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = baseColor;
            button.ForeColor = textColor;
            button.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            button.TextAlign = ContentAlignment.MiddleCenter;

            var normalColor = button.BackColor;
            var hoverColor = Color.FromArgb(
                Math.Min(255, normalColor.R + 18),
                Math.Min(255, normalColor.G + 18),
                Math.Min(255, normalColor.B + 18));
            var pressedColor = Color.FromArgb(
                Math.Max(0, normalColor.R - 14),
                Math.Max(0, normalColor.G - 14),
                Math.Max(0, normalColor.B - 14));

            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = normalColor;
            button.MouseDown += (s, e) => button.BackColor = pressedColor;
            button.MouseUp += (s, e) =>
            {
                var point = button.PointToClient(Cursor.Position);
                button.BackColor = button.ClientRectangle.Contains(point) ? hoverColor : normalColor;
            };

            button.Resize += (s, e) => ApplyRoundedRegion(button, 22);
            ApplyRoundedRegion(button, 22);
        }

        #endregion

        #region Event Handlers

        private void btnStartGame_Click(object sender, EventArgs e)
        {
            Hide();
            using (var gameForm = new GameForm(false))
            {
                gameForm.ShowDialog();
            }
            Show();
        }

        private void btnLoadGame_Click(object sender, EventArgs e)
        {
            Hide();
            using (var gameForm = new GameForm(true))
            {
                gameForm.ShowDialog();
            }
            Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            var result = BayanihanMessageBox.Show(this, "Leave Bayanihan Quest and return to desktop?", "Confirm Exit", BayanihanMessageType.Confirmation, MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void btnFeedback_Click(object sender, EventArgs e)
        {
            using (var dialog = new FeedbackDialog())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    FeedbackDialog.SaveFeedback(dialog.PlayerName, dialog.FeedbackText);
                    BayanihanMessageBox.Show(this, "Thanks for your feedback!", "Feedback", BayanihanMessageType.Success, MessageBoxButtons.OK);
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnStartGame.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                btnExit.PerformClick();
                e.Handled = true;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
                BackgroundImage = null;
            }

            base.OnFormClosed(e);
        }

        // Feedback dialog - dini-design para match sa dark theme ng BayanihanMessageBox
        private sealed class FeedbackDialog : Form
        {
            private readonly TextBox txtName;
            private readonly TextBox txtFeedback;
            private readonly Button btnSubmit;
            private readonly Button btnCancel;
            private readonly Label lblCharCount;
            private readonly Panel categoryPanel;
            private string selectedCategory = "General";

            private static readonly Color SurfaceDark = Color.FromArgb(14, 22, 38);
            private static readonly Color SurfaceCard = Color.FromArgb(18, 28, 48);
            private static readonly Color InputBg = Color.FromArgb(24, 36, 60);
            private static readonly Color InputBorder = Color.FromArgb(48, 64, 96);
            private static readonly Color InputFocusBorder = Color.FromArgb(56, 189, 248);
            private static readonly Color AccentBlue = Color.FromArgb(56, 189, 248);
            private static readonly Color TextPrimary = Color.FromArgb(233, 239, 250);
            private static readonly Color TextMuted = Color.FromArgb(148, 163, 184);
            private static readonly Color TextDim = Color.FromArgb(100, 116, 139);

            public FeedbackDialog()
            {
                Text = "Feedback";
                FormBorderStyle = FormBorderStyle.None;
                StartPosition = FormStartPosition.CenterParent;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                KeyPreview = true;
                ClientSize = new Size(560, 520);
                BackColor = SurfaceDark;
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);

                var root = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = SurfaceCard
                };
                Controls.Add(root);

                // Accent bar
                var accentBar = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 4,
                    BackColor = AccentBlue
                };
                root.Controls.Add(accentBar);

                // Header
                var header = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 64,
                    Padding = new Padding(24, 14, 16, 10),
                    BackColor = Color.Transparent
                };
                root.Controls.Add(header);

                var iconCircle = new Panel
                {
                    Size = new Size(36, 36),
                    Location = new Point(24, 14),
                    BackColor = Color.FromArgb(28, 44, 74)
                };
                header.Controls.Add(iconCircle);
                ApplyRoundedRegion(iconCircle, 18);

                var iconLabel = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = "✉",
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                    ForeColor = AccentBlue,
                    BackColor = Color.Transparent
                };
                iconCircle.Controls.Add(iconLabel);

                var titleLabel = new Label
                {
                    AutoSize = false,
                    Location = new Point(70, 12),
                    Size = new Size(ClientSize.Width - 120, 40),
                    Text = "Send Feedback",
                    ForeColor = TextPrimary,
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold)
                };
                header.Controls.Add(titleLabel);

                var closeButton = new Button
                {
                    FlatStyle = FlatStyle.Flat,
                    Text = "✕",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = TextMuted,
                    BackColor = Color.Transparent,
                    Size = new Size(32, 28),
                    Location = new Point(ClientSize.Width - 48, 16),
                    TabStop = false,
                    Cursor = Cursors.Hand
                };
                closeButton.FlatAppearance.BorderSize = 0;
                closeButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(40, 52, 76);
                closeButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(52, 66, 94);
                closeButton.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
                header.Controls.Add(closeButton);

                // Content area
                var content = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(28, 8, 28, 16),
                    BackColor = Color.Transparent
                };
                root.Controls.Add(content);

                // Category pills
                var lblCategory = CreateFieldLabel("Category");
                content.Controls.Add(lblCategory);

                categoryPanel = new Panel
                {
                    Location = new Point(28, lblCategory.Bottom + 6),
                    Size = new Size(ClientSize.Width - 56, 34),
                    BackColor = Color.Transparent
                };
                content.Controls.Add(categoryPanel);

                var categories = new[] { "General", "Bug", "Suggestion", "Praise" };
                var pillX = 0;
                foreach (var cat in categories)
                {
                    var pill = CreateCategoryPill(cat, cat == selectedCategory);
                    pill.Location = new Point(pillX, 0);
                    categoryPanel.Controls.Add(pill);
                    pillX += pill.Width + 8;
                }

                // Name field
                var lblName = CreateFieldLabel("Name (optional)");
                lblName.Location = new Point(28, categoryPanel.Bottom + 16);
                content.Controls.Add(lblName);

                txtName = new TextBox
                {
                    BackColor = InputBg,
                    ForeColor = TextPrimary,
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular)
                };

                var namePadding = new Panel
                {
                    Location = new Point(28, lblName.Bottom + 4),
                    Size = new Size(ClientSize.Width - 56, 36),
                    BackColor = InputBg,
                    Padding = new Padding(10, 7, 10, 7)
                };
                namePadding.Controls.Add(txtName);
                txtName.Dock = DockStyle.Fill;
                ApplyRoundedRegion(namePadding, 10);
                // Paint border ng input - nag-glow blue pag focused, gray pag hindi
                namePadding.Paint += (s, e) =>
                {
                    using (var pen = new Pen(txtName.Focused ? InputFocusBorder : InputBorder, 1.2f))
                    using (var path = CreateRoundedRectanglePath(new Rectangle(0, 0, namePadding.Width - 1, namePadding.Height - 1), 10))
                    {
                        if (path != null)
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.DrawPath(pen, path);
                        }
                    }
                };
                content.Controls.Add(namePadding);
                txtName.GotFocus += (s, e) => namePadding.Invalidate();
                txtName.LostFocus += (s, e) => namePadding.Invalidate();

                // Feedback field
                var lblFeedback = CreateFieldLabel("Your feedback");
                lblFeedback.Location = new Point(28, namePadding.Bottom + 14);
                content.Controls.Add(lblFeedback);

                txtFeedback = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    BackColor = InputBg,
                    ForeColor = TextPrimary,
                    BorderStyle = BorderStyle.None,
                    Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                    AcceptsReturn = true
                };
                txtFeedback.MaxLength = 2000;
                txtFeedback.TextChanged += (s, e) =>
                {
                    lblCharCount.Text = txtFeedback.Text.Length + " / 2000";
                    lblCharCount.ForeColor = txtFeedback.Text.Length > 1800
                        ? Color.FromArgb(245, 158, 11)
                        : TextDim;
                };

                var feedbackBoxHeight = 170;
                var feedbackPadding = new Panel
                {
                    Location = new Point(28, lblFeedback.Bottom + 6),
                    Size = new Size(ClientSize.Width - 56, feedbackBoxHeight),
                    BackColor = InputBg,
                    Padding = new Padding(10, 8, 10, 8)
                };
                feedbackPadding.Controls.Add(txtFeedback);
                txtFeedback.Dock = DockStyle.Fill;
                ApplyRoundedRegion(feedbackPadding, 10);
                feedbackPadding.Paint += (s, e) =>
                {
                    using (var pen = new Pen(txtFeedback.Focused ? InputFocusBorder : InputBorder, 1.2f))
                    using (var path = CreateRoundedRectanglePath(new Rectangle(0, 0, feedbackPadding.Width - 1, feedbackPadding.Height - 1), 10))
                    {
                        if (path != null)
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            e.Graphics.DrawPath(pen, path);
                        }
                    }
                };
                content.Controls.Add(feedbackPadding);
                txtFeedback.GotFocus += (s, e) => feedbackPadding.Invalidate();
                txtFeedback.LostFocus += (s, e) => feedbackPadding.Invalidate();

                lblCharCount = new Label
                {
                    Text = "0 / 2000",
                    ForeColor = TextDim,
                    Font = new Font("Segoe UI", 8F, FontStyle.Regular),
                    AutoSize = false,
                    Size = new Size(100, 18),
                    TextAlign = ContentAlignment.MiddleRight,
                    BackColor = Color.Transparent,
                    Location = new Point(ClientSize.Width - 138, feedbackPadding.Bottom + 2)
                };
                content.Controls.Add(lblCharCount);

                // Button bar
                var buttonBar = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 68,
                    FlowDirection = FlowDirection.RightToLeft,
                    Padding = new Padding(16, 14, 16, 14),
                    BackColor = Color.Transparent,
                    WrapContents = false
                };
                root.Controls.Add(buttonBar);

                btnCancel = CreateStyledButton("Cancel", false);
                btnCancel.DialogResult = DialogResult.Cancel;
                buttonBar.Controls.Add(btnCancel);

                btnSubmit = CreateStyledButton("Submit Feedback", true);
                btnSubmit.DialogResult = DialogResult.OK;
                buttonBar.Controls.Add(btnSubmit);

                AcceptButton = btnSubmit;
                CancelButton = btnCancel;

                // Validation: bago i-submit, check muna if may laman yung feedback
                btnSubmit.Click += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtFeedback.Text))
                    {
                        BayanihanMessageBox.Show(this, "Please enter feedback before submitting.", "Feedback", BayanihanMessageType.Warning, MessageBoxButtons.OK);
                        DialogResult = DialogResult.None;
                    }
                };

                // Rounded form
                Resize += (s, e) =>
                {
                    ApplyRoundedRegion(this, 16);
                    closeButton.Location = new Point(ClientSize.Width - 48, 16);
                    titleLabel.Size = new Size(ClientSize.Width - 120, 40);
                };
                ApplyRoundedRegion(this, 16);

                KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Escape)
                    {
                        DialogResult = DialogResult.Cancel;
                        Close();
                        e.Handled = true;
                    }
                };
            }

            private Label CreateFieldLabel(string text)
            {
                return new Label
                {
                    Text = text,
                    ForeColor = TextMuted,
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    AutoSize = false,
                    Size = new Size(300, 20),
                    BackColor = Color.Transparent
                };
            }

            private Button CreateCategoryPill(string text, bool selected)
            {
                var pill = new Button
                {
                    Text = text,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Size = new Size(Math.Max(72, TextRenderer.MeasureText(text, new Font("Segoe UI", 8.5F, FontStyle.Bold)).Width + 24), 30),
                    Cursor = Cursors.Hand,
                    BackColor = selected ? Color.FromArgb(28, 50, 88) : Color.FromArgb(24, 36, 60),
                    ForeColor = selected ? AccentBlue : TextMuted
                };
                pill.FlatAppearance.BorderSize = 0;
                pill.FlatAppearance.MouseOverBackColor = Color.FromArgb(34, 54, 88);

                var baseColor = pill.BackColor;
                var hoverColor = Color.FromArgb(
                    Math.Min(255, baseColor.R + 12),
                    Math.Min(255, baseColor.G + 12),
                    Math.Min(255, baseColor.B + 12));
                pill.MouseEnter += (s, e) => pill.BackColor = hoverColor;
                pill.MouseLeave += (s, e) => pill.BackColor = baseColor;

                ApplyRoundedRegion(pill, 10);

                // Category selection: highlight yung selected pill, reset yung iba
                pill.Click += (s, e) =>
                {
                    selectedCategory = text;
                    foreach (Button p in categoryPanel.Controls)
                    {
                        var isSel = p.Text == text;
                        p.BackColor = isSel ? Color.FromArgb(28, 50, 88) : Color.FromArgb(24, 36, 60);
                        p.ForeColor = isSel ? AccentBlue : TextMuted;
                    }
                };

                return pill;
            }

            private Button CreateStyledButton(string text, bool primary)
            {
                var button = new Button
                {
                    Text = text,
                    FlatStyle = FlatStyle.Flat,
                    Width = primary ? 160 : 110,
                    Height = 38,
                    Margin = new Padding(8, 0, 0, 0),
                    BackColor = primary ? AccentBlue : Color.FromArgb(40, 54, 80),
                    ForeColor = primary ? Color.FromArgb(10, 16, 28) : TextPrimary,
                    Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

                button.FlatAppearance.BorderSize = 0;
                var baseColor = button.BackColor;
                var hover = ChangeColor(baseColor, 14);
                var pressed = ChangeColor(baseColor, -14);

                button.MouseEnter += (s, e) => button.BackColor = hover;
                button.MouseLeave += (s, e) => button.BackColor = baseColor;
                button.MouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Left) button.BackColor = pressed;
                };
                button.MouseUp += (s, e) =>
                {
                    var pt = button.PointToClient(Cursor.Position);
                    button.BackColor = button.ClientRectangle.Contains(pt) ? hover : baseColor;
                };

                button.Resize += (s, e) => ApplyRoundedRegion(button, 12);
                ApplyRoundedRegion(button, 12);

                return button;
            }

            private static Color ChangeColor(Color color, int delta)
            {
                return Color.FromArgb(color.A,
                    Math.Max(0, Math.Min(255, color.R + delta)),
                    Math.Max(0, Math.Min(255, color.G + delta)),
                    Math.Max(0, Math.Min(255, color.B + delta)));
            }

            public string PlayerName
            {
                get { return txtName.Text.Trim(); }
            }

            public string FeedbackText
            {
                get { return txtFeedback.Text.Trim(); }
            }

            // Save feedback sa local file (AppData) - may timestamp at name
            public static void SaveFeedback(string name, string feedback)
            {
                if (string.IsNullOrWhiteSpace(feedback))
                {
                    return;
                }

                var folder = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "Windows_form_game_V1.0");
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                }

                var filePath = System.IO.Path.Combine(folder, "feedback.txt");
                var builder = new StringBuilder();
                builder.AppendLine("=== Feedback ===");
                builder.AppendLine("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                if (!string.IsNullOrWhiteSpace(name))
                {
                    builder.AppendLine("Name: " + name);
                }
                builder.AppendLine("Message:");
                builder.AppendLine(feedback);
                builder.AppendLine();

                System.IO.File.AppendAllText(filePath, builder.ToString());
            }
        }

        #endregion
    }
}