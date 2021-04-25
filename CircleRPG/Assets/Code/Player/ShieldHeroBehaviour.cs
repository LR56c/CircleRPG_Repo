using Code.Utility;

namespace Code
{
    public class ShieldHeroBehaviour : HeroBaseBehaviour
    {
        protected override void RegisterHero()
        {
            ServiceLocator.Instance.RegisterService(this);
        }
    }
}