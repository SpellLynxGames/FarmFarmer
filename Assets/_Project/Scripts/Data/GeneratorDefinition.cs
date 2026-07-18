using UnityEngine;

namespace FarmFarmer.Data
{
    [System.Serializable]
    public class ResourceTier
    {
        public string tierId;
        public int unlockLevel;
    }

    // Woodworker/Miner/Gatherer-style skill NPC (CLAUDE.md #7). Gameplay impact per skill is
    // still TBD -- this is the shape, not the balance.
    [CreateAssetMenu(menuName = "Farm Farmer/Generator Definition", fileName = "SO_GeneratorDefinition")]
    public class GeneratorDefinition : ScriptableObject
    {
        public string generatorId;
        public string displayName;
        public Sprite icon;

        // Expected 7 entries at unlock levels 1/15/32/50/65/80/91 per the locked skill spec.
        public ResourceTier[] resourceTiers;
    }
}
