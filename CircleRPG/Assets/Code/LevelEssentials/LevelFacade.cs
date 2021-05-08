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
        
        private                  PlayerGroupBehaviour _playerGroup;
        private                  UILoader             _uiLoader;
        private                  ArcherAbility        _archerAbility;
        private                  World                _world;
        [SerializeField] private int                  _currentLevelIndex = 0;
        [SerializeField] private float                _changeZoneFade = 0.5f;
        [SerializeField] private LevelZone[]          _levelZones;
        
        [SerializeField] private Image _zoneProgressBar;
        [SerializeField] private float _progressBarFade = 0.5f;

        [SerializeField] private UILevel _uiLevel;

        //curent / levels.count
        private void Start()
        {
            var locator = ServiceLocator.Instance;
            
            _playerGroup = locator.GetService<PlayerGroupBehaviour>();
            _uiLoader = locator.GetService<UILoader>();
            _archerAbility = locator.GetService<ArcherAbility>();
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
                    _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync( UnityConstants.Scenes.Mauricio_Hub));
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
                if(_zoneProgressBar)
                {
                    float percent = (float) _currentLevelIndex / _levelZones.Length;

                    _uiLoader.EnableBlackscreenGroup(false, 1f, () =>
                    {
                        _zoneProgressBar.DOFillAmount(percent, _progressBarFade);
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
                    _archerAbility.ConfigCollider(_levelZones[_currentLevelIndex].GetArcherArea());
                }
                else
                {
                    _levelZones[i].gameObject.SetActive(false);
                }
            }
        }
    }
}