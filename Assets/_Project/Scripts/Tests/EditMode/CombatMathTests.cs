using NUnit.Framework;
using FarmFarmer.Core;
using FarmFarmer.Core.Combat;

namespace FarmFarmer.Tests
{
    public class CombatMathTests
    {
        private const double Tolerance = 1e-9;

        [Test]
        public void BossCadence_Every25_MinibossEvery10()
        {
            Assert.IsTrue(CombatMath.IsBossStage(25));
            Assert.IsTrue(CombatMath.IsBossStage(50));
            Assert.IsTrue(CombatMath.IsBossStage(100));
            Assert.IsFalse(CombatMath.IsBossStage(10));

            Assert.IsTrue(CombatMath.IsMinibossStage(10));
            Assert.IsTrue(CombatMath.IsMinibossStage(20));
            Assert.IsFalse(CombatMath.IsMinibossStage(25));

            // Stage 50 satisfies BOTH raw predicates -- every caller (controller, balance SO,
            // HUD) checks boss first, so boss wins. This test pins that overlap as known.
            Assert.IsTrue(CombatMath.IsMinibossStage(50) && CombatMath.IsBossStage(50));
        }

        [Test]
        public void BossStages_AreSingleEncounter()
        {
            Assert.AreEqual(CombatMath.FloorsPerStage, CombatMath.FloorsRequiredForStage(false, false));
            Assert.AreEqual(1, CombatMath.FloorsRequiredForStage(true, false));
            Assert.AreEqual(1, CombatMath.FloorsRequiredForStage(false, true));
        }

        [Test]
        public void StageEnemyHp_GrowsPerStage_AndAppliesTierMultipliers()
        {
            BigDouble baseHp = 10d;
            var normal = CombatMath.StageEnemyHp(2, baseHp, 1.5f, false, false, 5f, 20f);
            Assert.AreEqual(15d, normal.ToDouble(), Tolerance);

            var boss = CombatMath.StageEnemyHp(1, baseHp, 1.5f, false, true, 5f, 20f);
            Assert.AreEqual(200d, boss.ToDouble(), Tolerance);

            var miniboss = CombatMath.StageEnemyHp(1, baseHp, 1.5f, true, false, 5f, 20f);
            Assert.AreEqual(50d, miniboss.ToDouble(), Tolerance);
        }

        [Test]
        public void TapDamage_IsSecondsWorthOfDps()
        {
            var tap = CombatMath.TapDamage(new BigDouble(100d), 1.5f);
            Assert.AreEqual(150d, tap.ToDouble(), Tolerance);
        }

        [Test]
        public void HeroDps_LevelZeroContributesNothing()
        {
            Assert.AreEqual(BigDouble.Zero, CombatMath.HeroDps(0, 5d, 0f, 1f, 1.07f));
        }

        [Test]
        public void KnockbackStage_DropsOneStage_NeverBelowOne()
        {
            Assert.AreEqual(24, CombatMath.KnockbackStage(25));
            Assert.AreEqual(9, CombatMath.KnockbackStage(10));
            Assert.AreEqual(1, CombatMath.KnockbackStage(1));
        }

        [Test]
        public void SeedsEarned_FloorsBelowThresholdPayNothing()
        {
            // v0.3 sim formula: floor((highest - 40) / 5), clamped at zero.
            Assert.AreEqual(0, CombatMath.SeedsEarnedForHighestStage(39, 40, 5));
            Assert.AreEqual(0, CombatMath.SeedsEarnedForHighestStage(44, 40, 5));
            Assert.AreEqual(1, CombatMath.SeedsEarnedForHighestStage(45, 40, 5));
            Assert.AreEqual(5, CombatMath.SeedsEarnedForHighestStage(66, 40, 5));
            Assert.AreEqual(0, CombatMath.SeedsEarnedForHighestStage(66, 40, 0)); // degenerate step
        }

        [Test]
        public void SeedDpsMultiplier_AdditiveVsCompounding()
        {
            // Zero seeds is exactly 1 either way.
            Assert.AreEqual(1.0, CombatMath.SeedDpsMultiplier(0, 0.15f, false), Tolerance);
            Assert.AreEqual(1.0, CombatMath.SeedDpsMultiplier(0, 0.15f, true), Tolerance);

            // 9 seeds (the sim's run-1 payout): additive 2.35x vs compounding ~3.52x. The gap is
            // the whole additive-vs-compounding debate in one number.
            Assert.AreEqual(2.35, CombatMath.SeedDpsMultiplier(9, 0.15f, false), 1e-6);
            Assert.AreEqual(3.5178, CombatMath.SeedDpsMultiplier(9, 0.15f, true), 1e-3);
        }
    }
}
