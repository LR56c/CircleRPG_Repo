using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObstacleSaw : MonoBehaviour
{
    [SerializeField] private float          _duration = 3.0f;
    //[SerializeField] private AnimationCurve _curve;
    [SerializeField] private Transform      _pointB;
    [SerializeField] private Vector3      _pointA;

    private void OnEnable()
    {
        _pointA = transform.position;
        
        var mySecuence = DOTween.Sequence();
        
        mySecuence.Append(transform.DOMove(_pointB.position, _duration).SetEase(Ease.Linear))
                  .Append(transform.DOMove(_pointA,  _duration).SetEase(Ease.Linear))
                  .SetLoops(-1, LoopType.Restart);
    }
}
