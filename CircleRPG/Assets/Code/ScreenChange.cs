using System;
using Lean.Gui;
using UnityEngine;

public class ScreenChange : MonoBehaviour
{
    private                  LeanSnap   _leanSnap;
    [SerializeField] private LeanSwitch _leanSwitch;
    [SerializeField] private Vector3[]  _initialRectPositions;

    private void Awake()
    {
        _leanSnap = GetComponent<LeanSnap>();
    }

    private void OnChangedState(int state)
    {
        //TODO: agregar tween & ver si es mejor mediator

        transform.position = state switch
        {
            0 => _initialRectPositions[0],
            1 => _initialRectPositions[1],
            2 => _initialRectPositions[2],
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    private void OnEnable()
    {
        _leanSnap.OnPositionChanged.AddListener(Call);
        _leanSwitch.OnChangedState.AddListener(OnChangedState);
    }

    private void OnDisable()
    {
        _leanSnap.OnPositionChanged.RemoveListener(Call);
        _leanSwitch.OnChangedState.RemoveListener(OnChangedState);
    }
    
    private void Call(Vector2Int arg0)
    {
        var x = arg0.x;

        _leanSwitch.State = x switch
        {
            0 => 0,
            -1 => 1,
            -2 => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
        };
    }
}