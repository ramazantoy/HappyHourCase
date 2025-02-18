using Interfaces;

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
            var horizontal = player.InputProvider.GetHorizontal();
            player.MoveCharacter(horizontal);
        }
    }
}