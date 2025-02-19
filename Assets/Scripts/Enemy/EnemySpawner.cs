using System;
using System.Collections.Generic;
using Pool;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Random = UnityEngine.Random;

namespace Enemy
{
    /// <summary>
    /// Oyun başında belli bir sayıda enemy spawn etmek ve istenildiğinde enemy spawn etmek için kullanılıyor.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Inject] private ObjectPool<EnemyBase> _enemyPool;
        [SerializeField] private List<SpawnPoint> _spawnPoints;

        private void Start()
        {
            for (var i = 0; i < 10; i++)
            {
                SpawnEnemy();
            }
        }

        public void SpawnEnemy()
        {
            var index = Random.Range(0, _spawnPoints.Count);
            var spawnPoint = _spawnPoints[index];
            var enemy = _enemyPool.Spawn();
            if (enemy == null || spawnPoint == null) return;

            if (spawnPoint.isOccupied)
            {
                var offset = new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-2f, 2f));
                enemy.transform.position = spawnPoint.transform.position + offset;
            }
            else
            {
                enemy.transform.position = spawnPoint.transform.position;
                enemy.SetSpawnPoint(spawnPoint);
            }
        }
    }
}