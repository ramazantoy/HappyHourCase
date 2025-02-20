using System;
using UnityEngine;


namespace Skills {
    
    /// <summary>
    /// Oyuncu'nn aktif skillerinin yer aldığı bir mono sınıfı mono olmasının sebebi hem canvas tarafında hemde player tarafında rahatça erişmek amacıyla.
    /// İçerisinde bir action var ama kullanılmadı ileride ona geçilip player'ın onu dinlemesi eklenebilir. Şuan state'ler çalışırken Aktif skilleri kontrol ediyor.
    /// </summary>
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