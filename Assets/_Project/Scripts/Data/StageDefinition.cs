using UnityEngine;

namespace FarmFarmer.Data
{
    // "Stage" naming is provisional -- CLAUDE.md Open Questions #5 hasn't reconciled
    // floor vs. stage vs. lane terminology yet. Rename this if/when that lands.
    [CreateAssetMenu(menuName = "Farm Farmer/Stage Definition", fileName = "SO_StageDefinition")]
    public class StageDefinition : ScriptableObject
    {
        public int stageNumber;
        public bool IsBossStage => stageNumber % 5 == 0;
        public EnemyDefinition bossOverride;
        public string backdropRegionId;
    }
}
