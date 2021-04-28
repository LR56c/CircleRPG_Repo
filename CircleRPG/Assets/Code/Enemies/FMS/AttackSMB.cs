using Code.Utility;
using UnityEngine;

namespace Code.Enemies.FMS
{
    public class AttackSMB : SceneLinkedSMB<EnemyBaseBehaviour>
    {
        private int _attackParam = Animator.StringToHash("Attack");
        
        public override void OnStart(Animator animator)
        {
            
        }

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.SetUpdate(false);
        }
    
        public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.Attack();
        }

        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                         int      layerIndex)
        {
            
        }
    
        public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.SetUpdate(true);
        }
    
        public override void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                           int      layerIndex)
        {
            
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
         
        }
    }
}
