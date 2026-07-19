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

        // Deliberately NOT reset by PrestigeService (Open Q #8 assumption: gems are meta
        // progression, so they survive The Harvest like the estate does).
        public BigDouble gems;

        // Which hero is actively focused (Decision 6) -- HeroSaveState.lastFocusedUtc alone can't
        // reliably answer this (ties, first-ever save), so it's tracked explicitly here.
        public string focusedHeroId;

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
        public int currentFloor; // 1-10 within currentStage

        // High-water mark. Needed because knockback (Decision 4) makes currentStage
        // non-monotonic, and the prestige seed payout reads the deepest stage ever reached.
        // Zero on saves older than schema v2 -- RosterService.Hydrate backfills from currentStage.
        public int highestStageReached;
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
