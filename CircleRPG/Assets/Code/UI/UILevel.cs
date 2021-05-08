using System;
using Code.LevelEssentials;
using Code.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.UI
{
    public class UILevel : MonoBehaviour
    {
        //no poner service locator, se vinculara en cada level
        private Action   OnLevelCompleted;
        private UILoader _uiLoader;

        [SerializeField] private float _fadeDuration = 0.5f;

        [Header("Buttons")] [SerializeField] private Button _pauseButton;
        [SerializeField]                     private Button _resumeButton;
        [SerializeField]                     private Button _restartButton;
        [SerializeField]                     private Button _nextLevelButton;
        [SerializeField]                     private Button _hubButton;

        [Header("CanvasGroups")] [SerializeField]
        private CanvasGroup ThisCanvasGroup;

        [SerializeField] private CanvasGroup WinGroup;
        [SerializeField] private CanvasGroup LoseGroup;
        [SerializeField] private CanvasGroup PauseGroup;

        private void OnEnable()
        {
            _uiLoader = ServiceLocator.Instance.GetService<UILoader>();
            _pauseButton.onClick.AddListener(OnPause);
            _resumeButton.onClick.AddListener(OnResume);
            _restartButton.onClick.AddListener(OnRestart);
            _nextLevelButton.onClick.AddListener(OnNextLevel);
            _hubButton.onClick.AddListener(GoHub);
        }

        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(OnPause);
            _resumeButton.onClick.RemoveListener(OnResume);
            _restartButton.onClick.RemoveListener(OnRestart);
            _nextLevelButton.onClick.RemoveListener(OnNextLevel);
            _hubButton.onClick.RemoveListener(GoHub);
        }

        public void OnWin(Action callback)
        {
            OnLevelCompleted = callback;
            EnableCanvasGroup(ThisCanvasGroup, true, _fadeDuration);
            EnableCanvasGroup(WinGroup,        true, _fadeDuration);
            DOVirtual.DelayedCall(_fadeDuration, () =>
            {
                PauseTime(true);
            });
        }

        public void OnLose()
        {
            EnableCanvasGroup(ThisCanvasGroup, true, _fadeDuration);
            EnableCanvasGroup(LoseGroup,       true, _fadeDuration);
            DOVirtual.DelayedCall(_fadeDuration, () =>
            {
                PauseTime(true);
            });
        }

        private void OnNextLevel()
        {
            OnLevelCompleted?.Invoke();
        }

        /*
         * TODO: problemas al reiniciar, repasar servicios:
         * joystick
         * playerGroup
         * UIHeroAbility
         * ArcherAbility
         * HammerAbility
         * ShieldAbility
         */
        private void OnRestart()
        {
            var thisScene = SceneManager.GetActiveScene().buildIndex;
            _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync(thisScene));
        }

        private void GoHub()
        {
            _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync(1));
        }

        private void OnPause()
        {
            EnableCanvasGroup(ThisCanvasGroup, true, _fadeDuration);
            EnableCanvasGroup(PauseGroup,      true, _fadeDuration);
            
            DOVirtual.DelayedCall(_fadeDuration, () =>
            {
                PauseTime(true);
            });
        }

        private void OnResume()
        {
            EnableCanvasGroup(ThisCanvasGroup, false, _fadeDuration);
            EnableCanvasGroup(PauseGroup,      false, _fadeDuration);
            PauseTime(false);
        }

        private void PauseTime(bool value)
        {
            int scale = value ? 0 : 1;
            Time.timeScale = scale;
        }

        private void EnableCanvasGroup(CanvasGroup canvasGroup, bool value,
                                       float       duration)
        {
            float fadeValue = value ? 1f : 0f;
            canvasGroup.blocksRaycasts = value;
            canvasGroup.DOFade(fadeValue, duration);
        }
    }
}