using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSlideMediator : MonoBehaviour
{
    private                  LeanSnap      _leanSnap;
    [SerializeField] private LeanSwitch    _buttonBarSwitch;
    [SerializeField] private RectTransform[]     _initialRectPositions;

    private void Awake()
    {
        _leanSnap = GetComponent<LeanSnap>();
    }

    private void OnChangedState(int state)
    {
       UpdateActualScreenFromState(state);
    }

    private void UpdateActualScreenFromState(int state)
    {
        TweenerCore<Vector3, Vector3, VectorOptions> tween = null;
        _leanSnap.enabled = false;

        tween = state switch
        {
            0 => transform.DOMove(_initialRectPositions[0].position, 0.2f),
            1 => transform.DOMove(_initialRectPositions[1].position, 0.2f),
            2 => transform.DOMove(_initialRectPositions[2].position, 0.2f),
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

        tween?.OnComplete(() =>
        {
            _leanSnap.enabled = true;
        });
    }

    private void OnEnable()
    {
        _leanSnap.OnPositionChanged.AddListener(OnPositionChanged);
        _buttonBarSwitch.OnChangedState.AddListener(OnChangedState);
    }

    private void OnDisable()
    {
        _leanSnap.OnPositionChanged.RemoveListener(OnPositionChanged);
        _buttonBarSwitch.OnChangedState.RemoveListener(OnChangedState);
    }
    
    private void OnPositionChanged(Vector2Int value)
    {
        UpdateButtonBarState(value);
    }

    private void UpdateButtonBarState(Vector2Int value)
    {
        var xTemp = value.x;
        
        _buttonBarSwitch.State = xTemp switch
        {
            0  => 0,
            -1 => 1,
            -2 => 2,
            _  => throw new ArgumentOutOfRangeException(nameof(xTemp), xTemp, null)
        };
    }
}