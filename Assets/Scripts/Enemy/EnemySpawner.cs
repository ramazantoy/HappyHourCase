using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Enemy
{
    public class EnemySpawner : MonoBehaviour {
        
        [Inject] private EnemyMemoryPool _enemyPool;
        [SerializeField] private List<Transform> spawnPoints;

        private void Awake()
        {
            for (var i = 0; i < 10; i++)
            {
                SpawnEnemy();
            }
        }

        public void SpawnEnemy() {
            
            var index = Random.Range(0, spawnPoints.Count);
            var spawnPoint = spawnPoints[index];
            
            var enemy = _enemyPool.Spawn();
            
            if(enemy==null || spawnPoint==null) return;
            
            enemy.transform.position = spawnPoint.position;
            //enemy.transform.rotation = spawnPoint.rotation;
        }
    }
}