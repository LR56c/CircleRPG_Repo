using Code.Player.Heroes;
using Code.Utility;
using DG.Tweening;
using UnityEngine;

namespace Code.Player
{
    public class HammerAbility : HeroAbilityBase
    {
        private int _stunParam = Animator.StringToHash("Stuned");
        [SerializeField]             private Collider _collider;
        [SerializeField] private float _seconds = 1.0f;
        
        protected override void            Register()
        {
            ServiceLocator.Instance.RegisterService(this);
        }

        protected override bool CanAbility()
        {
            /*deberia revisar si hay focus enemy
            pero se podria extender a que la ui avise si no hay enemigo cerca lo bloquie
            para que no se tire la habilidad sin querer,
            asi que solo se dejara pasar*/
            return true;
        }

        protected override void DoAbility()
        {
            _collider.enabled = true;

            DOVirtual.DelayedCall(_seconds, () =>
            {
                _collider.enabled = false;
            });
        }
        
        private void OnTriggerEnter(Collider other)
       {
           var enemy = other.GetComponent<Animator>();
           if(!enemy) return;
           enemy.SetBool(_stunParam, true);
       }
    }
}