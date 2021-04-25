using System.Collections;
using System.Collections.Generic;
using Code;
using UnityEngine;

public class AttackState : SceneLinkedSMB<EnemyBaseBehaviour>
{
    public override void OnStart(Animator animator)
    {
        Debug.Log($"OnStart.{nameof(AttackState)}");
    }

    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStateEnter.{nameof(AttackState)}");
    }
    
    public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStatePostEnter.{nameof(AttackState)}");
    }

    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                     int      layerIndex)
    {
        Debug.Log($"OnSLStateNoTransitionUpdate.{nameof(AttackState)}");
    }
    
    public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStatePreExit.{nameof(AttackState)}");
    }
    
    public override void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
                                                       int      layerIndex)
    {
        Debug.Log($"OnSLTransitionFromStateUpdate.{nameof(AttackState)}");
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log($"OnSLStateExit.{nameof(AttackState)}");
    }
}
