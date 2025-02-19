using System;
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

        [SerializeField] private float _burnDamagePerSecond = 3f;
        [SerializeField] private float _burnDamageTime = 3f;


        private bool _isRageMode = false;


        private void OnEnable()
        {
            _isRageMode = false;
        }

        public void OnRageMode()
        {
            _isRageMode = true;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<IEnemy>();

            if (enemy == null) return;

            // enemy.TakeDamage(BaseDamage);


            var damage = _isRageMode ? _burnDamagePerSecond * 2 : _burnDamagePerSecond;
            enemy.ApplyBurn(damage, _burnDamageTime);
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