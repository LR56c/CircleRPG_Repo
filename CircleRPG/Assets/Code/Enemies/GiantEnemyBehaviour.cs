using UnityEngine;

namespace Code.Enemies
{
    public class GiantEnemyBehaviour : EnemyBaseBehaviour
    {
        protected override void DoMove()
        {
            
        }

        protected override void DoAttack()
        {
            Debug.Log("DoAttack");
            
            /*Vector3 direction = (/*player#1# - transform.position).normalized;

            Quaternion lookRotation =
                Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            
            transform.rotation =
                Quaternion.Slerp(transform.rotation, lookRotation,
                                 Time.deltaTime * _slerpSpeed);*/
            
            //transform.LookAt(_championController.transform.position);
            
            //instantiate
        }

        protected override void DamageReceivedNotify(bool isDead)
        {
            
        }
    }
}