using System;
using System.Collections;
using UnityEngine;

namespace THMSV.RPGBuilder.CombatVisuals
{
    public class CombatVisualAnimation : MonoBehaviour
    {
        public enum ANIMATION_TYPE
        {
            TRIGGER,
            BOOL
        }
        [SerializeField] private ANIMATION_TYPE animType;
        [SerializeField] private string AnimationName;
        [SerializeField] private float activationDelay;
        [SerializeField] private float thisDuration;
        [SerializeField] private float animDuration;

        private GameObject nodeGO;
        private RPGAbility thisAb;
        public RPGAbility ThisAB 
        {
            get => thisAb;
            set => thisAb = value;
        }

        public void InitCombatAnimation(GameObject node, RPGAbility ab)
        {
            if (thisDuration != 0 && thisDuration != -1) Destroy(gameObject, thisDuration);

            nodeGO = node;
            ThisAB = ab;
            StartCoroutine(ExecuteCombatAnimation());
        }

        private IEnumerator ExecuteCombatAnimation()
        {
            yield return new WaitForSeconds(activationDelay);
            if(nodeGO == null) yield break;
            var nodeAnim = nodeGO.GetComponent<Animator>();
            if (nodeAnim == null) yield break;
        
            switch (animType)
            {
                case ANIMATION_TYPE.TRIGGER:
                    nodeAnim.SetTrigger(AnimationName);
                    break;
                case ANIMATION_TYPE.BOOL:
                    nodeAnim.SetBool(AnimationName, true);
                    yield return new WaitForSeconds(animDuration);
                    nodeAnim.SetBool(AnimationName, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
