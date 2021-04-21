using System;
using Code.Utility;
using Lean.Touch;
using Rewired;
using Rewired.ComponentControls;
using Rewired.Demos;
using UnityEngine;

namespace Code
{
    public class CircleController : MonoBehaviour
    {
        [SerializeField] private float         _speed = 5f;
        private TouchJoystick _touchJoystick;
        private                  Rigidbody     _rb;
        public                   Vector3       JoystickValue;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _touchJoystick = ServiceLocator.Instance.GetService<TouchJoystick>();
        }
        
        private void OnEnable()
        {
            
            _touchJoystick.ValueChangedEvent += OnValueChangedEvent;
        }
        
        private void OnDisable()
        {
            _touchJoystick.ValueChangedEvent -= OnValueChangedEvent;
        }

        private void OnValueChangedEvent(Vector2 position)
        {
            JoystickValue = new Vector3(position.x, 0f, position.y);
        }
        
        private void Update()
        {
            Vector3 vector = _speed * Time.deltaTime * JoystickValue;
            _rb.velocity += vector;

            Debug.Log($"count: {LeanTouch.Fingers.Count.ToString()}");
            _touchJoystick.gameObject.SetActive(LeanTouch.Fingers.Count != 3);
        }
    }
}