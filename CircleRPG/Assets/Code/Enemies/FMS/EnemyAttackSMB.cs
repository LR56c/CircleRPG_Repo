using Code.Utility;
using UnityEngine;

namespace Code.Enemies.FMS
{
    public class EnemyAttackSMB : EnemyLinkedSMB<EnemyBaseBehaviour>
    {
        public  int      _attackParam     = Animator.StringToHash("Attack");
        public  int      _idleParam       = Animator.StringToHash("Idle");
        public  int      _stopUpdateParam = Animator.StringToHash("bStopUpdate");

        public override void OnStart(Animator         animator)
        {
        }

        public override void OnSLStateEnter(Animator  animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _animator.SetBool(_stopUpdateParam, true);
        }
        
        public override void OnSLStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.Attack((() =>
            {
                Debug.Log("attack completed smb");
                _animator.SetBool(_stopUpdateParam, false);
            }));
        }
        
        public override void OnSLStateExit(Animator   animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}