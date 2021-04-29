using Code.Utility;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace Code.Enemies.FMS
{
    public abstract class EnemyMovingBaseSMB : EnemyLinkedSMB<EnemyBaseBehaviour>
    {
        //TODO: colocar params a private y ver componentes necesarios en la clase base
        protected NavMeshAgent _navMeshAgent;
        protected   int          _areaMask        = 0;
        public    int          _moveRandomParam = Animator.StringToHash("MoveRandom");
        public    int          _movingParam     = Animator.StringToHash("Moving");
        public    int          _stopUpdateParam = Animator.StringToHash("bStopUpdate");
        public int _playerInAttackRangeParam = Animator.StringToHash("PlayerInAttackRange");


        public override void OnStart(Animator animator)
        {
            _navMeshAgent = m_MonoBehaviour.GetComponent<NavMeshAgent>();
            _areaMask = _navMeshAgent.areaMask;
        }

        public override void OnSLStateEnter(Animator  animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _animator.SetBool(_stopUpdateParam, true);
        }
        
        public override void OnSLStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(bAction)
            {
                NavMeshShouldCancel();
                return;
            }

            Move();
        }

        public override void OnSLStateExit(Animator   animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }


        protected abstract void Move();
        
        private void NavMeshShouldCancel()
        {
            bool playerInAttackRangeParam = _animator.GetBool(_playerInAttackRangeParam);

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
            _animator.SetBool(_movingParam,     false);
            _animator.SetBool(_moveRandomParam, false);
            _animator.SetBool(_stopUpdateParam, false);
        }
    }
}