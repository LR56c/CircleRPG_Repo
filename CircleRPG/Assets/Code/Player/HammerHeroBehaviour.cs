using Code.Utility;

namespace Code.Player
{
    public class HammerHeroBehaviour : HeroBaseBehaviour
    {
        protected override void RegisterHero()
        {
            ServiceLocator.Instance.RegisterService(this);
        }
    }
}