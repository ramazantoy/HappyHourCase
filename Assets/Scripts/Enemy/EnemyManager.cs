using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using Zenject;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour, IEnemyManager {
        
        
        private List<EnemyBase> _enemies = new List<EnemyBase>();

        [SerializeField] private EnemySpawner _enemySpawner;
        
        public void RegisterEnemy(EnemyBase enemyBase) {
            if (!_enemies.Contains(enemyBase))
                _enemies.Add(enemyBase);
        }

        public void UnregisterEnemy(EnemyBase enemyBase) {
            if (_enemies.Contains(enemyBase)) {
                _enemies.Remove(enemyBase);
                // Sürekli en az 5 enemy olması için:
                while(_enemies.Count < 5) {
                    _enemySpawner.SpawnEnemy();
                }
            }
        }

        public EnemyBase GetNearestEnemy(Vector3 position) {
            EnemyBase nearest = null;
            float minDist = Mathf.Infinity;
            foreach (var enemy in _enemies) {
                if (enemy == null) continue;
                float dist = Vector3.Distance(position, enemy.transform.position);
                if (dist < minDist) {
                    minDist = dist;
                    nearest = enemy;
                }
            }
            return nearest;
        }

        public List<EnemyBase> GetNearestEnemies(Vector3 position, int count) {
            List<EnemyBase> sortedEnemies = new List<EnemyBase>(_enemies);
            sortedEnemies.Sort((a, b) => Vector3.Distance(a.transform.position, position)
                .CompareTo(Vector3.Distance(b.transform.position, position)));
            return sortedEnemies.GetRange(0, Mathf.Min(count, sortedEnemies.Count));
        }
    }
}