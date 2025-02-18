using Enemy;
using Interfaces;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
    public class EnemyInstaller : MonoInstaller
    {
        [SerializeField] private  EnemyManager _instance; 
        [SerializeField] private EnemyYBot enemyPrefab; 
        public override void InstallBindings()
        {
            Container.Bind<IEnemyManager>()
                .FromInstance(_instance)
                .AsSingle();
            
            Container.BindMemoryPool<EnemyYBot, EnemyMemoryPool>()
                .WithInitialSize(10)
                .FromComponentInNewPrefab(enemyPrefab)
                .UnderTransformGroup("EnemyPool");
        }
    }
}
