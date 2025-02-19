
using Arrow;
using Interfaces;
using Pool;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace MonoInstallers
{
    /// <summary>
    /// Dependency injection olarak arrowların poolarının injectlenmesi
    /// </summary>
    public class ArrowPoolInstaller : MonoInstaller
    {
        [SerializeField] private BasicArrow basicArrowPrefab;
        [SerializeField] private BounceArrow bounceArrowPrefab;
        [SerializeField] private BurnArrow burnArrowPrefab; 
        [SerializeField] private Transform basicPoolParent;
        [SerializeField] private Transform bouncePoolParent;
        [SerializeField] private Transform burnPoolParent;
        public override void InstallBindings()
        {
            Container.BindMemoryPool<BasicArrow, ObjectPool<BasicArrow>>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(basicArrowPrefab)
                .UnderTransform(basicPoolParent);

            Container.BindMemoryPool<BounceArrow,ObjectPool<BounceArrow>>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(bounceArrowPrefab)
                .UnderTransform(bouncePoolParent);

            Container.BindMemoryPool<BurnArrow,ObjectPool<BurnArrow>>()
                .WithInitialSize(20)
                .FromComponentInNewPrefab(burnArrowPrefab)
                .UnderTransform(burnPoolParent);
        }
    }
}
