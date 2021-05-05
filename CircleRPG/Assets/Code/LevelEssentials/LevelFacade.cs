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
        private                  ArcherAbility        _archerAbility;
        
        private void Start()
        {
            _playerGroup = ServiceLocator.Instance.GetService<PlayerGroupBehaviour>();
            _uiLoader = ServiceLocator.Instance.GetService<UILoader>();
            _archerAbility = ServiceLocator.Instance.GetService<ArcherAbility>();

            LoadZoneUpdate();
            //DOVirtual.DelayedCall(_delayStartCall, LoadZoneUpdate);
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
                //TODO: agregar index niveles completados, para asi actualizar texto
                //y al darle play en el hub denuevo mande a la siguiente escena
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