using Code.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies.SMB
{
    public abstract class EnemyMoveSMB : MySceneLinkedSMB<EnemyBaseBehaviour>
    {
        protected                NavMeshAgent _navMeshAgent;
        private                  int          _cameFromAttackParam = Animator.StringToHash("CameFromAttack");
        private                  int          _toAttackParam       = Animator.StringToHash("ToAttack");
        private                  int       _toSightParam = Animator.StringToHash("ToSight");
        [SerializeField] private bool         bWait                = false;
        [SerializeField] private float        _secondsToWait       = 3.0f;

        public override void OnStart(Animator animator)
        {
            _navMeshAgent = m_MonoBehaviour.GetComponent<NavMeshAgent>();
        }

        public override void OnSLStateEnter(Animator  animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }
        
        public override void OnSLStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(animator.GetBool(_cameFromAttackParam))
            {
                WaitDelay(animator);
                return;
            }

            if(bAction)
            {
                NavMeshShouldCancel();
                return;
            }

            Move();
            bAction = true;
        }

        protected abstract void Move();

        private void WaitDelay(Animator animator)
        {
            if(bWait) return;

            bWait = true;
            Debug.Log("move wait");
            DOVirtual.DelayedCall(_secondsToWait,
                                  () =>
                                  {
                                      animator.SetBool(_cameFromAttackParam,
                                                       false);
                                      Debug.Log("exit move wait");
                                  });

        }
        
        public override void OnSLStateExit(Animator   animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bWait = false;
        }
        
        
        private void NavMeshShouldCancel()
        {
            bool playerInAttackRangeParam = _animator.GetBool(_toAttackParam);

            bool remainingDistanceToPoint = _navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance;
            
            if(_navMeshAgent.hasPath && remainingDistanceToPoint && !playerInAttackRangeParam)
                return;
         
            Debug.Log("cancel nav");
            NavMeshCancel();
        }

        private void NavMeshCancel()
        {
            _navMeshAgent.ResetPath();
            bAction = false;

            if(m_MonoBehaviour.CanAttack())
            {
                _animator.SetBool(_toAttackParam,true);
                Debug.Log("attack for nav");
            }
            else
            {
                _animator.SetBool(_toSightParam, false);
            }
        }
    }
}