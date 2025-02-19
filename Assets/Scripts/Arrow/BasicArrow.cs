using UnityEngine;

using Pool;
using Zenject;

namespace Arrow
{
    public class BasicArrow : ArrowBase
    {
        [Inject] private ObjectPool<BasicArrow> _pool;

        [SerializeField] private ParticleSystem hitEffect;

        // Çarpışma anında OnTriggerEnter üzerinden hasar veriliyor.
        protected override void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<Interfaces.IEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                if (hitEffect != null)
                {
                    hitEffect.transform.position = transform.position;
                    hitEffect.Play();
                }
            }

            ReturnToPool();
        }

        protected override void ReturnToPool()
        {
            base.ReturnToPool();
            _pool.Despawn(this);
        }
    }
}