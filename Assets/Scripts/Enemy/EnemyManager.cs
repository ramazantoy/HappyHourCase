using System.Collections.Generic;
using Interfaces;
using Pool;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour, IEnemyManager 
    {   
        private ObjectPool<EnemyBase> _yBotPool;
        private List<EnemyBase> _enemies = new List<EnemyBase>();

        [SerializeField] private EnemySpawner _enemySpawner;

        public static bool IsQuitting { get; private set; }

        private void OnApplicationQuit()
        {
            IsQuitting = true;
        }
        
        public void RegisterEnemy(EnemyBase enemyBase) 
        {
            if (!_enemies.Contains(enemyBase))
                _enemies.Add(enemyBase);
        }

        public void UnregisterEnemy(EnemyBase enemyBase) 
        {
            if (_enemies.Contains(enemyBase)) 
            {
                _enemies.Remove(enemyBase);
                
                if (_yBotPool != null) 
                {
                    _yBotPool.Despawn(enemyBase);
                }
                
                if (_enemies.Count >= 10) return;
                
                var rate = 10 - _enemies.Count;
                for (var i = 0; i < rate; i++)
                {
                    _enemySpawner.SpawnEnemy(); 
                }
            }
        }

        public EnemyBase GetNearestEnemy(Vector3 position) 
        {
            EnemyBase nearest = null;
            var minDist = Mathf.Infinity;
            foreach (var enemy in _enemies) 
            {
                if (enemy == null) continue;
                var dist = Vector3.Distance(position, enemy.transform.position);
                if (!(dist < minDist)) continue;
                minDist = dist;
                nearest = enemy;
            }
            return nearest;
        }
        
        public List<EnemyBase> GetNearestEnemies(Vector3 position, int count) 
        {
            List<EnemyBase> sortedEnemies = new List<EnemyBase>(_enemies);
            sortedEnemies.Sort((a, b) => Vector3.Distance(a.transform.position, position)
                .CompareTo(Vector3.Distance(b.transform.position, position)));
            return sortedEnemies.GetRange(0, Mathf.Min(count, sortedEnemies.Count));
        }
    }
}
