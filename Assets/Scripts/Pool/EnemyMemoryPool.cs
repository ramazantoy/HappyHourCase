using Zenject;
using UnityEngine;

namespace Enemy
{
    public class EnemyMemoryPool : MemoryPool<EnemyYBot>
    {
        protected override void OnCreated(EnemyYBot enemy)
        {
     
        }

        protected override void OnSpawned(EnemyYBot enemy)
        {
            enemy.gameObject.SetActive(true);
        }

        protected override void OnDespawned(EnemyYBot enemy)
        {
            enemy.gameObject.SetActive(false);
        }
    }
}
