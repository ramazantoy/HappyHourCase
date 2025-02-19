using Interfaces;
using Player;

namespace States
{
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