using UnityEngine;

namespace Code.Utility
{
    public class MySceneLinkedSMB<TMonoBehaviour> : StateMachineBehaviour
        where TMonoBehaviour : MonoBehaviour
    {
        protected                  TMonoBehaviour m_MonoBehaviour;
        protected                  Animator       _animator;

        public static void Initialise(Animator animator, TMonoBehaviour monoBehaviour)
        {
            MySceneLinkedSMB<TMonoBehaviour>[] sceneLinkedSMBs =
                animator.GetBehaviours<MySceneLinkedSMB<TMonoBehaviour>>();

            for(int i = 0; i < sceneLinkedSMBs.Length; i++)
            {
                sceneLinkedSMBs[i].InternalInitialise(animator, monoBehaviour);
            }
        }

        protected void InternalInitialise(Animator animator, TMonoBehaviour monoBehaviour)
        {
            m_MonoBehaviour = monoBehaviour;
            _animator = animator;
            OnStart(animator);
        }

        //Unity SMB

        public sealed override void OnStateEnter(Animator          animator,
                                                 AnimatorStateInfo stateInfo,
                                                 int               layerIndex)
        {
            OnSLStateEnter(animator, stateInfo, layerIndex);
        }

        public sealed override void OnStateExit(Animator          animator,
                                                AnimatorStateInfo stateInfo,
                                                int               layerIndex)
        {
            OnSLStateExit(animator, stateInfo, layerIndex);
        }

        public sealed override void OnStateUpdate(Animator          animator,
                                                  AnimatorStateInfo stateInfo,
                                                  int               layerIndex)
        {
            OnSLStateUpdate(animator, stateInfo, layerIndex);
        }

        //Virtuals

        public virtual void OnStart(Animator animator) {}

        public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo,
                                           int      layerIndex)
        {
        }

        public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo,
                                          int      layerIndex)
        {
        }

        public virtual void OnSLStateUpdate(Animator          animator,
                                            AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}