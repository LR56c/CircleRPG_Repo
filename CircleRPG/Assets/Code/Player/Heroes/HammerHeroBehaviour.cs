using System;
using Code.Domain.Interfaces;
using Code.Utility;
using UnityEngine;

namespace Code.Player.Heroes
{
    public class HammerHeroBehaviour : HeroBaseBehaviour
    {
        protected override void DoAttack(Collider objetive)
        {
            Debug.Log($"DoAttack Hammer, {objetive.gameObject.name}", objetive);
            //TODO: preguntar al profe, sino colocar monoBehaviour a collider
            var enemy = objetive.GetComponentInParent<IDamageable>();
            enemy?.DamageReceived(1);
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
        }
    }
}