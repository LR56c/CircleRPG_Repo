using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Code.Enemies
{
    public class GiantEnemyBehaviour : EnemyBaseBehaviour
    {
        [SerializeField] private GameObject[] _ballsPositions;
        [SerializeField] private GameObject _ballPrefab;

        protected override void DoMove(Vector3 destination)
        {
            _navMeshAgent.SetDestination(destination);
        }
        
        protected override void DoAttack()
        {
            Debug.Log($"DoAttack Giant to");
            
            /*Vector3 direction = (player- transform.position).normalized;

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
        
        public override void AttackFinish()
        {
            OnAttackComplete?.Invoke();
        }

        public void DisableBalls()
        {
            foreach(GameObject ball in _ballsPositions)
            {
                ball.SetActive(false);
            }
        }

        public void ThrowBalls()
        {
            GameObject go = Instantiate(_ballPrefab, _ballsPositions[0].transform.position, Quaternion.identity);
            go.SetActive(true);
        }
    }
}