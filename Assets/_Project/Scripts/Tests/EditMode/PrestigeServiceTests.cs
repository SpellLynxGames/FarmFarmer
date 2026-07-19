using NUnit.Framework;
using UnityEngine;
using FarmFarmer.Core;
using FarmFarmer.Data;

namespace FarmFarmer.Tests
{
    // Decision 7 in executable form: the reset clears the battlefield, never the estate.
    public class PrestigeServiceTests
    {
        private CombatBalanceDefinition _balance;

        [SetUp]
        public void SetUp()
        {
            // Asset defaults ARE the placeholder tuning -- testing against them keeps these
            // numbers honest with what a fresh SO_CombatBalanceDefinition would ship.
            _balance = ScriptableObject.CreateInstance<CombatBalanceDefinition>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_balance);
            WalletService.Instance.SetBalance(BigDouble.Zero);
        }

        private static SaveData MakeTwoHeroSave()
        {
            var save = new SaveData
            {
                sharedWallet = new BigDouble(1d, 9L),
                gems = new BigDouble(4d),
            };
            save.heroes.Add(new HeroSaveState
            {
                heroId = "sprout_knight", isUnlocked = true,
                currentStage = 60, currentFloor = 3, highestStageReached = 66, level = 361, xp = 10,
            });
            save.heroes.Add(new HeroSaveState
            {
                heroId = "hero_2", isUnlocked = true,
                currentStage = 55, currentFloor = 8, highestStageReached = 60, level = 294, xp = 5,
            });
            save.generators.Add(new GeneratorSaveState { generatorId = "woodworker", level = 40, xp = 12 });
            return save;
        }

        [Test]
        public void PendingSeeds_SumsAcrossRoster()
        {
            // Mirrors the v0.3 sim's run-1 endstate: stages 66 + 60 -> 5 + 4 = 9 seeds.
            var save = MakeTwoHeroSave();
            Assert.AreEqual(9, PrestigeService.Instance.PendingSeeds(save, _balance));
        }

        [Test]
        public void Harvest_ClearsTheBattlefield()
        {
            var save = MakeTwoHeroSave();
            var earned = PrestigeService.Instance.Harvest(save, _balance);

            Assert.AreEqual(9, earned);
            Assert.AreEqual(new BigDouble(9d), save.prestigeCurrency);
            Assert.AreEqual(BigDouble.Zero, save.sharedWallet);
            Assert.AreEqual(BigDouble.Zero, WalletService.Instance.Balance);

            foreach (var hero in save.heroes)
            {
                Assert.AreEqual(1, hero.currentStage);
                Assert.AreEqual(1, hero.currentFloor);
                Assert.AreEqual(1, hero.highestStageReached);
                Assert.AreEqual(1, hero.level); // both unlocked -> back to the bootstrap level
                Assert.AreEqual(0, hero.xp);
                Assert.IsTrue(hero.isUnlocked); // roster characters, not Clicker Heroes re-buys
            }
        }

        [Test]
        public void Harvest_NeverTouchesTheEstate()
        {
            var save = MakeTwoHeroSave();
            PrestigeService.Instance.Harvest(save, _balance);

            // Gems (Open Q #8 assumption) and generator/skill state must survive The Harvest.
            Assert.AreEqual(new BigDouble(4d), save.gems);
            Assert.AreEqual(1, save.generators.Count);
            Assert.AreEqual(40, save.generators[0].level);
            Assert.AreEqual(12, save.generators[0].xp);
        }

        [Test]
        public void Harvest_TwiceInARow_PaysNothingTheSecondTime()
        {
            var save = MakeTwoHeroSave();
            PrestigeService.Instance.Harvest(save, _balance);
            var secondEarned = PrestigeService.Instance.Harvest(save, _balance);

            Assert.AreEqual(0, secondEarned);
            Assert.AreEqual(new BigDouble(9d), save.prestigeCurrency);
        }

        [Test]
        public void Harvest_LockedHeroResetsToLevelZero()
        {
            var save = MakeTwoHeroSave();
            save.heroes[1].isUnlocked = false;

            PrestigeService.Instance.Harvest(save, _balance);

            Assert.AreEqual(0, save.heroes[1].level);
            Assert.IsFalse(save.heroes[1].isUnlocked);
        }
    }
}
