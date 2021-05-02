using Code.Player.Heroes;
using DG.Tweening;
using UnityEngine;

namespace Code.Enemies.Triggers
{
    public class EnemyStayTriggerAttackBehaviour : MonoBehaviour
    {
        [SerializeField] private bool  bCanAttack     = false;
        [SerializeField] private float _secondsToWait = 5.0f;
        [SerializeField] private int   _damage        = 5;
        private                  Tween Delay;

        private void OnTriggerStay(Collider other)
        {
            var player = other.GetComponent<HeroBaseBehaviour>();

            if(!player) return;

            DoStay();

            if(bCanAttack) return;
            bCanAttack = true;
            player.DamageReceived(_damage);
            Delay = DOVirtual.DelayedCall(_secondsToWait, () => {bCanAttack = false;});
        }

        protected virtual void DoStay() {}

        private void OnDisable()
        {
            Delay.Kill();
        }
    }
}