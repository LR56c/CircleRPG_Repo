using UnityEngine;
using UnityEngine.UI;

public class BillboardBehaviour : MonoBehaviour
{
    [SerializeField]             private RectTransform _rectTransform;
    
    private                  Camera        _camera;
    
    private void Start()
    {
        _camera = Camera.main;
    }

    private void LateUpdate()
    {
        _rectTransform.rotation = _camera.transform.rotation;
    }
}
