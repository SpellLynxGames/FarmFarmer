using System;

namespace FarmFarmer.Core
{
    // Shared across the roster -- allocation across heroes is the strategy layer (CLAUDE.md #6).
    // UI subscribes to BalanceChanged rather than polling.
    public class WalletService
    {
        public static WalletService Instance { get; } = new WalletService();

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
