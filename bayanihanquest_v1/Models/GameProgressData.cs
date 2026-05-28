using System;

namespace Windows_form_game_V1._0.Models
{
    public sealed class GameProgressData
    {
        public int Money { get; set; }
        public int Reputation { get; set; }
        public int Stamina { get; set; }
        public int TrashCapacity { get; set; }
        public bool HasGloves { get; set; }
        public int TrashCollected { get; set; }
        public int TotalTrashCollected { get; set; }
        public int ChickenCollected { get; set; }
        public int CoconutCollected { get; set; }
        public bool PantryMissionUnlocked { get; set; }
        public bool PantryMissionInProgress { get; set; }
        public bool PantryMissionCompleted { get; set; }
        public int InGameDay { get; set; }
        public int BarangayFund { get; set; }
        public int DailyMissionStreak { get; set; }
        public int DailyTrashTarget { get; set; }
        public int DailyTrashProgress { get; set; }
        public bool DailyMissionCompleted { get; set; }
        public int StreetlightLevel { get; set; }
        public int WasteBinLevel { get; set; }
        public int GardenLevel { get; set; }
        public int LastNpcRequestDay { get; set; }
        public int MiniGamesCompleted { get; set; }
        public bool CommunityEventActive { get; set; }
        public string CommunityEventName { get; set; }
        public int CommunityEventGoal { get; set; }
        public int CommunityEventProgress { get; set; }
        public int CommunityEventEndDay { get; set; }
        public int CommunityEventsCompleted { get; set; }
        [Obsolete("Use CoconutCollected. Kept for backward compatibility with older save files.")]
        public int PapayaCollected { get; set; }
        public string CurrentMapKey { get; set; }
        public int PlayerX { get; set; }
        public int PlayerY { get; set; }
        public QuestProgressData ActiveQuest { get; set; }
        public QuestProgressData Map2Quest { get; set; }
    }

    public sealed class QuestProgressData
    {
        public QuestStatus Status { get; set; }
        public int CurrentProgress { get; set; }
    }
}
