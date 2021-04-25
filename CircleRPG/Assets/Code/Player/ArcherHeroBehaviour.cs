using Code.Utility;
using UnityEngine;

namespace Code
{
    public class ArcherHeroBehaviour : HeroBaseBehaviour
    {
        protected override void RegisterHero()
        {
            ServiceLocator.Instance.RegisterService(this);
        }
    }
}