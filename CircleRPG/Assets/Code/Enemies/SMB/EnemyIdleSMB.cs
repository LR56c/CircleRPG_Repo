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

        /*
         * idleSMB
         * agregar animator param Stun, que provocaria la habilidad de martillo,
         * desactivando attack & sight
         * y luego de (seconds) desactivaria stun
         */

        /*
         * hab arquero fuego?
         * activa triggerStay que cubre toda la parte superior del piso de la zona,
         * quitando vida como si se atacara 3 veces los 3 heroes (=9)
         */
    }
}

