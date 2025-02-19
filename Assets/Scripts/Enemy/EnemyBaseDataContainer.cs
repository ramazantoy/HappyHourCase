using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class EnemyBaseDataContainer : ScriptableObject
    { 
        public Color FullHealthColor = Color.green; 
        public Color MidHealthColor = Color.yellow; 
        public Color LowHealthColor = Color.red; 
    }
}