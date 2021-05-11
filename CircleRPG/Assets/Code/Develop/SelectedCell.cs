using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCell : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
    
    [SerializeField]             private LayoutCell[] _buttons;
    [SerializeField] private RectTransform            _thisRectTransform;

    [SerializeField] private LayoutCell _currentCell;
    [SerializeField] private LayoutCell _previousCell;
    
    private void OnValidate()
    {
        if(_buttons.Length == 0)
            _buttons = _horizontalLayoutGroup.GetComponentsInChildren<LayoutCell>();
        
        if(!_thisRectTransform)
            _thisRectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        foreach(var button in _buttons)
        {
            button.OnPressed += ButtonOnPressed;
        }
    }

    private void OnDisable()
    {
        foreach(var button in _buttons)
        {
            button.OnPressed -= ButtonOnPressed;
        }
    }

    private void Start()
    {
        _currentCell = _buttons[1];
        _currentCell.Sum();
    }

    private void ButtonOnPressed(LayoutCell obj)
    {
        //se podria hacer mejor deteccion layoutCell para que se achique el mismo
        _previousCell = _currentCell;
        _previousCell.Small();
        _currentCell = obj;
        DOVirtual.DelayedCall(0.3f, () =>
        {
            _thisRectTransform.DOAnchorPosX(obj.RectPosition.x, 0.3f);
        });
    }
}
