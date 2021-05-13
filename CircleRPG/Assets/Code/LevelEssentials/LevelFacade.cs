using System;
using Code.Player;
using Code.UI;
using Code.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Code.LevelEssentials
{
    public class LevelFacade : MonoBehaviour
    {
        [Header("External")]
        [SerializeField] private PlayerGroupBehaviour _playerGroup;
        [SerializeField]                      private ArcherAbility        _archerAbility;

        private UILoader _uiLoader;
        private World    _world;

        [Header("Config")]
        
        [SerializeField] private int         _currentLevelIndex = 0;
        [SerializeField]                    private float       _changeZoneFade    = 0.5f;
        [SerializeField]                    private LevelZone[] _levelZones;

        [SerializeField] private Slider _zoneProgressSlider;
        [SerializeField] private float _progressBarSliderFade = 0.5f;

        [SerializeField] private UILevel _uiLevel;

        private void Start()
        {
            var locator = ServiceLocator.Instance;

            _uiLoader = locator.GetService<UILoader>();
            _world = locator.GetService<World>();

            LoadZoneUpdate();
        }

        public void LevelUpdate()
        {
            if(!CanUpdate()) return;
            PlayerPosUpdate();
            LoadZoneUpdate();
        }

        private bool CanUpdate()
        {
            if(_currentLevelIndex >= _levelZones.Length - 1)
            {
                _uiLevel.OnWin(() =>
                {
                    _world.CheckCanAddNextLevel();
                    _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync(UnityConstants
                                                 .Scenes.Mauricio_Hub));
                });
                _currentLevelIndex = _levelZones.Length;
                return false;
            }

            _currentLevelIndex++;
            return true;
        }

        private void PlayerPosUpdate()
        {
            var nextZone = _levelZones[_currentLevelIndex];

            _uiLoader.EnableBlackscreenGroup(true);
            _playerGroup.MoveHeroes(nextZone.ZoneStartPosition);

            DOVirtual.DelayedCall(_changeZoneFade, () =>
            {
                if(_zoneProgressSlider)
                {
                    float percent = (float) _currentLevelIndex / _levelZones.Length;

                    _uiLoader.EnableBlackscreenGroup(false, 1f,
                                                     () =>
                                                     {
                                                         _zoneProgressSlider.DOValue(percent, _progressBarSliderFade);
                                                     });
                }
                else
                {
                    _uiLoader.EnableBlackscreenGroup(false, 1f);
                }
            });
        }

        private void LoadZoneUpdate()
        {
            for(int i = 0; i < _levelZones.Length; i++)
            {
                if(i == _currentLevelIndex)
                {
                    _levelZones[_currentLevelIndex].gameObject.SetActive(true);
                    _archerAbility.ConfigCollider(_levelZones[_currentLevelIndex]
                                                      .GetArcherArea());
                }
                else
                {
                    _levelZones[i].gameObject.SetActive(false);
                }
            }
        }
    }
}