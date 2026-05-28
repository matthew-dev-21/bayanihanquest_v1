namespace Windows_form_game_V1._0.Forms
{
    partial class MainMenuForm
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
            this.menuCard = new System.Windows.Forms.Panel();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.btnFeedback = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLoadGame = new System.Windows.Forms.Button();
            this.btnStartGame = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblFooter = new System.Windows.Forms.Label();
            this.menuCard.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuCard
            // 
            this.menuCard.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.menuCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(27)))), ((int)(((byte)(49)))), ((int)(((byte)(22)))));
            this.menuCard.Controls.Add(this.lblSubtitle);
            this.menuCard.Controls.Add(this.btnFeedback);
            this.menuCard.Controls.Add(this.btnExit);
            this.menuCard.Controls.Add(this.btnLoadGame);
            this.menuCard.Controls.Add(this.btnStartGame);
            this.menuCard.Controls.Add(this.lblTitle);
            this.menuCard.Location = new System.Drawing.Point(360, 58);
            this.menuCard.Name = "menuCard";
            this.menuCard.Size = new System.Drawing.Size(560, 596);
            this.menuCard.TabIndex = 0;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblSubtitle.ForeColor = System.Drawing.Color.Ivory;
            this.lblSubtitle.Location = new System.Drawing.Point(54, 156);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(452, 34);
            this.lblSubtitle.TabIndex = 4;
            this.lblSubtitle.Text = "Clean, help, and earn for your barangay.";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnFeedback
            // 
            this.btnFeedback.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFeedback.Location = new System.Drawing.Point(112, 432);
            this.btnFeedback.Name = "btnFeedback";
            this.btnFeedback.Size = new System.Drawing.Size(335, 60);
            this.btnFeedback.TabIndex = 3;
            this.btnFeedback.Text = "FEEDBACK";
            this.btnFeedback.UseVisualStyleBackColor = true;
            this.btnFeedback.Click += new System.EventHandler(this.btnFeedback_Click);
            // 
            // btnExit
            // 
            this.btnExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExit.Location = new System.Drawing.Point(112, 508);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(335, 60);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "OPTIONS";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLoadGame
            // 
            this.btnLoadGame.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLoadGame.Location = new System.Drawing.Point(112, 356);
            this.btnLoadGame.Name = "btnLoadGame";
            this.btnLoadGame.Size = new System.Drawing.Size(335, 60);
            this.btnLoadGame.TabIndex = 2;
            this.btnLoadGame.Text = "CONTINUE";
            this.btnLoadGame.UseVisualStyleBackColor = true;
            this.btnLoadGame.Click += new System.EventHandler(this.btnLoadGame_Click);
            // 
            // btnStartGame
            // 
            this.btnStartGame.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartGame.Location = new System.Drawing.Point(112, 280);
            this.btnStartGame.Name = "btnStartGame";
            this.btnStartGame.Size = new System.Drawing.Size(335, 60);
            this.btnStartGame.TabIndex = 1;
            this.btnStartGame.Text = "START";
            this.btnStartGame.UseVisualStyleBackColor = true;
            this.btnStartGame.Click += new System.EventHandler(this.btnStartGame_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Black", 42F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(241)))), ((int)(((byte)(161)))));
            this.lblTitle.Location = new System.Drawing.Point(38, 26);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(488, 120);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "BAYANIHAN\r\nQUEST";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblFooter
            // 
            this.lblFooter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblFooter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFooter.ForeColor = System.Drawing.Color.White;
            this.lblFooter.Location = new System.Drawing.Point(18, 688);
            this.lblFooter.Name = "lblFooter";
            this.lblFooter.Size = new System.Drawing.Size(446, 23);
            this.lblFooter.TabIndex = 1;
            this.lblFooter.Text = "Press ESC to exit";
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 720);
            this.Controls.Add(this.lblFooter);
            this.Controls.Add(this.menuCard);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainMenuForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Barangay Quest: Clean & Earn";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.menuCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel menuCard;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnStartGame;
        private System.Windows.Forms.Button btnLoadGame;
        private System.Windows.Forms.Button btnFeedback;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblFooter;
    }
}

