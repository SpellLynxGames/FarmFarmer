using UnityEngine;
using FarmFarmer.Core;

namespace FarmFarmer.Data
{
    [CreateAssetMenu(menuName = "Farm Farmer/Enemy Definition", fileName = "SO_EnemyDefinition")]
    public class EnemyDefinition : ScriptableObject
    {
        public string enemyId;
        public string displayName;
        public Sprite sprite;
        public BigDouble baseHp;
    }
}
