using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public class IdleState : SceneLinkedSMB<EnemyBaseBehaviour>
{
    public override void OnStart(Animator animator)
    {
        Debug.Log($"OnStart.{nameof(IdleState)}");
    }

    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStateEnter.{nameof(IdleState)}");
    }

    public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStatePostEnter.{nameof(IdleState)}");
    }
    
    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                     int      layerIndex)
    {
        Debug.Log($"OnSLStateNoTransitionUpdate.{nameof(IdleState)}");
    }
    
    public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStatePreExit.{nameof(IdleState)}");
    }
    
    public override void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                       int      layerIndex)
    {
        Debug.Log($"OnSLTransitionFromStateUpdate.{nameof(IdleState)}");
    }
    
    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStateExit.{nameof(IdleState)}");
    }
}
