using Code.Enemies.Types;
using Code.Utility;
using DG.Tweening;
using UnityEngine;

namespace Code.Enemies.SMB
{
    public class EnemyAimSMB : MySceneLinkedSMB<EnemyBaseBehaviour>
    {
        private                  int   _aimParam = Animator.StringToHash("ToAim");
        [SerializeField] private float _secondsAiming = 5.0f;
        private float _secondsInitialSmoothTurn = 0.2f;
        private                  bool  bForceLookAt = false;

        [SerializeField] private GameObject _aimRay;
        
        
        public override void OnStart(Animator animator)
        {
        }

        public override void OnSLStateEnter(Animator          animator,
                                            AnimatorStateInfo stateInfo, int layerIndex)
        {
            _aimRay.SetActive(true);
            _secondsInitialSmoothTurn = m_MonoBehaviour.TweenTimeRotate;
            m_MonoBehaviour.TurnToTarget(m_MonoBehaviour.GetHero().transform.position);
            DOVirtual.DelayedCall(_secondsInitialSmoothTurn, () =>
            {
                bForceLookAt = true;
            });
            DOVirtual.DelayedCall(_secondsAiming, () => {animator.SetTrigger(_aimParam);});
        }

        public override void OnSLStateUpdate(Animator          animator,
                                             AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!bForceLookAt) return;
            m_MonoBehaviour.transform.LookAt(m_MonoBehaviour.GetHero().transform
                                                 .position);
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo,
                                           int      layerIndex)
        {
            _aimRay.SetActive(false);
            bForceLookAt = false;
        }
        
        public void SetSpecificAimRay(GameObject aimRay)
        {
            _aimRay = aimRay;
        }
    }
}