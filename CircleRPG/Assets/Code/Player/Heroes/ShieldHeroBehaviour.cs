using System;
using Code.Domain.Interfaces;
using Code.UI;
using Code.Utility;
using UnityEngine;

namespace Code.Player.Heroes
{
    public class ShieldHeroBehaviour : HeroBaseBehaviour
    {
        protected override int GetHeroEType() => (int) EAbilityType.Shield;

        protected override void DamageReceivedNotify(bool isDead)
        {
            
        }
    }
}