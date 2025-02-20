namespace Interfaces
{
    /// <summary>
    /// Aniamtor Handler'ın interface'i gerekli state'lere bind ediliyor.
    /// </summary>
    public interface IAnimationHandler {
        void SetMovementSpeed(float speed);

        void SetAttack(bool value);
        
        void SetShootSpeed(float speed);
    }
}