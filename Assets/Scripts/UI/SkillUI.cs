using Skills;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillUI : MonoBehaviour {
        public Button arrowMultiplicationButton;
        public Button bounceDamageButton;
        public Button burnDamageButton;
        public Button attackSpeedIncreaseButton;
        public Button rageModeButton;

        public PlayerSkills playerSkills;

        private void Start() {
            arrowMultiplicationButton.onClick.AddListener(() => {
                playerSkills.ToggleArrowMultiplication();
                UpdateButtonOutline(arrowMultiplicationButton, playerSkills.arrowMultiplication);
            });
            bounceDamageButton.onClick.AddListener(() => {
                playerSkills.ToggleBounceDamage();
                UpdateButtonOutline(bounceDamageButton, playerSkills.bounceDamage);
            });
            burnDamageButton.onClick.AddListener(() => {
                playerSkills.ToggleBurnDamage();
                UpdateButtonOutline(burnDamageButton, playerSkills.burnDamage);
            });
            attackSpeedIncreaseButton.onClick.AddListener(() => {
                playerSkills.ToggleAttackSpeedIncrease();
                UpdateButtonOutline(attackSpeedIncreaseButton, playerSkills.attackSpeedIncrease);
            });
            rageModeButton.onClick.AddListener(() => {
                playerSkills.ToggleRageMode();
                UpdateButtonOutline(rageModeButton, playerSkills.rageMode);
            });
        }

        private void UpdateButtonOutline(Button button, bool isActive) {
            // Butona ekli Outline component’i aktif veya pasif hale getirir.
            var outline = button.GetComponent<Outline>();
            if (outline != null) {
                outline.enabled = isActive;
            }
        }
    }
}