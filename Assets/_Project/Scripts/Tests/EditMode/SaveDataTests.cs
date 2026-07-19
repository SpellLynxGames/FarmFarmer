using NUnit.Framework;
using UnityEngine;
using FarmFarmer.Core;
using FarmFarmer.Data;

namespace FarmFarmer.Tests
{
    public class SaveDataTests
    {
        [Test]
        public void SaveData_RoundTripsThroughJsonUtility()
        {
            var save = new SaveData
            {
                savedAtUtc = "2026-07-19T00:00:00.0000000Z",
                sharedWallet = new BigDouble(7.5d, 12L), // exactly representable -> equality-safe
                prestigeCurrency = new BigDouble(9d),
                gems = new BigDouble(3d),
                focusedHeroId = "sprout_knight",
            };
            save.heroes.Add(new HeroSaveState
            {
                heroId = "sprout_knight",
                isUnlocked = true,
                currentStage = 24,
                currentFloor = 7,
                highestStageReached = 25,
                level = 130,
                xp = 42.5,
            });

            var back = JsonUtility.FromJson<SaveData>(JsonUtility.ToJson(save));

            Assert.AreEqual(save.sharedWallet, back.sharedWallet);
            Assert.AreEqual(save.prestigeCurrency, back.prestigeCurrency);
            Assert.AreEqual(save.gems, back.gems);
            Assert.AreEqual("sprout_knight", back.focusedHeroId);
            Assert.AreEqual(1, back.heroes.Count);
            Assert.AreEqual(24, back.heroes[0].currentStage);
            Assert.AreEqual(25, back.heroes[0].highestStageReached);
            Assert.AreEqual(130, back.heroes[0].level);
        }

        [Test]
        public void SchemaV1Json_LoadsWithV2FieldsDefaulted()
        {
            // A literal v1 save body -- no gems, no highestStageReached. Loading must not throw
            // and must leave the new fields at their defaults (the additive-migration contract
            // SaveService.CurrentSchemaVersion's comment promises).
            const string v1Json = "{\"schemaVersion\":1,\"savedAtUtc\":\"2026-07-18T00:00:00Z\"," +
                "\"sharedWallet\":{\"Mantissa\":1.5,\"Exponent\":3}," +
                "\"prestigeCurrency\":{\"Mantissa\":0.0,\"Exponent\":0}," +
                "\"focusedHeroId\":\"sprout_knight\"," +
                "\"heroes\":[{\"heroId\":\"sprout_knight\",\"isUnlocked\":true,\"currentStage\":30," +
                "\"currentFloor\":4,\"level\":12,\"xp\":0.0,\"lastFocusedUtc\":\"\"}]," +
                "\"generators\":[]}";

            var save = JsonUtility.FromJson<SaveData>(v1Json);

            Assert.AreEqual(new BigDouble(1.5d, 3L), save.sharedWallet);
            Assert.AreEqual(BigDouble.Zero, save.gems);
            Assert.AreEqual(0, save.heroes[0].highestStageReached);
        }

        [Test]
        public void Hydrate_BackfillsHighestStageFromCurrentStage()
        {
            // The v1 -> v2 backfill lives in RosterService.Hydrate: a hero mid-run at stage 30
            // with highestStageReached 0 must come out claiming 30, or the first prestige after
            // updating would pay zero seeds for the whole pre-update run.
            var save = new SaveData();
            save.heroes.Add(new HeroSaveState
            {
                heroId = "sprout_knight",
                isUnlocked = true,
                currentStage = 30,
                currentFloor = 4,
                highestStageReached = 0,
                level = 12,
            });

            RosterService.Instance.Hydrate(save);

            Assert.AreEqual(30, save.heroes[0].highestStageReached);
            Assert.AreEqual("sprout_knight", save.focusedHeroId);
        }
    }
}
