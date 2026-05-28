using System;
using System.Drawing;
using System.Windows.Forms;

namespace Windows_form_game_V1._0.Forms
{
    // Loading screen - nagpapakita ng progress bago mag-main menu
    public class LoadingForm : Form
    {
        private readonly Timer loadingTimer;
        private readonly Label lblTitle;
        private readonly Label lblStatus;
        private readonly ProgressBar progressBar;
        private int elapsedMs;
        private bool launched;

        public LoadingForm()
        {
            BackColor = Color.FromArgb(18, 28, 46);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(560, 280);
            DoubleBuffered = true;
            ShowInTaskbar = true;

            lblTitle = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 124,
                Text = "BAYANIHAN QUEST",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 28F, FontStyle.Bold),
                ForeColor = Color.FromArgb(251, 192, 45),
                BackColor = Color.Transparent
            };

            lblStatus = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Bottom,
                Height = 46,
                Text = "Preparing your barangay...",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Regular),
                ForeColor = Color.FromArgb(235, 240, 225),
                BackColor = Color.Transparent
            };

            progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Continuous,
                Maximum = 100,
                Value = 0,
                Width = 440,
                Height = 20,
                Left = (ClientSize.Width - 440) / 2,
                Top = 176,
                ForeColor = Color.FromArgb(46, 125, 50)
            };

            Controls.Add(lblTitle);
            Controls.Add(progressBar);
            Controls.Add(lblStatus);

            loadingTimer = new Timer { Interval = 25 };
            loadingTimer.Tick += LoadingTimer_Tick;

            Shown += (s, e) => loadingTimer.Start();
        }

        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            elapsedMs += loadingTimer.Interval;
            var progress = Math.Min(100, (elapsedMs * 100) / 1500);
            progressBar.Value = progress;

            if (progress >= 100)
            {
                loadingTimer.Stop();
                LaunchMainMenu();
            }
        }

        private void LaunchMainMenu()
        {
            if (launched)
            {
                return;
            }

            launched = true;
            var mainForm = new MainMenuForm();
            mainForm.FormClosed += (s, e) => Close();
            mainForm.Show();
            Hide();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (loadingTimer != null)
                {
                    loadingTimer.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
