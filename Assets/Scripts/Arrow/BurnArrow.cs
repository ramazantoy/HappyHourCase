using UnityEngine;
using Interfaces;
using Zenject;
using Pool;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Arrow
{
    public class BurnArrow : ArrowBase
    {
        [Inject] private ObjectPool<BurnArrow> _pool;
        
        private float _burnDamagePerSecond=3f;
        private float _burnDamageTime=3f;
        
        
        

        protected override void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<IEnemy>();
            
            if (enemy == null) return;
            
            enemy.TakeDamage(damage);
            enemy.ApplyBurn(_burnDamagePerSecond,_burnDamageTime);
            ReturnToPool();
        }
        protected override void ReturnToPool()
        {
    
            
            base.ReturnToPool();
            _pool.Despawn(this);
        }

    
    }
}