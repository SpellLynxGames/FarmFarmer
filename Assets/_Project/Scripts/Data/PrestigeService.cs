using System;
using FarmFarmer.Core;

namespace FarmFarmer.Data
{
    // "The Harvest" (final name pending -- Open Q #4 collision). Decision 7's rule verbatim:
    // the reset clears the battlefield, never the estate. Combat progression, hero levels, and
    // coin reset; skill XP, stockpiles, buildings, and (Open Q #8 assumption) gems all survive.
    // Lives in Data for the same reason as RosterService: needs CombatBalanceDefinition, and the
    // asmdef dependency only runs Data -> Core. No UI calls this yet -- scaffold + tests only.
    public class PrestigeService
    {
        public static PrestigeService Instance { get; } = new PrestigeService();

        public event Action<int> Harvested; // seeds earned by the run just ended

        // Payout: floor((highest - floor) / step) per hero, summed across the roster. The
        // summing-across-roster part is the v0.3 sim's assumption, not a confirmed decision --
        // if Derek rules otherwise, this is the only method that changes.
        public int PendingSeeds(SaveData save, CombatBalanceDefinition balance)
        {
            var total = 0;
            foreach (var hero in save.heroes)
            {
                total += balance.SeedsEarnedForHighestStage(hero.highestStageReached);
            }
            return total;
        }

        // Mutates the save in place and resets the live coin wallet; the caller owns actually
        // writing the save to disk (CombatController's SyncAndSave), same as everywhere else.
        public int Harvest(SaveData save, CombatBalanceDefinition balance)
        {
            var earned = PendingSeeds(save, balance);
            save.prestigeCurrency += earned;

            foreach (var hero in save.heroes)
            {
                // Unlock status survives on purpose: heroes are roster characters (Decision 6),
                // not Clicker Heroes-style re-buys. Level 1 mirrors the first-run bootstrap.
                hero.currentStage = 1;
                hero.currentFloor = 1;
                hero.highestStageReached = 1;
                hero.level = hero.isUnlocked ? 1 : 0;
                hero.xp = 0;
            }

            save.sharedWallet = BigDouble.Zero;
            WalletService.Instance.SetBalance(BigDouble.Zero);
            // save.gems, WalletService.Gems, and save.generators are untouched on purpose.

            Harvested?.Invoke(earned);
            return earned;
        }
    }
}
