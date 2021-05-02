using Code.Enemies.Types;
using Code.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace Code.Enemies.SMB
{
    public abstract class EnemyMoveBaseSMB : MySceneLinkedSMB<EnemyBaseBehaviour>
    {
        protected Rigidbody _rb;
        protected                  int          _cameFromAttackParam = Animator.StringToHash("CameFromAttack");
        protected                  int          _toAttackParam       = Animator.StringToHash("ToAttack");
        protected                  int       _toSightParam = Animator.StringToHash("ToSight");
        [SerializeField] protected bool         bWait                = false;
        [SerializeField] protected bool bAction = false;
        [SerializeField] private float        _secondsToWait       = 3.0f;
        

        public override void OnStart(Animator animator)
        {
            _rb = m_MonoBehaviour.GetComponent<Rigidbody>();
        }

        public override void OnSLStateEnter(Animator  animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }
        
        public override void OnSLStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            MoveBase();
            
        }

        protected virtual void MoveBase()
        {
            if(_animator.GetBool(_cameFromAttackParam))
            {
                WaitDelay();
                return;
            }

            if(bAction)
            {
                NavMeshShouldCancel();
                return;
            }

            bAction = true;
            Move();
        }

        protected abstract void Move();

        protected void WaitDelay()
        {
            if(bWait) return;

            bWait = true;
            DOVirtual.DelayedCall(_secondsToWait,
                                  () =>
                                  {
                                      _animator.SetBool(_cameFromAttackParam,
                                                       false);
                                  });

        }
        
        public override void OnSLStateExit(Animator   animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bWait = false;
        }
        
        
        protected virtual void NavMeshShouldCancel()
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