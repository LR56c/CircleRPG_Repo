using Code.Utility;

namespace Code.Player
{
    public class ArcherHeroBehaviour : HeroBaseBehaviour
    {
        protected override void RegisterHero()
        {
            ServiceLocator.Instance.RegisterService(this);
        }
    }
}