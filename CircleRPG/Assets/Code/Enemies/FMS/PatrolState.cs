using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;
using UnityEngine.Animations;

public class PatrolState : SceneLinkedSMB<EnemyBaseBehaviour>
{
    public override void OnStart(Animator animator)
    {
        Debug.Log($"OnStart.{nameof(PatrolState)}");
    }

    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStateEnter.{nameof(PatrolState)}");
    }

    public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStatePostEnter.{nameof(PatrolState)}");
    }

    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                     int      layerIndex)
    {
        Debug.Log($"OnSLStateNoTransitionUpdate.{nameof(PatrolState)}");
    }
    
    public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStatePreExit.{nameof(PatrolState)}");
    }
    
    public override void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                       int      layerIndex)
    {
        Debug.Log($"OnSLTransitionFromStateUpdate.{nameof(PatrolState)}");
    }
    
    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStateExit.{nameof(PatrolState)}");
    }
}
