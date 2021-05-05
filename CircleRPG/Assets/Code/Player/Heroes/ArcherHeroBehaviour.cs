using System;
using Code.Domain.Interfaces;
using Code.UI;
using Code.Utility;
using UnityEngine;

namespace Code.Player.Heroes
{
    public class ArcherHeroBehaviour : HeroBaseBehaviour
    {
        protected override int GetHeroEType() => (int) EAbilityType.Archer;

        protected override void DamageReceivedNotify(bool isDead)
        {
        }
    }
}