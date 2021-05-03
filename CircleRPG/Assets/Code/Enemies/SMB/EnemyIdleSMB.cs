using Code.Enemies.Types;
using Code.Utility;
using UnityEngine;

namespace Code.Enemies.SMB
{
    public class EnemyIdleSMB : MySceneLinkedSMB<EnemyBaseBehaviour>
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
           
        }
        
        /*
         * idleSMB
         * agregar animator param Stun, que provocaria la habilidad de martillo,
         * desactivando attack & sight
         * y luego de (seconds) desactivaria stun
         */
        
        //TODO: falta marcar linea vision de aim state crossbowman
        
        /*
         * hab arquero fuego?
         * activa triggerStay que cubre toda la parte superior del piso de la zona,
         * quitando vida como si se atacara 3 veces los 3 heroes (=9)
         */
    }
}