using System;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Enemy
{
    public class EnemyBase : MonoBehaviour
    {   
        
        [Inject]
        private IEnemyManager _enemyManager;
        

        public float health = 100f;

    

        public virtual void TakeDamage(float damage)
        {
            health -= damage;

            if (!(health <= 0f)) return;


            _enemyManager.UnregisterEnemy(this);
            Destroy(gameObject);
        }

        protected  void OnEnable()
        {
            _enemyManager.RegisterEnemy(this);
        }

        protected void OnDisable()
        {
            _enemyManager.UnregisterEnemy(this);
        }
    }
}