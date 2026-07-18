using UnityEngine;

namespace FarmFarmer.Data
{
    [CreateAssetMenu(menuName = "Farm Farmer/Hero Definition", fileName = "SO_HeroDefinition")]
    public class HeroDefinition : ScriptableObject
    {
        public string heroId;
        public string displayName;
        public Sprite icon;

        // Balance containment rule (CLAUDE.md #6): heroes differ only by these two plus one
        // signature mechanic. Never add bespoke curves per hero.
        [Header("Master curve offset/multiplier only -- no bespoke curves")]
        public float statOffset;
        public float statMultiplier = 1f;
        public string signatureMechanicId;

        public int unlockOrder;
    }
}
