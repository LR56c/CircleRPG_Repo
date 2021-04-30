using System;
using UnityEngine;

namespace Code.Enemies
{
    public class EnemyAttackRangeChildTriggerBehaviour : MonoBehaviour
    {
        [SerializeField]             private Animator _enemyAnimator;
        
        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag(UnityConstants.Tags.Player)) return;
            _enemyAnimator.SetBool("ToAttack", true);
        }

        private void OnTriggerExit(Collider other)
        {
            if(!other.CompareTag(UnityConstants.Tags.Player)) return;
            _enemyAnimator.SetBool("ToAttack", false);
        }
    }
}