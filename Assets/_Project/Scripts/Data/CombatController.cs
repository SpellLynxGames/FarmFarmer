using System;
using UnityEngine;
using FarmFarmer.Core;

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

        private SaveData _save;
        private BigDouble _currentEnemyHp;
        private BigDouble _currentEnemyMaxHp;

        private void Awake()
        {
            _save = SaveService.Load();
            RosterService.Instance.Hydrate(_save);
            SpawnEnemyForCurrentProgress();
        }

        private void Update()
        {
            ApplyDamage(FocusedDps() * Time.deltaTime);
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
            return balance.HeroDps(def, state.level);
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

        private void AdvanceFloorOrStage(HeroSaveState state)
        {
            var floorsRequired = balance.FloorsRequiredForStage(state.currentStage);
            state.currentFloor++;
            if (state.currentFloor > floorsRequired)
            {
                state.currentFloor = 1;
                state.currentStage++;
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

            EnemySpawned?.Invoke(def);
            EnemyHpChanged?.Invoke(_currentEnemyHp, _currentEnemyMaxHp);
            ProgressChanged?.Invoke(stage, state.currentFloor, balance.FloorsRequiredForStage(stage));
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) SaveService.Save(_save);
        }

        private void OnApplicationQuit()
        {
            SaveService.Save(_save);
        }
    }
}
