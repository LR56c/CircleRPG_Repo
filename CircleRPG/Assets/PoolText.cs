using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FredericRP.ObjectPooling;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PoolText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Animator        _animator;
    private                  int             _randParam = Animator.StringToHash("Rand");
    private                  MyObjectPoolText      _pool;
    private                  int             _rand;
    
    private void OnDisable()
    {
        _rand = Random.Range(0, 6);
    }

    public void SetText(string text, Color color)
    {
        _animator.SetInteger(_randParam, _rand);
        _text.DOColor(color, 0.1f);
        _text.SetText(text);
    }

    private void DisableAnimator()
    {
        _pool.Pool(this);
    }

    public void Config(MyObjectPoolText pool)
    {
        _pool = pool;
    }
}

