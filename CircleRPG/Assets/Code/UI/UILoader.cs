using System;
using System.Collections;
using Code.Utility;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UILoader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup                             _loadingCanvasGroup;
        [SerializeField] private CanvasGroup                             _tempBlackscreenCanvasGroup;
        [SerializeField] private Slider                                  _loadingSlider;
        [SerializeField] private TextMeshProUGUI                         _loadingPercentText;
        private                  AsyncOperation                          _sceneLoading;
        private                  TweenerCore<float, float, FloatOptions> _tweenBlackscreen;
        private                  World                                   _firstWorld;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(this);

            TempInitWorld();
            DontDestroyOnLoad(gameObject);    
        }

        //TODO: recordar sacar al hacer build
        private void TempInitWorld()
        {
            if(PlayerPrefs.HasKey("Level"))
            {
                var savedLevel = PlayerPrefs.GetInt("Level");
                _firstWorld = new World(savedLevel);
            }
            else
            {
                _firstWorld = new World(1);
            }
            
            ServiceLocator.Instance.RegisterService(_firstWorld);
        }

#if UNITY_EDITOR
        private void Start()
        {
            EnableLoadingGroup(false, 0.1f);
        }
#endif

        public void LoadSceneAsync(AsyncOperation scene)
        {
            EnableLoadingGroup(true,0.1f);
            _sceneLoading = scene;
            StartCoroutine(GetLoadProgress());
        }

        private IEnumerator GetLoadProgress()
        {
            while(!_sceneLoading.isDone)
            {
                _loadingPercentText.SetText($"{(_sceneLoading.progress * 100f).ToString()}%");
                _loadingSlider.value = _sceneLoading.progress;
                yield return null;
            }
            
            EnableLoadingGroup(false,0.1f);
        }
        
        public void EnableBlackscreenGroup(bool value, float duration, TweenCallback onComplete)
        {
            EnableBlackscreenGroup(value,duration);
            _tweenBlackscreen.OnComplete(onComplete);
        }
        
        public void EnableBlackscreenGroup(bool value, float duration)
        {
            float fadeValue = value ? 1f : 0f;
            _tempBlackscreenCanvasGroup.blocksRaycasts = value;
            _tweenBlackscreen = _tempBlackscreenCanvasGroup.DOFade(fadeValue, duration);
        }
        
        public void EnableBlackscreenGroup(bool value)
        {
            float fadeValue = value ? 1f : 0f;
            _tempBlackscreenCanvasGroup.blocksRaycasts = value;
            _tempBlackscreenCanvasGroup.alpha = fadeValue;
        }
        
        //talvez se podria hcer una interfaz IEnableGroup
        private void EnableLoadingGroup(bool value, float duration)
        {
            float fadeValue = value ? 1f : 0f;
            _loadingCanvasGroup.blocksRaycasts = value;
            _loadingCanvasGroup.DOFade(fadeValue, duration);
        }
    }
}