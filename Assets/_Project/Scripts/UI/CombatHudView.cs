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

        private void OnEnable()
        {
            combatController.EnemyHpChanged += HandleEnemyHpChanged;
            combatController.ProgressChanged += HandleProgressChanged;
            WalletService.Instance.BalanceChanged += HandleBalanceChanged;

            HandleBalanceChanged(WalletService.Instance.Balance);
        }

        private void OnDisable()
        {
            combatController.EnemyHpChanged -= HandleEnemyHpChanged;
            combatController.ProgressChanged -= HandleProgressChanged;
            WalletService.Instance.BalanceChanged -= HandleBalanceChanged;
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
    }
}
