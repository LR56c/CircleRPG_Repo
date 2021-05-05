﻿using System;
using System.Collections.Generic;
using Code.Enemies.Types;
using UnityConstants;
using UnityEngine;

namespace Code.LevelEssentials
{
    public class LevelZone : MonoBehaviour
    {
        [SerializeField]             private LevelFacade _levelFacade;
        [SerializeField]             private Transform   _zoneStart;
        [SerializeField]             private DoorZone    _doorZone;
        [SerializeField]             private Collider    _archerAbilityArea;
        [SerializeField] private GameObject           _enemysContainer;
        

        [SerializeField]             private EnemyBaseBehaviour[] _enemys;
        [SerializeField] private int                        _enemyNumbers = 99;

        public Vector3 ZoneStartPosition => _zoneStart.position;
        
        private void OnEnable()
        {
            _enemys = _enemysContainer.GetComponentsInChildren<EnemyBaseBehaviour>();

            foreach(EnemyBaseBehaviour enemy in _enemys)
            {
                enemy.OnDied += EnemyOnOnDied;
            }
            _enemyNumbers = _enemys.Length;
        }
        
        private void OnDisable()
        {
            foreach(EnemyBaseBehaviour enemy in _enemys)
            {
                enemy.OnDied -= EnemyOnOnDied;
            }
        }

        public void ZoneComplete()
        {
            _levelFacade.LevelUpdate();
        }

        private void EnemyOnOnDied()
        {
            _enemyNumbers--;

            if(_enemyNumbers > 0) return;

            _enemyNumbers = 0;
            _doorZone.Open();
        }
    }
}