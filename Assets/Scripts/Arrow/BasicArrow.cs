using Interfaces;
using UnityEngine;
using Pool;
using Zenject;

namespace Arrow
{
    /// <summary>
    /// Basit Arrow script'i base'e bağlı özellikleri kullanıyor.
    /// </summary>
    public class BasicArrow : ArrowBase
    {
        [Inject] private ObjectPool<BasicArrow> _pool;

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