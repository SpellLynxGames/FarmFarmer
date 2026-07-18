using System.Collections.Generic;
using UnityEngine;

namespace FarmFarmer.Data
{
    // Roster size is data, not code (CLAUDE.md architecture intents) -- LiveOps adds heroes by
    // editing this list, no code changes required.
    [CreateAssetMenu(menuName = "Farm Farmer/Roster Definition", fileName = "SO_RosterDefinition")]
    public class RosterDefinition : ScriptableObject
    {
        public List<HeroDefinition> heroes = new List<HeroDefinition>();
    }
}
