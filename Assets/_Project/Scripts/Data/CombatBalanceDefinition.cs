using UnityEngine;
using FarmFarmer.Core;
using FarmFarmer.Core.Combat;

namespace FarmFarmer.Data
{
    // One asset instance drives all combat tuning. Every field here is a PLACEHOLDER pending the
    // economy re-model (CLAUDE.md Open Question #5) -- final numbers drop in via Inspector, no
    // code change needed. This class only holds numbers; CombatMath owns the actual formulas.
    [CreateAssetMenu(menuName = "Farm Farmer/Combat Balance Definition", fileName = "SO_CombatBalanceDefinition")]
    public class CombatBalanceDefinition : ScriptableObject
    {
        [Header("PLACEHOLDER -- stage HP curve, pending economy re-model")]
        public BigDouble baseEnemyHp = new BigDouble(10, 0);
        public float hpGrowthPerStage = 1.12f; // per STAGE, never per floor -- Decision 4
        public float minibossHpMultiplier = 5f;
        public float bossHpMultiplier = 20f;

        [Header("PLACEHOLDER -- coin reward curve")]
        public BigDouble baseCoinReward = new BigDouble(1, 0);
        public float coinGrowthPerStage = 1.10f;
        public float minibossCoinMultiplier = 5f;
        public float bossCoinMultiplier = 20f;

        [Header("PLACEHOLDER -- hero DPS curve; per-hero offset/multiplier applied on top, Decision 6")]
        public BigDouble baseHeroDps = new BigDouble(5, 0);
        public float dpsGrowthPerLevel = 1.07f;

        [Header("PLACEHOLDER -- tap model: tap = N seconds' worth of current DPS (confirmed w/ Derek)")]
        public float tapDamageSecondsEquivalent = 1f;

        [Header("PLACEHOLDER -- background hero rate, Decision 6 / Open Question #1, formula TBD")]
        public float backgroundDpsMultiplier = 0.1f;

        public bool IsMinibossStage(int stage) => CombatMath.IsMinibossStage(stage);
        public bool IsBossStage(int stage) => CombatMath.IsBossStage(stage);

        public int FloorsRequiredForStage(int stage) =>
            CombatMath.FloorsRequiredForStage(IsMinibossStage(stage), IsBossStage(stage));

        public BigDouble EnemyHpForStage(int stage) =>
            CombatMath.StageEnemyHp(stage, baseEnemyHp, hpGrowthPerStage,
                IsMinibossStage(stage), IsBossStage(stage), minibossHpMultiplier, bossHpMultiplier);

        public BigDouble CoinRewardForStage(int stage) =>
            CombatMath.StageCoinReward(stage, baseCoinReward, coinGrowthPerStage,
                IsMinibossStage(stage), IsBossStage(stage), minibossCoinMultiplier, bossCoinMultiplier);

        public BigDouble HeroDps(HeroDefinition hero, int level) =>
            CombatMath.HeroDps(level, baseHeroDps, hero.statOffset, hero.statMultiplier, dpsGrowthPerLevel);

        public BigDouble TapDamage(BigDouble currentDps) =>
            CombatMath.TapDamage(currentDps, tapDamageSecondsEquivalent);

        public BigDouble BackgroundDps(BigDouble focusedDps) =>
            CombatMath.BackgroundDps(focusedDps, backgroundDpsMultiplier);
    }
}
