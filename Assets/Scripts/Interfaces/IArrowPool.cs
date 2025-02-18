using Arrow;

namespace Interfaces
{
    public interface IArrowPool {
        ArrowProjectile GetArrow();
        void ReturnArrow(ArrowProjectile arrow);
    }
}