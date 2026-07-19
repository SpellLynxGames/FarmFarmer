using System;

namespace FarmFarmer.Core
{
    // Shared across the roster -- allocation across heroes is the strategy layer (CLAUDE.md #6).
    // UI subscribes to BalanceChanged rather than polling.
    public class WalletService
    {
        // Coin. FARM resources (Decision 9's third leg) will need per-tier typing, not more
        // instances of this -- don't extend this pattern to them.
        public static WalletService Instance { get; } = new WalletService();

        // Gems, Decision 9's meta currency. Same event contract as coin, separate subscribers.
        // Survives prestige (Open Q #8 -- genre-convention assumption, not Derek-confirmed).
        public static WalletService Gems { get; } = new WalletService();

        public BigDouble Balance { get; private set; }

        public event Action<BigDouble> BalanceChanged;

        public void Add(BigDouble amount)
        {
            Balance += amount;
            BalanceChanged?.Invoke(Balance);
        }

        public bool TrySpend(BigDouble amount)
        {
            if (Balance < amount) return false;
            Balance -= amount;
            BalanceChanged?.Invoke(Balance);
            return true;
        }

        public void SetBalance(BigDouble amount)
        {
            Balance = amount;
            BalanceChanged?.Invoke(Balance);
        }
    }
}
