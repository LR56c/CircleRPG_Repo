using Code.Utility;
using DG.Tweening;
using UnityEngine;

namespace Code.Enemies.FMS
{
    public class EnemyIdleSMB : EnemyLinkedSMB<EnemyBaseBehaviour>
    {
        private int _idleParam   = Animator.StringToHash("Idle");
        public  int _stopUpdateParam = Animator.StringToHash("bStopUpdate");

        [SerializeField] private float    _secondsToWait = 10.0f;
        [SerializeField] private bool     bWait          = false;
        
        public override void OnStart(Animator animator)
        {
        }
        
        public override void OnSLStateEnter(Animator  animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ConfigState(true);
        }   
        
        public override void OnSLStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(bWait) return;
            bWait = true;
            DOVirtual.DelayedCall(_secondsToWait, () => ConfigState(false));   
        }
        
        public override void OnSLStateExit(Animator   animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bWait = false;
        }

        //TODO: puede ser abstract o interface
        private void ConfigState(bool value)
        {
            _animator.SetBool(_idleParam,       value);
            _animator.SetBool(_stopUpdateParam, value);
        }
    }
}