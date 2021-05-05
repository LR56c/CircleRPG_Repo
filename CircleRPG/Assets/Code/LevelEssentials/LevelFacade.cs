using System;
using Code.Player;
using Code.UI;
using Code.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.LevelEssentials
{
    public class LevelFacade : MonoBehaviour
    {
        [SerializeField] private LevelZone[]          _levelZones;
        [SerializeField] private int                  _currentLevelIndex = 0;
        private                  PlayerGroupBehaviour _playerGroup;
        private                  UILoader             _uiLoader;
        
        /*var countTest = 0;
            
            for(int i = 0; i < _levelZones.Length - 1; i++)
            {
                countTest++;
            }

            Debug.Log($"countTest: {countTest.ToString()}");
            */
        
        private void Start()
        {
            _playerGroup = ServiceLocator.Instance.GetService<PlayerGroupBehaviour>();
            _uiLoader = ServiceLocator.Instance.GetService<UILoader>();
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
                _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync(1));
                _currentLevelIndex = _levelZones.Length;
                return false;
            }

            _currentLevelIndex++;
            return true;
        }

        private void PlayerPosUpdate()
        {
            var nextZone = _levelZones[_currentLevelIndex];

            _playerGroup.ForceNavMeshHeroes();
            
            _playerGroup.EnableHeroCollider(false);
            
            _uiLoader.EnableBlackscreenGroup(true);
            
            _playerGroup.SetPosition(nextZone.ZoneStartPosition);

            DOVirtual.DelayedCall(1f, () =>
            {
                _playerGroup.EnableHeroCollider(true);
                _uiLoader.EnableBlackscreenGroup(false, 1f);
            });
        }

        private void LoadZoneUpdate()
        {
            for(int i = 0; i < _levelZones.Length; i++)
            {
                if(i == _currentLevelIndex)
                {
                    _levelZones[_currentLevelIndex].gameObject.SetActive(true);
                }
                else
                {
                    _levelZones[i].gameObject.SetActive(false);
                }
            }
        }
    }
}