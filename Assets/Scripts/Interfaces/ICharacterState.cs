namespace Interfaces
{
    
    /// <summary>
    /// Oyunca yer alan state machine'in interface'i
    /// </summary>
    public interface ICharacterState {
        void Enter();
        void Exit();
        void OnTick();
    }
}