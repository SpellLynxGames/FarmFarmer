using System;

namespace FarmFarmer.Core
{
    // Locked formula, CLAUDE.md decision 7 -- run-2 sim validated this curve, don't retune casually.
    public static class SkillProgression
    {
        public const int MaxLevel = 99;

        public static readonly int[] TierUnlockLevels = { 1, 15, 32, 50, 65, 80, 91 };

        public static double XpRequiredForLevel(int level)
        {
            return 40d * Math.Pow(1.12d, level);
        }
    }
}
