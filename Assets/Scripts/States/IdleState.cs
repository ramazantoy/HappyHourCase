using Interfaces;
using Player;

namespace States
{
    /// <summary>
    /// ICharacter State'in idle state'i idle sırasında sadece animasyonun hareket hızını sıfır yapıyoruz böylelikle blend three idle'a geçiyor.
    /// </summary>
    public class IdleState : ICharacterState {
        
        private readonly PlayerController player;

        public IdleState(PlayerController player) {
            this.player = player;
        }

        public void Enter() {
            // Idle durumunda hız 0 olarak blend tree’de idle animasyona geçiş sağlar.
            player.AnimationHandler.SetMovementSpeed(0f);
        }

        public void Exit() { }

        public void OnTick() {
            // Idle durumunda ek işlem yapılmasına gerek yok.
        }
    }
}