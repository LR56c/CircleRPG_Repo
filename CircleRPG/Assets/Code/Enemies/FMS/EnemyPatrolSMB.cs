using Code.Utility;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies.FMS
{
    public class EnemyPatrolSMB : EnemyMovingBaseSMB
    {
        private float   _walkPointRange = 5.0f;
        private Vector3 _validPoint;

        protected override void Move()
        {
            var point = GetRandomPointInXZ();

            if(!NavMesh.SamplePosition(point, out NavMeshHit hit,
                                       _walkPointRange, _areaMask)) 
                return;

            _validPoint = hit.position;
            Debug.DrawRay(_validPoint, Vector3.up, Color.cyan, 10f);
            _navMeshAgent.SetDestination(_validPoint);
            bAction = true;
        }

        private Vector3 GetRandomPointInCircle()
        {
            var value = Random.Range(-_walkPointRange, _walkPointRange) *
                        Random.insideUnitCircle;

            return new Vector3(value.x, 0, value.y);
        }

        private Vector3 GetRandomPointInXZ()
        {
            float randomZ = Random.Range(-_walkPointRange, _walkPointRange);
            float randomX = Random.Range(-_walkPointRange, _walkPointRange);
            var myPos = _animator.transform.position;

            return new Vector3(myPos.x + randomX, myPos.y, myPos.z + randomZ);
        }
    }
}