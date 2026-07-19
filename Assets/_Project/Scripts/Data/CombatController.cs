using System;
using UnityEngine;
using FarmFarmer.Core;
using FarmFarmer.Core.Combat;

namespace FarmFarmer.Data
{
    // The actual combat loop for the focused hero. Continuous auto-DPS ticks every frame; tapping
    // adds bonus burst damage on top (confirmed w/ Derek -- see plan notes). Lives in Data because
    // it depends on RosterService, which itself has to live in Data (Core can't reference Data).
    public class CombatController : MonoBehaviour
    {
        [SerializeField] private CombatBalanceDefinition balance;
        [SerializeField] private EnemyDefinition normalEnemy;
        [SerializeField] private EnemyDefinition minibossEnemy;
        [SerializeField] private EnemyDefinition bossEnemy;

        public event Action<BigDouble, BigDouble> EnemyHpChanged; // current, max
        public event Action<EnemyDefinition> EnemySpawned;
        public event Action<int, int, int> ProgressChanged;      // stage, floor, floorsRequired
        public event Action<BigDouble> EnemyDefeated;             // coin reward
        public event Action<float, float> EncounterTimerChanged;  // remaining, total; total <= 0 = untimed
        public event Action<int> EncounterFailed;                 // stage the hero was knocked back to

        private SaveData _save;
        private BigDouble _currentEnemyHp;
        private BigDouble _currentEnemyMaxHp;

        // Wall timer (miniboss/boss only, Decision 4). 0 total = untimed normal stage.
        private float _encounterTimeLeft;
        private float _encounterTimeTotal;

        private void Awake()
        {
            _save = SaveService.Load();
            RosterService.Instance.Hydrate(_save);

            // Save file is the source of truth on boot. (This used to be missing -- sharedWallet
            // got written to disk but never read back, so every restart zeroed the coin wallet.)
            WalletService.Instance.SetBalance(_save.sharedWallet);
            WalletService.Gems.SetBalance(_save.gems);

            SpawnEnemyForCurrentProgress();
        }

        private void Update()
        {
            ApplyDamage(FocusedDps() * Time.deltaTime);
            TickEncounterTimer(Time.deltaTime);
        }

        public void OnEnemyTapped()
        {
            ApplyDamage(balance.TapDamage(FocusedDps()));
        }

        private BigDouble FocusedDps()
        {
            var state = RosterService.Instance.FocusedHeroState;
            var def = RosterService.Instance.FocusedHeroDefinition;
            if (state == null || def == null) return BigDouble.Zero;

            // Seed bonus is a global DPS multiplier (Decision 7); additive vs compounding is the
            // balance asset's toggle, not this class's business.
            var seedMult = balance.SeedDpsMultiplier(_save.prestigeCurrency.ToDouble());
            return balance.HeroDps(def, state.level) * seedMult;
        }

        private void ApplyDamage(BigDouble amount)
        {
            _currentEnemyHp -= amount;
            EnemyHpChanged?.Invoke(_currentEnemyHp, _currentEnemyMaxHp);

            if (_currentEnemyHp <= BigDouble.Zero)
            {
                HandleDefeated();
            }
        }

        private void HandleDefeated()
        {
            var state = RosterService.Instance.FocusedHeroState;
            var reward = balance.CoinRewardForStage(state.currentStage);
            WalletService.Instance.Add(reward);
            EnemyDefeated?.Invoke(reward);

            AdvanceFloorOrStage(state);
            SpawnEnemyForCurrentProgress();
        }

        private void TickEncounterTimer(float dt)
        {
            if (_encounterTimeTotal <= 0f) return;

            _encounterTimeLeft -= dt;
            if (_encounterTimeLeft > 0f)
            {
                EncounterTimerChanged?.Invoke(_encounterTimeLeft, _encounterTimeTotal);
                return;
            }

            // Decision 4: failing a miniboss/boss knocks the player back one stage. Floor restarts
            // at 1 -- the previous stage's partial progress isn't tracked anywhere to restore.
            var state = RosterService.Instance.FocusedHeroState;
            state.currentStage = CombatMath.KnockbackStage(state.currentStage);
            state.currentFloor = 1;
            EncounterFailed?.Invoke(state.currentStage);
            SpawnEnemyForCurrentProgress();
        }

        private void AdvanceFloorOrStage(HeroSaveState state)
        {
            var floorsRequired = balance.FloorsRequiredForStage(state.currentStage);
            state.currentFloor++;
            if (state.currentFloor > floorsRequired)
            {
                var clearedStage = state.currentStage;
                state.currentFloor = 1;
                state.currentStage++;
                if (state.currentStage > state.highestStageReached)
                {
                    state.highestStageReached = state.currentStage;
                }

                // Decision 9: gems drop from combat every N stages (100). Clear-triggered, so a
                // knockback-and-reclear of the same gem stage would double-drop -- irrelevant
                // today (gem stages are boss/miniboss stages you can't re-fail past), revisit if
                // the cadence ever lands on a normal stage.
                if (balance.IsGemStage(clearedStage))
                {
                    WalletService.Gems.Add(balance.gemsPerDrop);
                }
            }

            var newFloorsRequired = balance.FloorsRequiredForStage(state.currentStage);
            ProgressChanged?.Invoke(state.currentStage, state.currentFloor, newFloorsRequired);
        }

        private void SpawnEnemyForCurrentProgress()
        {
            var state = RosterService.Instance.FocusedHeroState;
            if (state == null) return;

            var stage = state.currentStage;
            var isBoss = balance.IsBossStage(stage);
            var isMiniboss = balance.IsMinibossStage(stage);
            var def = isBoss ? bossEnemy : isMiniboss ? minibossEnemy : normalEnemy;

            _currentEnemyMaxHp = balance.EnemyHpForStage(stage);
            _currentEnemyHp = _currentEnemyMaxHp;

            _encounterTimeTotal = balance.EncounterTimerForStage(stage);
            _encounterTimeLeft = _encounterTimeTotal;

            EnemySpawned?.Invoke(def);
            EnemyHpChanged?.Invoke(_currentEnemyHp, _currentEnemyMaxHp);
            ProgressChanged?.Invoke(stage, state.currentFloor, balance.FloorsRequiredForStage(stage));
            // Fired for untimed stages too so the HUD knows to hide the timer.
            EncounterTimerChanged?.Invoke(_encounterTimeLeft, _encounterTimeTotal);
        }

        // Runtime services hold the live balances; the save object only learns them at write time.
        private void SyncAndSave()
        {
            _save.sharedWallet = WalletService.Instance.Balance;
            _save.gems = WalletService.Gems.Balance;
            SaveService.Save(_save);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SyncAndSave();
        }

        private void OnApplicationQuit()
        {
            SyncAndSave();
        }
    }
}
