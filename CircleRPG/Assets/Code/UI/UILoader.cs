using System.Collections;
using Code.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class UILoader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup     _loadingCanvasGroup;
        [SerializeField] private Slider          _loadingSlider;
        [SerializeField] private TextMeshProUGUI _loadingPercentText;
        private                  AsyncOperation  _sceneLoading;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(this);
            DontDestroyOnLoad(gameObject);    
        }

#if UNITY_EDITOR
        private void Start()
        {
            EnableLoadingGroup(false, 1f);
        }
#endif

        public void LoadSceneAsync(AsyncOperation scene)
        {
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
            
            EnableLoadingGroup(false,1f);
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