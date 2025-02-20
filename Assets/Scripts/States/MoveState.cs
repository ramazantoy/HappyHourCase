using Interfaces;
using Player;

namespace States
{
    /// <summary>
    /// Move state ise OnTick ile eğer bu state'de ise karakterin hareket etmesini sağlıyor.
    /// Input provider bu sınıfa readonly olarak eklenebilir.
    /// </summary>
    public class MoveState : ICharacterState {
        
        private PlayerController player;

        public MoveState(PlayerController player) {
            this.player = player;
        }

        public void Enter() { }

        public void Exit() { }

        public void OnTick() {
  
            player.MoveCharacter( player.InputProvider.GetMovement());
        }
    }
}