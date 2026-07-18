using UnityEngine;

namespace FarmFarmer.Data
{
    // A stage contains 10 floors (the kill tally); reaching floor 10/10 clears the stage.
    // Floors are generally inconsequential on their own -- stage-to-stage transitions and the
    // miniboss/boss cadence are where difficulty escalates. Backdrops change per stage, not per
    // floor. See CLAUDE.md decision 4.
    [CreateAssetMenu(menuName = "Farm Farmer/Stage Definition", fileName = "SO_StageDefinition")]
    public class StageDefinition : ScriptableObject
    {
        public const int FloorsPerStage = 10;

        public int stageNumber;
        public bool IsMinibossStage => stageNumber % 10 == 0;
        public bool IsBossStage => stageNumber % 25 == 0;
        public EnemyDefinition bossOverride;
        public string backdropRegionId;
    }
}
