using System;

namespace FarmFarmer.Core.Combat
{
    // Pure functions only -- no Unity/SO dependency, mirrors SkillProgression's placement/style.
    // Numbers passed in are placeholders owned by CombatBalanceDefinition (FarmFarmer.Data);
    // this class only owns the shape of the formulas, not the tuning.
    public static class CombatMath
    {
        public const int FloorsPerStage = 10;

        public static bool IsMinibossStage(int stageNumber) => stageNumber % 10 == 0;
        public static bool IsBossStage(int stageNumber) => stageNumber % 25 == 0;

        // Miniboss/boss stages are single-encounter (Decision 4) -- no floor grind before them.
        public static int FloorsRequiredForStage(bool isMiniboss, bool isBoss) =>
            (isMiniboss || isBoss) ? 1 : FloorsPerStage;

        public static BigDouble StageEnemyHp(int stage, BigDouble baseHp, float hpGrowthPerStage,
            bool isMiniboss, bool isBoss, float minibossMult, float bossMult)
        {
            var hp = baseHp * Math.Pow(hpGrowthPerStage, stage - 1);
            if (isBoss) return hp * bossMult;
            if (isMiniboss) return hp * minibossMult;
            return hp;
        }

        public static BigDouble StageCoinReward(int stage, BigDouble baseReward, float coinGrowthPerStage,
            bool isMiniboss, bool isBoss, float minibossMult, float bossMult)
        {
            var coin = baseReward * Math.Pow(coinGrowthPerStage, stage - 1);
            if (isBoss) return coin * bossMult;
            if (isMiniboss) return coin * minibossMult;
            return coin;
        }

        public static BigDouble HeroDps(int heroLevel, BigDouble baseDps, float statOffset,
            float statMultiplier, float dpsGrowthPerLevel)
        {
            if (heroLevel <= 0) return BigDouble.Zero;
            var perLevel = baseDps * Math.Pow(dpsGrowthPerLevel, heroLevel - 1);
            return (perLevel + statOffset) * statMultiplier;
        }

        public static BigDouble TapDamage(BigDouble currentDps, float tapSecondsEquivalent) =>
            currentDps * tapSecondsEquivalent;

        // Seam only -- Open Question #1's background-progression formula is still TBD. Nothing
        // calls this yet; exists so the eventual formula doesn't require touching call sites.
        public static BigDouble BackgroundDps(BigDouble focusedDps, float backgroundMultiplier) =>
            focusedDps * backgroundMultiplier;

        // Decision 4: failing a miniboss/boss knocks the player back one stage. Clamped so a
        // hypothetical early fail can't push below stage 1 (stage 1 is never a boss anyway).
        public static int KnockbackStage(int stageNumber) => Math.Max(1, stageNumber - 1);

        // Seed payout for ONE hero, from the v0.3 sim: floor((highest - floor) / step), never
        // negative. Whether payouts sum across the roster is the caller's business -- that part
        // is a sim assumption, not a confirmed decision.
        public static int SeedsEarnedForHighestStage(int highestStage, int seedStageFloor, int seedStageStep)
        {
            if (seedStageStep <= 0) return 0;
            return Math.Max(0, (highestStage - seedStageFloor) / seedStageStep);
        }

        // Both readings of Decision 7's "+15% per Seed" live here so the additive-vs-compounding
        // call (open TODO -- additive stalls by run 3 in the v0.3 sim) is a data toggle, not a
        // code change. Plain double is fine: seed counts stay small even deep into the game.
        public static double SeedDpsMultiplier(double seedsHeld, float bonusPerSeed, bool compounding)
        {
            if (seedsHeld <= 0) return 1.0;
            return compounding
                ? Math.Pow(1.0 + bonusPerSeed, seedsHeld)
                : 1.0 + bonusPerSeed * seedsHeld;
        }
    }
}
