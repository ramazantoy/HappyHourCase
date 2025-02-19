using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SkillButton : MonoBehaviour
    {
        [SerializeField] private List<Color> _activeColors;
        [SerializeField] private Color _inactiveColor;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Button _button;

        private bool _isActive;
        private Coroutine _colorChangeRoutine;

        private void Start()
        {
            _button.onClick.AddListener(ToggleSkill);
            UpdateVisualState();
        }

        private void ToggleSkill()
        {
            _isActive = !_isActive;
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            if (_isActive)
            {
                if (_colorChangeRoutine != null)
                    StopCoroutine(_colorChangeRoutine);

                _colorChangeRoutine = StartCoroutine(ChangeColorRoutine());
            }
            else
            {
                if (_colorChangeRoutine != null)
                {
                    StopCoroutine(_colorChangeRoutine);
                    _colorChangeRoutine = null;
                }

                _buttonImage.color = _inactiveColor;
            }
        }

        private IEnumerator ChangeColorRoutine()
        {
            int index = 0;
            while (true)
            {
                _buttonImage.color = _activeColors[index];
                index = (index + 1) % _activeColors.Count;
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}