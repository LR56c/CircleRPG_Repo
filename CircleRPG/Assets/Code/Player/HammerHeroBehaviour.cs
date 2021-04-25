using Code.Utility;

namespace Code
{
    public class HammerHeroBehaviour : HeroBaseBehaviour
    {
        protected override void RegisterHero()
        {
            ServiceLocator.Instance.RegisterService(this);
        }
    }
}