using Enemy;
using Interfaces;
using Pool;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField] private  EnemyManager _instance; 
        [SerializeField] private EnemyYBot enemyPrefab;
        [SerializeField] private Transform _enemyPoolTransform;
        public override void InstallBindings()
        {
            Container.Bind<IEnemyManager>()
                .FromInstance(_instance)
                .AsSingle();
            
            Container.BindMemoryPool<EnemyBase,ObjectPool<EnemyBase>>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(enemyPrefab)
                .UnderTransform(_enemyPoolTransform);
        }
    }
}
