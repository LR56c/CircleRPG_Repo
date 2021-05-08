using Code.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    
    public class UIHubFacade : MonoBehaviour
    {
        /*[Header("Buttons")]
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _levelsButton;
        [SerializeField] private Button _returnSettingsButton;
        [SerializeField] private Button _returnLevelsButton;
    
        [Header("CanvasGroups")]
        [SerializeField] private CanvasGroup _settingsCanvasGroup;
        [SerializeField] private CanvasGroup _hubCanvasGroup;
        [SerializeField] private CanvasGroup _levelsCanvasGroup;
        
        private void OnEnable()
        {
            _settingsButton.onClick.AddListener(SettingsCallback);
            _levelsButton.onClick.AddListener(LevelsCallback);
            _returnLevelsButton.onClick.AddListener(ReturnFromLevelsCallback);
            _returnSettingsButton.onClick.AddListener(ReturnFromSettingsCallback);
        }
    
        private void OnDisable()
        {
            _settingsButton.onClick.RemoveListener(SettingsCallback);
            _levelsButton.onClick.RemoveListener(LevelsCallback);
            _returnLevelsButton.onClick.RemoveListener(ReturnFromLevelsCallback);
            _returnSettingsButton.onClick.RemoveListener(ReturnFromSettingsCallback);
        }

        private void ReturnFromSettingsCallback()
        {
            EnableCanvasGroup(_settingsCanvasGroup, false, 1f);
            EnableCanvasGroup(_hubCanvasGroup,      true,  1f);
        }

        private void ReturnFromLevelsCallback()
        {
            EnableCanvasGroup(_levelsCanvasGroup, false, 1f);
            EnableCanvasGroup(_hubCanvasGroup,    true,  1f);
        }
    
        private void LevelsCallback()
        {
            EnableCanvasGroup(_hubCanvasGroup,    false, 1f);
            EnableCanvasGroup(_levelsCanvasGroup, true,  1f);
        }

        private void SettingsCallback()
        {
            EnableCanvasGroup(_hubCanvasGroup,      false, 1f);
            EnableCanvasGroup(_settingsCanvasGroup, true,  1f);
        }

        private void EnableCanvasGroup(CanvasGroup canvasGroup, bool value, float duration)
        {
            float fadeValue = value ? 1f : 0f;
            canvasGroup.blocksRaycasts = value;
            canvasGroup.DOFade(fadeValue, duration);
        }*/
    }
}
