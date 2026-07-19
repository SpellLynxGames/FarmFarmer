using NUnit.Framework;
using FarmFarmer.Core;

namespace FarmFarmer.Tests
{
    // The curve here is LOCKED (Decision 7, run-2 sim validated) -- if one of these fails,
    // someone retuned a locked constant and should know exactly what they did.
    public class SkillProgressionTests
    {
        [Test]
        public void XpCurve_Is40Times1Point12PerLevel()
        {
            Assert.AreEqual(44.8d, SkillProgression.XpRequiredForLevel(1), 1e-9);
            Assert.AreEqual(40d * System.Math.Pow(1.12d, 50), SkillProgression.XpRequiredForLevel(50), 1e-6);
        }

        [Test]
        public void SevenTiers_AtTheLockedUnlockLevels()
        {
            CollectionAssert.AreEqual(new[] { 1, 15, 32, 50, 65, 80, 91 }, SkillProgression.TierUnlockLevels);
        }

        [Test]
        public void LevelCap_Is99()
        {
            Assert.AreEqual(99, SkillProgression.MaxLevel);
        }
    }
}
