using System;
using UnityEngine;

namespace Code.Enemies
{
    public class GiantEnemyBehaviour : EnemyBaseBehaviour
    {
        protected override void DoMove()
        {
            _navMeshAgent.SetDestination(GetHeroPosition().transform.position);
        }

        protected override void DoAttack(Action onComplete)
        {
            Debug.Log("DoAttack");
            onComplete?.Invoke();
            
            /*Vector3 direction = (/*player#1# - transform.position).normalized;

            Quaternion lookRotation =
                Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            
            transform.rotation =
                Quaternion.Slerp(transform.rotation, lookRotation,
                                 Time.deltaTime * _slerpSpeed);*/
            
            //transform.LookAt(_championController.transform.position);
            
            //TODO: instantiate por anim
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
            
        }
    }
}