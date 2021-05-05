using System;
using Code.Enemies.Types;
using DG.Tweening;
using UnityEngine;

namespace Code.Player
{
    public class ArcherAbilityZoneBehaviour : MonoBehaviour
    {
        [SerializeField] private bool  bCanAttack     = false;
        [SerializeField] private float _secondsToWait = 5.0f;
        [SerializeField] private int   _damage        = 5;
        private                  Tween Delay;
        
        private void OnTriggerStay(Collider other)
        {
            var enemy = other.GetComponent<EnemyBaseBehaviour>();
            
            if(!enemy) return;
            
            if(bCanAttack) return;
            bCanAttack = true;
            enemy.DamageReceived(_damage);
            Delay = DOVirtual.DelayedCall(_secondsToWait, () => {bCanAttack = false;});
        }
    }
}