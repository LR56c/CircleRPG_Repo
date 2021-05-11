using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LayoutCell : MonoBehaviour
{
    [SerializeField]             private Button          _thisButton;
    [SerializeField]             private LayoutElement   _layoutElement;
    [SerializeField]             private TextMeshProUGUI _childText;
    [SerializeField] private RectTransform               _thisRectTransform;

    private int _min = 100;
    private int _max = 150;
    
    public event Action<LayoutCell> OnPressed;
    public Vector2                  RectPosition => _thisRectTransform.anchoredPosition;

    private void OnValidate()
    {
        if(!_thisRectTransform)
            _thisRectTransform = GetComponent<RectTransform>();
        
        if(!_layoutElement)
            _layoutElement = GetComponent<LayoutElement>();
        
        if(!_childText)
            _childText = GetComponentInChildren<TextMeshProUGUI>();
        
        if(!_thisButton)
            _thisButton = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _thisButton.onClick.AddListener(Call);
    }

    private void OnDisable()
    {
        _thisButton.onClick.RemoveListener(Call);
    }

    private void Call()
    {
        Sum();
        OnPressed?.Invoke(this);
    }

    public void Sum()
    {
        _childText.fontSize *= 2;
        _layoutElement.minWidth = _max;
    }

    public void Small()
    {
        _layoutElement.minWidth = _min;
        _childText.fontSize /= 2;
    }
}
