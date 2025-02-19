using System;
using Skills;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillUI : MonoBehaviour
    {
        public Button arrowMultiplicationButton;
        public Button bounceDamageButton;
        public Button burnDamageButton;
        public Button attackSpeedIncreaseButton;
        public Button rageModeButton;

        public PlayerSkills playerSkills;

        private void SetListeners()
        {
            arrowMultiplicationButton.onClick.AddListener(() => { playerSkills.ToggleArrowMultiplication(); });
            bounceDamageButton.onClick.AddListener(() => { playerSkills.ToggleBounceDamage(); });
            burnDamageButton.onClick.AddListener(() => { playerSkills.ToggleBurnDamage(); });
            attackSpeedIncreaseButton.onClick.AddListener(() => { playerSkills.ToggleAttackSpeedIncrease(); });
            rageModeButton.onClick.AddListener(() => { playerSkills.ToggleRageMode(); });
        }

        private void RemoveListeners()
        {
            arrowMultiplicationButton.onClick.RemoveAllListeners();
            burnDamageButton.onClick.RemoveAllListeners();
            attackSpeedIncreaseButton.onClick.RemoveAllListeners();
            rageModeButton.onClick.RemoveAllListeners();
        }

        private void Start()
        {
            SetListeners();
        }

        private void OnDestroy()
        {
            
        }
    }
}