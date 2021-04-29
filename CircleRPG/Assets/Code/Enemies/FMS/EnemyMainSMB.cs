using UnityEngine;

namespace Code.Enemies.FMS
{
    public class EnemyMainSMB : StateMachineBehaviour
    {
        public int _stopUpdateParam             = Animator.StringToHash("bStopUpdate");
        public int _playerInSightRangeParam = Animator.StringToHash("PlayerInSightRange");
        public int _playerInAttackRangeParam = Animator.StringToHash("PlayerInAttackRange");
        public int _movingParam     = Animator.StringToHash("Moving");
        public int _dieParam        = Animator.StringToHash("Die");
        public int _attackParam     = Animator.StringToHash("Attack");
        public int _moveRandomParam = Animator.StringToHash("MoveRandom");
        
        [SerializeField] private LayerMask _groundLayer = UnityConstants.Layers.FloorMask;
        [SerializeField] private LayerMask _playerLayer = UnityConstants.Layers.ChampionMask;
        [SerializeField] private float _attackRange = 3f;
        [SerializeField] private float _sightRange  = 5f;
        [SerializeField] private bool bUpdate;

        private Animator _animator;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
                                          int      layerIndex)
        {
            _animator = animator;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
                                         int      layerIndex)
        {
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                           int      layerIndex)
        {
            bUpdate = _animator.GetBool(_stopUpdateParam);

            if(bUpdate) return;
            
            _animator.SetBool(_stopUpdateParam, true);

            var playerInAttackRange = Physics.CheckSphere(_animator.transform.position,
                                                          _attackRange, _playerLayer);
            _animator.SetBool(_playerInAttackRangeParam, playerInAttackRange);

            var playerInSightRange = Physics.CheckSphere(_animator.transform.position,
                                                         _sightRange, _playerLayer);
            _animator.SetBool(_playerInSightRangeParam, playerInSightRange);

            if(!playerInAttackRange)
            {
                if(!playerInSightRange)
                {
                    Debug.Log("Move Random");
                    _animator.SetBool(_moveRandomParam, true);
                    _animator.SetBool(_movingParam,     true);
                }
                else
                {
                    Debug.Log("Move Player");
                    _animator.SetBool(_moveRandomParam, false);
                    _animator.SetBool(_movingParam,     true);
                }
            }
            else
            {
                Debug.Log("Attack Player");
                _animator.SetTrigger(_attackParam);
            }
        }

        public override void OnStateMachineExit(Animator animator,
                                                int      stateMachinePathHash)
        {
        }

        public override void OnStateMachineEnter(Animator animator,
                                                 int      stateMachinePathHash)
        {
        }
    }
}