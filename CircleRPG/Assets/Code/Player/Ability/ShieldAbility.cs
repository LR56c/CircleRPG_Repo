using Code.Player.Heroes;
using Code.Utility;
using DG.Tweening;
using UnityEngine;

namespace Code.Player
{
    public class ShieldAbility : HeroAbilityBase
    {
        [SerializeField] private int                 _healAmount = 20;
        [SerializeField] private                  float               _seconds    = 5.0f;
        [SerializeField] private GameObject          _effect;
        [SerializeField] private HeroBaseBehaviour[] _heroBaseBehaviours;

        protected override bool CanAbility()
        {
            return true;
        }

        protected override void DoAbility()
        {
            _effect.SetActive(true);
            
            foreach(var heroBaseBehaviour in _heroBaseBehaviours)
            {
                heroBaseBehaviour.Heal(_healAmount);
            }

            DOVirtual.DelayedCall(_seconds, () =>
            {
                _effect.SetActive(false);
            });
        }
    }
}