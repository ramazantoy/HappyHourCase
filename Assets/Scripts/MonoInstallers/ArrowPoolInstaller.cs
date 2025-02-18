
using Interfaces;
using Pool;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
    /// <summary>
    /// Dependency injection olarak arrow pool interface örneğinin injectlenmesi
    /// </summary>
    public class ArrowPoolInstaller : MonoInstaller
    {
        [SerializeField]
        private ArrowPool _instance;
        
        public override void InstallBindings()
        {
            Container
                .Bind<IArrowPool>()
                .FromInstance(_instance)
                .AsSingle();
        }
    }
}
