using UnityEngine;
using Zenject;

namespace MonoInstallers
{
    public class CanvasInstaller : MonoInstaller
    {
        [SerializeField]
        private CanvasController _instance;
        
        public override void InstallBindings()
        {
            Container
                .Bind<CanvasController>()
                .FromInstance(_instance)
                .AsSingle().NonLazy();
        }
    }
}
