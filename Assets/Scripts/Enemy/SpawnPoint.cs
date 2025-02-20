using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// Enemylerin üst üste spawn olmasını engellemeyi saplayan editörde dolu veya boş olma durumunu gösteren  spawn point
    /// İleride kaldırılıp başka bir çözüme gidilebilir.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        public bool isOccupied = false;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = isOccupied ? Color.red : Color.green;
            Gizmos.DrawSphere(transform.position, 0.3f);
        }
#endif
    }
}