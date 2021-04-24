using System;
using Code.Utility;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Code
{
    public class UILoader : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _loadingCanvasGroup;

        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(this);
            DontDestroyOnLoad(gameObject);    
        }

        //talvez se podria hcer una interfaz IEnableGroup
        public void EnableLoadingGroup(bool value, float duration)
        {
            float fadeValue = value ? 1f : 0f;
            _loadingCanvasGroup.DOFade(fadeValue, duration);
        }
    }
}