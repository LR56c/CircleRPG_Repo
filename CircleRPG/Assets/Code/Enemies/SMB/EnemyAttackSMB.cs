using Code.Utility;
using UnityEngine;

namespace Code.Enemies.SMB
{
    public class EnemyAttackSMB : MySceneLinkedSMB<EnemyBaseBehaviour>
    {
        private int _cameFromAttackParam = Animator.StringToHash("CameFromAttack");
        private int _toAttackParam       = Animator.StringToHash("ToAttack");

        public override void OnStart(Animator animator)
        {
        }

        public override void OnSLStateEnter(Animator          animator,
                                            AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.Attack(() => {animator.SetBool(_toAttackParam, false);});
            Debug.Log("attack enter");
        }

        public override void OnSLStateUpdate(Animator          animator,
                                             AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo,
                                           int      layerIndex)
        {
            animator.SetBool(_cameFromAttackParam, true);
            Debug.Log("attack exit");
        }
    }
}