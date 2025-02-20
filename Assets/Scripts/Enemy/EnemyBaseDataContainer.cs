using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Enemy ile ilgili bazı şeylerin depolanabileceği enemy için SO script'i
    /// </summary>
    [CreateAssetMenu(fileName = "EnemyBaseDataContainer", menuName = "ScriptableObjects/EnemyBaseDataContainer", order = 1)]
    public class EnemyBaseDataContainer : ScriptableObject
    { 
        public Color FullHealthColor = Color.green; 
        public Color MidHealthColor = Color.yellow; 
        public Color LowHealthColor = Color.red; 
    }
}