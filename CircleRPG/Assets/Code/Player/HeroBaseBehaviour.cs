using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Code.Utility;
using UnityEngine;
using UnityEngine.AI;

public abstract class HeroBaseBehaviour : MonoBehaviour
{
    private                              NavMeshAgent _agent;
    [SerializeField]             private GameObject   _point;
    
    protected virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        RegisterHero();
    }

    protected abstract void RegisterHero();

    protected virtual void Update()
    {
        _agent.SetDestination(_point.transform.position);
    }
}
