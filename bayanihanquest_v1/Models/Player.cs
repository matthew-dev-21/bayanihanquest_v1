using System;

namespace Windows_form_game_V1._0.Models
{
    // Player model - pera, stamina, reputation, at inventory
    public class Player
    {
        public int Money { get; private set; }
        public int Stamina { get; private set; }
        public int Reputation { get; private set; }
        public int Speed { get; set; }

        public int TrashCollected { get; set; }
        public int TotalTrashCollected { get; set; }
        public int TrashCapacity { get; set; }

        public bool HasGloves { get; set; }

        public Player()
        {
            Money = 100;
            Stamina = 100;
            Speed = 5;
            TrashCapacity = 25;
        }

        public void LoadState(int money, int reputation, int stamina, int trashCapacity, bool hasGloves, int trashCollected, int totalTrashCollected)
        {
            Money = Math.Max(0, money);
            Reputation = Math.Max(0, reputation);
            Stamina = Math.Max(0, Math.Min(100, stamina));
            TrashCapacity = Math.Max(1, trashCapacity);
            HasGloves = hasGloves;
            TrashCollected = Math.Max(0, trashCollected);
            TotalTrashCollected = Math.Max(0, totalTrashCollected);
        }

        public void ConsumeStamina(int amount)
        {
            Stamina -= amount;
            if (Stamina < 0)
            {
                Stamina = 0;
            }
        }

        public void RecoverStamina(int amount)
        {
            Stamina += amount;
            if (Stamina > 100)
            {
                Stamina = 100;
            }
        }

        public void AddReward(int money, int reputation)
        {
            Money += money;
            Reputation += reputation;
        }

        // Important: gamitin pag may binibili - nagre-return false pag kulang pera
        public bool TrySpend(int amount)
        {
            if (Money < amount)
            {
                return false;
            }

            Money -= amount;
            return true;
        }
    }
}
