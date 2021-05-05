using Code.Player.Heroes;
using UnityEngine;

namespace Code.Player
{
    public abstract class HeroAbilityBase : MonoBehaviour
    {
        protected virtual void Awake()
        {
            Register();
        }

        protected abstract void Register();


        public void Ability()
        {
            if(CanAbility())
            {
                DoAbility();
            }
        }
        
        protected abstract bool CanAbility();

        protected abstract void DoAbility();
    }
}