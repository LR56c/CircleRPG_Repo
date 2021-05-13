using System;
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

        protected override bool CanAbility()
        {
            return _floorColliderStay;
        }

        protected override void DoAbility()
        {
            _floorColliderStay.enabled = true;
            _floorColliderStay.gameObject.SetActive(true);

            DOVirtual.DelayedCall(_secondsToBurn, () =>
            {
                _floorColliderStay.enabled = false;
                _floorColliderStay.gameObject.SetActive(false);
            });
        }

        public void ConfigCollider(Collider collider)
        {
            _floorColliderStay = collider;
        }
    }
}