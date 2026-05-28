namespace Windows_form_game_V1._0.Models
{
    public enum QuestStatus
    {
        NotStarted,
        InProgress,
        Completed
    }

    // Quest model - may status na NotStarted, InProgress, o Completed
    public class Quest
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public int ObjectiveCount { get; private set; }
        public int RewardMoney { get; private set; }
        public int RewardReputation { get; private set; }

        public int CurrentProgress { get; private set; }
        public QuestStatus Status { get; private set; }

        public Quest(string title, string description, int objectiveCount, int rewardMoney, int rewardReputation)
        {
            Title = title;
            Description = description;
            ObjectiveCount = objectiveCount;
            RewardMoney = rewardMoney;
            RewardReputation = rewardReputation;
            Status = QuestStatus.NotStarted;
        }

        public void LoadState(QuestStatus status, int currentProgress)
        {
            Status = status;

            if (Status == QuestStatus.NotStarted)
            {
                CurrentProgress = 0;
                return;
            }

            if (Status == QuestStatus.Completed)
            {
                CurrentProgress = ObjectiveCount;
                return;
            }

            CurrentProgress = currentProgress < 0 ? 0 : currentProgress;
            if (CurrentProgress >= ObjectiveCount)
            {
                CurrentProgress = ObjectiveCount;
                Status = QuestStatus.Completed;
            }
        }

        public void Start()
        {
            if (Status == QuestStatus.NotStarted)
            {
                Status = QuestStatus.InProgress;
            }
        }

        // Tumatanggap ng progress - auto-complete pag na-reach yung goal
        public void AddProgress(int amount)
        {
            if (Status != QuestStatus.InProgress)
            {
                return;
            }

            CurrentProgress += amount;
            if (CurrentProgress >= ObjectiveCount)
            {
                CurrentProgress = ObjectiveCount;
                Status = QuestStatus.Completed;
            }
        }

        public void ResetForRepeat()
        {
            CurrentProgress = 0;
            Status = QuestStatus.NotStarted;
        }
    }
}
