using System;
using System.Collections.Generic;

namespace FarmFarmer.Core
{
    [Serializable]
    public class SaveData
    {
        public int schemaVersion = SaveService.CurrentSchemaVersion;
        public string savedAtUtc; // ISO 8601 -- string so JsonUtility round-trips it without help

        public BigDouble sharedWallet;
        public BigDouble prestigeCurrency; // "Harvest Seeds" name is pending, CLAUDE.md Open Q #4

        // Per-hero state as a list from day one (CLAUDE.md), even with only one hero unlocked.
        public List<HeroSaveState> heroes = new List<HeroSaveState>();
        public List<GeneratorSaveState> generators = new List<GeneratorSaveState>();
    }

    [Serializable]
    public class HeroSaveState
    {
        public string heroId;
        public bool isUnlocked;
        public int currentStage;
        public int level;
        public double xp;
        public string lastFocusedUtc; // feeds the focused/background progression split on load
    }

    [Serializable]
    public class GeneratorSaveState
    {
        public string generatorId;
        public int level;
        public double xp;
    }
}
