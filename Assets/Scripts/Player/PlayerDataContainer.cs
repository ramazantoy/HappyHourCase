using UnityEngine;

namespace Player
{
    
    /// <summary>
    /// Oyuncu ile ilgili ayarların yer aldığı SO.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerDataContainer", menuName = "ScriptableObjects/PlayerDataContainer",order = 1)]
    public class PlayerDataContainer : ScriptableObject
    { 
        public float MovementSpeed = 5f;
        public float RotationSpeed = 10f; 
        public float BaseAttackSpeed = 3f;
        public VectorClamps MovementClamps;
        public bool ShootSfx = true;
        public AudioClip ShootClip;
    }

    [System.Serializable]
    public struct VectorClamps
    {
        public float MinX;//-10
        public float MaxX;//10
        public float MinZ;
        public float MaxZ;
    }
}