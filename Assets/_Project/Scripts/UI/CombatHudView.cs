using UnityEngine;
using TMPro;
using FarmFarmer.Core;
using FarmFarmer.Core.Combat;
using FarmFarmer.Data;

namespace FarmFarmer.UI
{
    // Subscribes only -- never polls (CLAUDE.md convention).
    public class CombatHudView : MonoBehaviour
    {
        [SerializeField] private CombatController combatController;
        [SerializeField] private TMP_Text coinText;
        [SerializeField] private TMP_Text stageFloorText;
        [SerializeField] private TMP_Text enemyHpText;

        // Optional until the manual scene-wiring pass catches up -- null means "don't show it",
        // not a broken reference, so everything below null-guards these two.
        [SerializeField] private TMP_Text gemsText;
        [SerializeField] private TMP_Text encounterTimerText;

        private void OnEnable()
        {
            combatController.EnemyHpChanged += HandleEnemyHpChanged;
            combatController.ProgressChanged += HandleProgressChanged;
            combatController.EncounterTimerChanged += HandleEncounterTimerChanged;
            WalletService.Instance.BalanceChanged += HandleBalanceChanged;
            WalletService.Gems.BalanceChanged += HandleGemsChanged;

            HandleBalanceChanged(WalletService.Instance.Balance);
            HandleGemsChanged(WalletService.Gems.Balance);
        }

        private void OnDisable()
        {
            combatController.EnemyHpChanged -= HandleEnemyHpChanged;
            combatController.ProgressChanged -= HandleProgressChanged;
            combatController.EncounterTimerChanged -= HandleEncounterTimerChanged;
            WalletService.Instance.BalanceChanged -= HandleBalanceChanged;
            WalletService.Gems.BalanceChanged -= HandleGemsChanged;
        }

        private void HandleEnemyHpChanged(BigDouble current, BigDouble max)
        {
            enemyHpText.text = $"{current} / {max}";
        }

        private void HandleProgressChanged(int stage, int floor, int floorsRequired)
        {
            if (CombatMath.IsBossStage(stage)) stageFloorText.text = $"Stage {stage} (Boss)";
            else if (CombatMath.IsMinibossStage(stage)) stageFloorText.text = $"Stage {stage} (Miniboss)";
            else stageFloorText.text = $"Stage {stage} - {floor}/{floorsRequired}";
        }

        private void HandleBalanceChanged(BigDouble balance)
        {
            coinText.text = $"Coin: {balance}";
        }

        private void HandleGemsChanged(BigDouble gems)
        {
            if (gemsText == null) return;
            gemsText.text = $"Gems: {gems}";
        }

        private void HandleEncounterTimerChanged(float remaining, float total)
        {
            if (encounterTimerText == null) return;
            var timed = total > 0f;
            if (encounterTimerText.gameObject.activeSelf != timed)
            {
                encounterTimerText.gameObject.SetActive(timed);
            }
            if (timed) encounterTimerText.text = $"{remaining:0.0}s";
        }
    }
}
