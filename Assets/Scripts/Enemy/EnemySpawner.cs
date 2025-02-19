using System;
using System.Collections.Generic;
using Pool;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Enemy
{
    
    /// <summary>
    /// Oyun başında belli bir sayıda enemy spawn etmek ve istenildiğinde enemy spawn etmek için kullanılıyor.
    /// </summary>
    public class EnemySpawner : MonoBehaviour {
        
        [Inject] private ObjectPool<EnemyBase> _enemyPool;
        [SerializeField] private List<Transform> spawnPoints;
        private void Start()
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
        }
    }
}