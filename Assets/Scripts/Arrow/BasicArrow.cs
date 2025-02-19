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

            enemy.TakeDamage(BaseDamage);
            ReturnToPool();
        }

        protected override void ReturnToPool()
        {
            base.ReturnToPool();
            if (_pool != null)
            {
                _pool.Despawn(this);
            }
        }
    }
}