using System;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Enemy
{
    public class EnemyBase : MonoBehaviour
    {
        [Inject] private IEnemyManager _enemyManager;


        private float _health = 100f;


        public virtual void TakeDamage(float damage)
        {
            _health -= damage;

            if (!(_health <= 0f)) return;


            _enemyManager.UnregisterEnemy(this);
            gameObject.SetActive(false);
        }

        protected void OnEnable()
        {
            _health = 100f;
            _enemyManager.RegisterEnemy(this);
        }

        protected void OnDisable()
        {
            if (_enemyManager != null)
            {
                _enemyManager.UnregisterEnemy(this);
            }
        }
    }
}