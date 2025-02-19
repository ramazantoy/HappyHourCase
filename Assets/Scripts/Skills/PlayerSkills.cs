using System;
using UnityEngine;

namespace Skills {
    public class PlayerSkills : MonoBehaviour {
        
        public bool arrowMultiplication;
        public bool bounceDamage;
        public bool burnDamage;
        public bool attackSpeedIncrease;
        public bool rageMode;

        public event Action OnSkillChanged;

        public void ToggleArrowMultiplication() {
            arrowMultiplication = !arrowMultiplication;
            OnSkillChanged?.Invoke();
        }
        public void ToggleBounceDamage() {
            bounceDamage = !bounceDamage;
            OnSkillChanged?.Invoke();
        }
        public void ToggleBurnDamage() {
            burnDamage = !burnDamage;
            OnSkillChanged?.Invoke();
        }
        public void ToggleAttackSpeedIncrease() {
            attackSpeedIncrease = !attackSpeedIncrease;
            OnSkillChanged?.Invoke();
        }
        public void ToggleRageMode() {
            rageMode = !rageMode;
            OnSkillChanged?.Invoke();
        }
    }
}