using Code.Enemies.Types;
using Code.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies.SMB
{
    public abstract class EnemyMoveBaseSMB : MySceneLinkedSMB<EnemyBaseBehaviour>
    {
        protected                NavMeshAgent _navMeshAgent;
        protected Rigidbody _rb;
        private                  int          _cameFromAttackParam = Animator.StringToHash("CameFromAttack");
        private                  int          _toAttackParam       = Animator.StringToHash("ToAttack");
        private                  int       _toSightParam = Animator.StringToHash("ToSight");
        [SerializeField]             private bool         bWait                = false;
        [SerializeField] protected bool bAction = false;
        [SerializeField]             private float        _secondsToWait       = 3.0f;
        

        public override void OnStart(Animator animator)
        {
            _navMeshAgent = m_MonoBehaviour.GetComponent<NavMeshAgent>();
            _rb = m_MonoBehaviour.GetComponent<Rigidbody>();
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
            DOVirtual.DelayedCall(_secondsToWait,
                                  () =>
                                  {
                                      animator.SetBool(_cameFromAttackParam,
                                                       false);
                                  });

        }
        
        public override void OnSLStateExit(Animator   animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bWait = false;
        }
        
        
        private void NavMeshShouldCancel()
        {
            if(_navMeshAgent.remainingDistance > _navMeshAgent.stoppingDistance) return;

            NavMeshCancel();
        }

        protected virtual void NavMeshCancel()
        {
            bAction = false;
            
            _navMeshAgent.ResetPath();
            
            if(m_MonoBehaviour.CanAttack())
            {
                _animator.SetBool(_toAttackParam,true);
            }
            else
            {
                _animator.SetBool(_toSightParam, false);
            }
        }
    }
}