using System;
using Code.Domain.Interfaces;
using Code.Utility;
using UnityEngine;

namespace Code.Player.Heroes
{
    public class ArcherHeroBehaviour : HeroBaseBehaviour
    {
        protected override void DoAttack(Collider objetive)
        {
            Debug.Log($"DoAttack Archer, {objetive.gameObject.name}", objetive);
            var enemy = objetive.GetComponentInParent<IDamageable>();
            enemy?.DamageReceived(1);
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
            
        }
    }
}