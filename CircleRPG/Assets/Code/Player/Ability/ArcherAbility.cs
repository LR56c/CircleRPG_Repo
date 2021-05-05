using Code.Utility;
using UnityEngine;

namespace Code.Player
{
    public class ArcherAbility : HeroAbilityBase
    {
        [SerializeField] private GameObject _floorObject;
        
        
        protected override void            Register()
        {
            ServiceLocator.Instance.RegisterService(this);
        }

        protected override bool CanAbility()
        {
            //TODO: aqui pase el objeto nomas, pero deberia pasar el objeto trigger stay justo por encima del floor de cada zona para tener efecto
            return _floorObject;
        }

        protected override void DoAbility()
        {
            
        }
    }
}