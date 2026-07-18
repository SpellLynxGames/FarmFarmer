using UnityEngine;
using UnityEngine.UI;
using FarmFarmer.Data;

namespace FarmFarmer.UI
{
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private CombatController combatController;
        [SerializeField] private Image enemySprite;

        private void OnEnable()
        {
            combatController.EnemySpawned += HandleEnemySpawned;
        }

        private void OnDisable()
        {
            combatController.EnemySpawned -= HandleEnemySpawned;
        }

        private void HandleEnemySpawned(EnemyDefinition enemy)
        {
            enemySprite.sprite = enemy != null ? enemy.sprite : null;
        }
    }
}
