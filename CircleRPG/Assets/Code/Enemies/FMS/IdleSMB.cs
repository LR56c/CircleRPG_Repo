using System.Collections;
using Code.Domain.Interfaces;
using Code.Utility;
using DG.Tweening;
using UnityEngine;

namespace Code.Enemies.FMS
{
    public class IdleSMB : SceneLinkedSMB<EnemyBaseBehaviour>
    {
        private WaitForSeconds _coroutine;
        private int            _idleParam   = Animator.StringToHash("Idle");
        [SerializeField] private float _secondsToWait = 5f;

        public override void OnStart(Animator animator)
        {
            _coroutine = new WaitForSeconds(_secondsToWait);
        }

        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            m_MonoBehaviour.SetUpdate(false);
        }

        public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }
    
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                         int      layerIndex)
        {
            if(bAction) return;
            
            //m_MonoBehaviour.StartCoroutine(Execute());
            DOVirtual.DelayedCall(_secondsToWait, () =>
            {
                _thisAnimator.SetBool(_idleParam, false);
            });
            bAction = true;
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
            bAction = false;
        }
    }
}
