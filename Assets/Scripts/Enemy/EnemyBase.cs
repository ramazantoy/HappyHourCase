using Interfaces;
using UnityEngine;
using Zenject;

namespace Enemy
{
    
    /// <summary>
    /// Enemy'in base sınıfı ilerde başka enemyle eklenirse bu sınıftan inherit olup özellik olarak ayrıştırılabilirler
    /// </summary>
    public class EnemyBase : MonoBehaviour,IEnemy
    {
         IEnemyManager _enemyManager;

       
       [Inject]
        public void Constructor(IEnemyManager enemyManager)
        {
            _enemyManager = enemyManager;
        }
        
        
        private float _health = 100f;

        public virtual void TakeDamage(float damage)
        {
            _health -= damage;
            if (!(_health <= 0f)) return;
            
            _enemyManager.UnregisterEnemy(this);
            gameObject.SetActive(false);
        }

        public Vector3 Position => transform.position;

        protected void OnEnable()
        {
            _health = 100f;
            _enemyManager.RegisterEnemy(this);
        }

        protected void OnDisable()
        {
            if (EnemyManager.IsQuitting) //Zenject bir hata veriyordu onu fixlemek için flag ekledim.
                return;
            
            if (_enemyManager != null)
            {
                _enemyManager.UnregisterEnemy(this);
            }
        }
    }
}