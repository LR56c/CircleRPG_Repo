using Code.Enemies.Types;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

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
        [SerializeField] private int            _numJumps          = 1;
        [SerializeField] private float          _jumpTweenDuration = 2.0f;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private Transform      _dangerZone;


        public override void OnStart(Animator animator)
        {
            base.OnStart(animator);
            _walkMask = _navMeshAgent.areaMask;
            var log = m_MonoBehaviour.GetComponent<LogEnemyBehaviour>();
            m_MonoBehaviour = log;
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
                                          AnimatorControllerPlayable controller)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex, controller);
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
            _dangerZone.gameObject.SetActive(true);
            _dangerZone.position = m_MonoBehaviour.transform.position;
            _dangerZone.DOMove(point, (_jumpTweenDuration / 2));

            m_MonoBehaviour.transform.DOJump(point, _jumpPower, _numJumps,
                                             _jumpTweenDuration)
                           .SetEase(_animationCurve)
                           .OnComplete(() =>
                           {
                               NavMeshCancel();
                               _dangerZone.position = m_MonoBehaviour.transform.position;
                               _dangerZone.gameObject.SetActive(false);
                           });
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
                                         AnimatorControllerPlayable controller)
        {
            base.OnStateExit(animator, stateInfo, layerIndex, controller);
        }

        public void SetSpecificDangerZone(Transform zone)
        {
            _dangerZone = zone;
        }
        
        protected override void NavMeshShouldCancel() {}
    }
    
    /*
        TODO: GetRandomPoint respaldo
       private Vector3 GetRandomPoint()
       {
           float randomZ = Random.Range(-1f, 1f);
           float randomX = Random.Range(-1f, 1f);
           var myPos = m_MonoBehaviour.transform.position;

           return new Vector3(myPos.x + randomX, myPos.y, myPos.z + randomZ);
       }
       
       private Vector3 GetRandomPointXZ()
        {
            float randomZ = Random.Range(-_randomPointRange, _randomPointRange);
            float randomX = Random.Range(-_randomPointRange, _randomPointRange);
            var myPos = _animator.transform.position;

            return new Vector3(myPos.x + randomX, myPos.y, myPos.z + randomZ);
        }
    */
}