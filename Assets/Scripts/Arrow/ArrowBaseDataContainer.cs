using UnityEngine;
using UnityEngine.Serialization;

namespace Arrow
{
    [CreateAssetMenu(fileName = "ArrowBaseData", menuName = "ScriptableObjects/ArrowBaseData", order = 1)]
    public class ArrowBaseDataContainer : ScriptableObject
    {
        public float MovementSpeed = 20f;
        public float Damage = 10f;
        public float Lifetime = 5f;
        public float Gravity = 9.8f;
        public float AirResistance = 0.1f;
        public float StabilizationFactor = 5f;
        public float RotationSmoothing = 25f;
    }
}