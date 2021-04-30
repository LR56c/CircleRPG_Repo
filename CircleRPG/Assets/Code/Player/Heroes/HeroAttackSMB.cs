using Code.Player.Heroes;
using Code.Utility;
using UnityEngine;

namespace Code.Enemies.SMB2
{
    public class HeroAttackSMB : MySceneLinkedSMB<HeroBaseBehaviour>
    {
        public override void OnStart(Animator animator)
        {
            
        }

        public override void OnSLStateEnter(Animator          animator,
                                            AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public override void OnSLStateUpdate(Animator          animator,
                                             AnimatorStateInfo stateInfo, int layerIndex)
        {
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo,
                                           int      layerIndex)
        {
            m_MonoBehaviour.OnAttackComplete?.Invoke();
        }
    }
}