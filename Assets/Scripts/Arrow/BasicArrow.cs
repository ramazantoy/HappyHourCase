using Interfaces;
using UnityEngine;

using Pool;
using Zenject;

namespace Arrow
{
    public class BasicArrow : ArrowBase
    {
        [Inject] private ObjectPool<BasicArrow> _pool;
        

        // Çarpışma anında OnTriggerEnter üzerinden hasar veriliyor.
        protected override void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IEnemy enemy)) return;
            
            enemy.TakeDamage(damage);
            ReturnToPool();


        }

        protected override void ReturnToPool()
        {
            base.ReturnToPool();
            _pool.Despawn(this);
        }
    }
}