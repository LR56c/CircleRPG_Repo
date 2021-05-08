using System;
using Code.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.UI
{
    public class UIWorld : MonoBehaviour
    {
        private                  UILoader        _uiLoader;
        private                  World           _world;
        [SerializeField] private TextMeshProUGUI _levelText;
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
        }

        public void PlayButton()
        {
            //offset
            _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync(_nextLevel + 1));
        }
    }
}