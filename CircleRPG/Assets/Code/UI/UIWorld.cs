using System;
using Code.Utility;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.UI
{
    public class UIWorld : MonoBehaviour
    {
        private                              UILoader        _uiLoader;
        private                              World           _world;
        [SerializeField]             private TextMeshProUGUI _levelText;
        [SerializeField] private Image               _totalLevelsProgressionBar;
        
        private                  int             _nextLevel = 0;
        
        private void Start()
        {
            _world = ServiceLocator.Instance.GetService<World>();
            _uiLoader = ServiceLocator.Instance.GetService<UILoader>();
            WorldPanelUpdate();
        }

        private void WorldPanelUpdate()
        {
            _nextLevel = _world.GetCurrentLevel();
            _levelText.SetText($"Level {_nextLevel.ToString()}");
            float percent = (float)_nextLevel / _world.GetMaxLevels();
            _totalLevelsProgressionBar.DOFillAmount(percent, 0.1f);
        }

        public void PlayButton()
        {
            //offset
            _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync(_nextLevel + 1));
        }
    }
}