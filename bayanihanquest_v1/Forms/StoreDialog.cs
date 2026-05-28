using System;
using System.Drawing;
using System.Windows.Forms;

namespace Windows_form_game_V1._0.Forms
{
    // Store dialog - binibili dito yung gloves, energy drink, at trash bag
    internal sealed class StoreDialog : Form
    {
        public StoreDialog(Action buyGloves, Action buyEnergyDrink, Action buyTrashBag)
        {
            Text = "Store";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ClientSize = new Size(420, 250);
            BackColor = ColorTranslator.FromHtml("#0f172a");

            var title = new Label
            {
                Text = "Store",
                ForeColor = ColorTranslator.FromHtml("#38bdf8"),
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Bounds = new Rectangle(20, 16, 280, 34)
            };

            var btnGloves = CreateStoreButton("Gloves (faster cleaning) - ₱100", new Rectangle(20, 62, 380, 42));
            var btnEnergy = CreateStoreButton("Energy Drink (+40 stamina) - ₱30", new Rectangle(20, 110, 380, 42));
            var btnBag = CreateStoreButton("Trash Bag (+10 capacity) - ₱60", new Rectangle(20, 158, 380, 42));

            btnGloves.Click += (s, e) => buyGloves();
            btnEnergy.Click += (s, e) => buyEnergyDrink();
            btnBag.Click += (s, e) => buyTrashBag();

            Controls.Add(title);
            Controls.Add(btnGloves);
            Controls.Add(btnEnergy);
            Controls.Add(btnBag);
        }

        private static Button CreateStoreButton(string text, Rectangle bounds)
        {
            var button = new Button
            {
                Text = text,
                Bounds = bounds,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = ColorTranslator.FromHtml("#334155")
            };

            button.FlatAppearance.BorderSize = 0;
            return button;
        }
    }
}
