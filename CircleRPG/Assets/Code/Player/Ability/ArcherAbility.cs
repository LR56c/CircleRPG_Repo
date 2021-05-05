﻿using System;
using Code.LevelEssentials;
using Code.Utility;
using DG.Tweening;
using UnityEngine;

namespace Code.Player
{
    public class ArcherAbility : HeroAbilityBase
    {
        private                              Collider _floorColliderStay;
        [SerializeField] private float        _secondsToBurn = 3.0f;
        

        protected override void Register()
        {
            ServiceLocator.Instance.RegisterService(this);
        }

        protected override bool CanAbility()
        {
            return _floorColliderStay;
        }

        protected override void DoAbility()
        {
            _floorColliderStay.enabled = true;

            DOVirtual.DelayedCall(_secondsToBurn, () =>
            {
                _floorColliderStay.enabled = false;
            });
        }

        public void ConfigCollider(Collider collider)
        {
            _floorColliderStay = collider;
        }
    }
}