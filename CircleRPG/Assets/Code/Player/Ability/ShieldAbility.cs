using Code.Utility;

namespace Code.Player
{
    public class ShieldAbility : HeroAbilityBase
    {
        protected override void            Register()
        {
            ServiceLocator.Instance.RegisterService(this);
        }

        protected override bool CanAbility()
        {
            return true;
        }

        protected override void DoAbility()
        {
            
        }
    }
}