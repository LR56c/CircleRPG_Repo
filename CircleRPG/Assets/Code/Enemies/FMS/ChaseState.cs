using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public class ChaseState : SceneLinkedSMB<EnemyBaseBehaviour>
{
    public override void OnStart(Animator animator)
    {
        Debug.Log($"OnStart.{nameof(ChaseState)}");
    }
    
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStateEnter.{nameof(ChaseState)}");
    }
    
    public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStatePostEnter.{nameof(ChaseState)}");
    }
    
    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                     int      layerIndex)
    {
        Debug.Log($"OnSLStateNoTransitionUpdate.{nameof(ChaseState)}");
    }

    public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStatePreExit.{nameof(ChaseState)}");
    }
    
    public override void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                       int      layerIndex)
    {
        Debug.Log($"OnSLTransitionFromStateUpdate.{nameof(ChaseState)}");
    }
    
    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStateExit.{nameof(ChaseState)}");
    }
}
