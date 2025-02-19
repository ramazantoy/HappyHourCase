using Interfaces;
using UnityEngine;

namespace Handlers
{
    public class AnimatorHandler : IAnimationHandler {
        private readonly Animator animator;

        public AnimatorHandler(Animator animator) {
            this.animator = animator;
        }

        public void SetMovementSpeed(float speed) {
            animator.SetFloat("Speed", speed);
        }
        public void SetAttack(bool attack) {
            animator.SetBool("Attack", attack);
        }

        public void SetShootSpeed(float speed)
        {
            animator.SetFloat("ShootSpeed", speed);
        }
    }
}