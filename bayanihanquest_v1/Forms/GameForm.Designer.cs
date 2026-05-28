namespace Windows_form_game_V1._0.Forms
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// </summary>
        /// </summary>
        /// </summary>
        /// 
        /// 
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
            this.components = new System.ComponentModel.Container();
            this.panelHud = new System.Windows.Forms.Panel();
            this.btnLoadGame = new System.Windows.Forms.Button();
            this.btnSaveGame = new System.Windows.Forms.Button();
            this.btnBackToMenu = new System.Windows.Forms.Button();
            this.lblHint = new System.Windows.Forms.Label();
            this.lblTrashCount = new System.Windows.Forms.Label();
            this.lblQuestProgress = new System.Windows.Forms.Label();
            this.lblActiveQuest = new System.Windows.Forms.Label();
            this.lblReputation = new System.Windows.Forms.Label();
            this.lblMoney = new System.Windows.Forms.Label();
            this.panelStaminaBack = new System.Windows.Forms.Panel();
            this.panelStaminaFill = new System.Windows.Forms.Panel();
            this.lblStaminaTitle = new System.Windows.Forms.Label();
            this.panelMap = new System.Windows.Forms.Panel();
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.panelHud.SuspendLayout();
            this.panelStaminaBack.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHud
            // 
            this.panelHud.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(41)))), ((int)(((byte)(59)))));
            this.panelHud.Controls.Add(this.btnBackToMenu);
            this.panelHud.Controls.Add(this.btnLoadGame);
            this.panelHud.Controls.Add(this.btnSaveGame);
            this.panelHud.Controls.Add(this.lblHint);
            this.panelHud.Controls.Add(this.lblTrashCount);
            this.panelHud.Controls.Add(this.lblQuestProgress);
            this.panelHud.Controls.Add(this.lblActiveQuest);
            this.panelHud.Controls.Add(this.lblReputation);
            this.panelHud.Controls.Add(this.lblMoney);
            this.panelHud.Controls.Add(this.panelStaminaBack);
            this.panelHud.Controls.Add(this.lblStaminaTitle);
            this.panelHud.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelHud.Location = new System.Drawing.Point(1180, 0);
            this.panelHud.Name = "panelHud";
            this.panelHud.Size = new System.Drawing.Size(320, 840);
            this.panelHud.TabIndex = 0;
            // 
            // btnLoadGame
            // 
            this.btnLoadGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLoadGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadGame.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnLoadGame.ForeColor = System.Drawing.Color.White;
            this.btnLoadGame.Location = new System.Drawing.Point(154, 733);
            this.btnLoadGame.Name = "btnLoadGame";
            this.btnLoadGame.Size = new System.Drawing.Size(142, 36);
            this.btnLoadGame.TabIndex = 14;
            this.btnLoadGame.Text = "Load";
            this.btnLoadGame.UseVisualStyleBackColor = true;
            this.btnLoadGame.Click += new System.EventHandler(this.btnLoadGame_Click);
            // 
            // btnSaveGame
            // 
            this.btnSaveGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveGame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveGame.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnSaveGame.ForeColor = System.Drawing.Color.White;
            this.btnSaveGame.Location = new System.Drawing.Point(23, 733);
            this.btnSaveGame.Name = "btnSaveGame";
            this.btnSaveGame.Size = new System.Drawing.Size(125, 36);
            this.btnSaveGame.TabIndex = 13;
            this.btnSaveGame.Text = "Save";
            this.btnSaveGame.UseVisualStyleBackColor = true;
            this.btnSaveGame.Click += new System.EventHandler(this.btnSaveGame_Click);
            // 
            // btnBackToMenu
            // 
            this.btnBackToMenu.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBackToMenu.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackToMenu.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnBackToMenu.ForeColor = System.Drawing.Color.White;
            this.btnBackToMenu.Location = new System.Drawing.Point(23, 781);
            this.btnBackToMenu.Name = "btnBackToMenu";
            this.btnBackToMenu.Size = new System.Drawing.Size(274, 42);
            this.btnBackToMenu.TabIndex = 15;
            this.btnBackToMenu.Text = "Back to Menu";
            this.btnBackToMenu.UseVisualStyleBackColor = true;
            this.btnBackToMenu.Click += new System.EventHandler(this.btnBackToMenu_Click);
            // 
            // lblHint
            // 
            this.lblHint.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblHint.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblHint.Location = new System.Drawing.Point(19, 344);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(278, 41);
            this.lblHint.TabIndex = 8;
            this.lblHint.Text = "Move with W A S D";
            // 
            // lblTrashCount
            // 
            this.lblTrashCount.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblTrashCount.ForeColor = System.Drawing.Color.White;
            this.lblTrashCount.Location = new System.Drawing.Point(19, 302);
            this.lblTrashCount.Name = "lblTrashCount";
            this.lblTrashCount.Size = new System.Drawing.Size(278, 30);
            this.lblTrashCount.TabIndex = 7;
            this.lblTrashCount.Text = "Trash Collected: 0";
            // 
            // lblQuestProgress
            // 
            this.lblQuestProgress.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblQuestProgress.ForeColor = System.Drawing.Color.Gainsboro;
            this.lblQuestProgress.Location = new System.Drawing.Point(19, 241);
            this.lblQuestProgress.Name = "lblQuestProgress";
            this.lblQuestProgress.Size = new System.Drawing.Size(278, 58);
            this.lblQuestProgress.TabIndex = 6;
            this.lblQuestProgress.Text = "Progress";
            // 
            // lblActiveQuest
            // 
            this.lblActiveQuest.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblActiveQuest.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.lblActiveQuest.Location = new System.Drawing.Point(19, 182);
            this.lblActiveQuest.Name = "lblActiveQuest";
            this.lblActiveQuest.Size = new System.Drawing.Size(278, 49);
            this.lblActiveQuest.TabIndex = 5;
            this.lblActiveQuest.Text = "Active Quest";
            // 
            // lblReputation
            // 
            this.lblReputation.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblReputation.ForeColor = System.Drawing.Color.White;
            this.lblReputation.Location = new System.Drawing.Point(19, 145);
            this.lblReputation.Name = "lblReputation";
            this.lblReputation.Size = new System.Drawing.Size(278, 30);
            this.lblReputation.TabIndex = 4;
            this.lblReputation.Text = "Reputation: 0";
            // 
            // lblMoney
            // 
            this.lblMoney.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblMoney.ForeColor = System.Drawing.Color.White;
            this.lblMoney.Location = new System.Drawing.Point(19, 20);
            this.lblMoney.Name = "lblMoney";
            this.lblMoney.Size = new System.Drawing.Size(278, 30);
            this.lblMoney.TabIndex = 3;
            this.lblMoney.Text = "Money: ₱0";
            // 
            // panelStaminaBack
            // 
            this.panelStaminaBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.panelStaminaBack.Controls.Add(this.panelStaminaFill);
            this.panelStaminaBack.Location = new System.Drawing.Point(23, 104);
            this.panelStaminaBack.Name = "panelStaminaBack";
            this.panelStaminaBack.Size = new System.Drawing.Size(274, 26);
            this.panelStaminaBack.TabIndex = 2;
            // 
            // panelStaminaFill
            // 
            this.panelStaminaFill.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this.panelStaminaFill.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelStaminaFill.Location = new System.Drawing.Point(0, 0);
            this.panelStaminaFill.Name = "panelStaminaFill";
            this.panelStaminaFill.Size = new System.Drawing.Size(274, 26);
            this.panelStaminaFill.TabIndex = 0;
            // 
            // lblStaminaTitle
            // 
            this.lblStaminaTitle.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblStaminaTitle.ForeColor = System.Drawing.Color.White;
            this.lblStaminaTitle.Location = new System.Drawing.Point(19, 63);
            this.lblStaminaTitle.Name = "lblStaminaTitle";
            this.lblStaminaTitle.Size = new System.Drawing.Size(278, 30);
            this.lblStaminaTitle.TabIndex = 1;
            this.lblStaminaTitle.Text = "Stamina";
            // 
            // panelMap
            // 
            this.panelMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.panelMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMap.Location = new System.Drawing.Point(0, 0);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(1180, 840);
            this.panelMap.TabIndex = 1;
            // 
            // gameTimer
            // 
            this.gameTimer.Interval = 16;
            this.gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(1500, 840);
            this.Controls.Add(this.panelMap);
            this.Controls.Add(this.panelHud);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Barangay Quest: Clean & Earn";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyUp);
            this.panelHud.ResumeLayout(false);
            this.panelStaminaBack.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelHud;
        private System.Windows.Forms.Panel panelMap;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Label lblStaminaTitle;
        private System.Windows.Forms.Panel panelStaminaBack;
        private System.Windows.Forms.Panel panelStaminaFill;
        private System.Windows.Forms.Label lblMoney;
        private System.Windows.Forms.Label lblReputation;
        private System.Windows.Forms.Label lblActiveQuest;
        private System.Windows.Forms.Label lblQuestProgress;
        private System.Windows.Forms.Label lblTrashCount;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.Button btnLoadGame;
        private System.Windows.Forms.Button btnSaveGame;
        private System.Windows.Forms.Button btnBackToMenu;
    }
}
