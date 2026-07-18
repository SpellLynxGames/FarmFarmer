using UnityEngine;
using FarmFarmer.Core;

namespace FarmFarmer.Data
{
    [CreateAssetMenu(menuName = "Farm Farmer/Upgrade Definition", fileName = "SO_UpgradeDefinition")]
    public class UpgradeDefinition : ScriptableObject
    {
        public string upgradeId;
        public string displayName;
        public BigDouble baseCost;
        public float costGrowthRate = 1.07f;
    }
}
