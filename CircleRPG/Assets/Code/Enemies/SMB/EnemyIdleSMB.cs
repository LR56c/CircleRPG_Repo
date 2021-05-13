using Code.Enemies.Types;
using Code.Utility;
using DG.Tweening;
using UnityEngine;

namespace Code.Enemies.SMB
{
    public class EnemyIdleSMB : MySceneLinkedSMB<EnemyBaseBehaviour>
    {
        private                  int   _stunParam = Animator.StringToHash("Stuned");
        [SerializeField] private float _seconds = 5.0f;
        private Vector3 _cachedPos = Vector3.zero;

        public override void OnStart(Animator animator)
        {
        }

        public override void OnSLStateEnter(Animator          animator,
                                            AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(!animator.GetBool(_stunParam)) return;

            //TODO: aqui pasar stunFX_go y desactivar en delayedCall
            _cachedPos = m_MonoBehaviour.transform.position;
            DOVirtual.DelayedCall(_seconds, () =>
            {
                animator.SetBool(_stunParam, false);
            });
        }

        public override void OnSLStateUpdate(Animator          animator,
                                             AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(animator.GetBool(_stunParam))
            {
                m_MonoBehaviour.transform.position = _cachedPos;
            }
        }

        public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo,
                                           int      layerIndex)
        {
        }
    }
}

