using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies.SMB
{
    public class EnemyJumpMoveSMB : EnemyMoveBaseSMB
    {
        [Header("Sample")]
        [SerializeField] private float _randomPointRange = 5.0f;
        [SerializeField] private float _sampleMaxDistance = 2.0f;
        private                  int   _walkMask          = 0;
        
        [Header("DOJump")]
        [SerializeField] private float _jumpPower = 5.0f;
        [SerializeField]             private int   _numJumps          = 1;
        [SerializeField]             private float _jumpTweenDuration = 2.0f;
        [SerializeField] private AnimationCurve     _animationCurve;
        

        public override void OnStart(Animator animator)
        {
            base.OnStart(animator);
            _walkMask = _navMeshAgent.areaMask;

        }
        
        protected override void Move()
        {
            var hero = m_MonoBehaviour.GetHero();

            if(!hero) return;

            var point = hero.transform.position;
            
            if(!NavMesh.SamplePosition(point, out NavMeshHit hit,
                                       _sampleMaxDistance, _walkMask))
            {
                Move();
                return;
            }

            point = hit.position;
            m_MonoBehaviour.transform.DOJump(point, _jumpPower, _numJumps,
                                             _jumpTweenDuration)
                           .SetEase(_animationCurve)
                           .OnComplete(NavMeshCancel);
        }

        protected override void NavMeshShouldCancel()
        {
            
        }

        private Vector3 GetRandomPointXZ()
        {
            float randomZ = Random.Range(-_randomPointRange, _randomPointRange);
            float randomX = Random.Range(-_randomPointRange, _randomPointRange);
            var myPos = _animator.transform.position;

            return new Vector3(myPos.x + randomX, myPos.y, myPos.z + randomZ);
        }
    }
    
    /*
        TODO: GetRandomPoint respaldo
       private Vector3 GetRandomPoint()
       {
           float randomZ = Random.Range(-1f, 1f);
           float randomX = Random.Range(-1f, 1f);
           var myPos = m_MonoBehaviour.transform.position;

           return new Vector3(myPos.x + randomX, myPos.y, myPos.z + randomZ);
       }*/
}