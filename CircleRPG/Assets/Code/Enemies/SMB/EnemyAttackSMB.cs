using Code.Enemies.Types;
using Code.Utility;
using UnityEngine;

namespace Code.Enemies.SMB
{
    public  class EnemyAttackSMB : MySceneLinkedSMB<EnemyBaseBehaviour>
    {
        private int          _cameFromAttackParam = Animator.StringToHash("CameFromAttack");
        private int          _toAttackParam       = Animator.StringToHash("ToAttack");
        private float _cachedNavSpeed = 0f;
        
        public override void OnStart(Animator animator)
        {
        }

        public override void OnSLStateEnter(Animator          animator,
                                            AnimatorStateInfo stateInfo, int layerIndex)
        {
            _cachedNavSpeed = _navMeshAgent.speed;
            _navMeshAgent.speed = 0f;
            m_MonoBehaviour.Attack(() => {animator.SetBool(_toAttackParam, false);});
        }

        public override void OnSLStateUpdate(Animator          animator,
                                             AnimatorStateInfo stateInfo, int layerIndex)
        {
         
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo,
                                           int      layerIndex)
        {
            _navMeshAgent.speed = _cachedNavSpeed;
            animator.SetBool(_cameFromAttackParam, true);
            m_MonoBehaviour.OnAttackComplete?.Invoke();
        }
    }
}