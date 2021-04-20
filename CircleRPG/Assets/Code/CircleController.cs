using System;
using Rewired;
using Rewired.ComponentControls;
using Rewired.Demos;
using UnityEngine;

namespace Code
{
    public class CircleController : MonoBehaviour
    {
        [SerializeField] private float         _speed = 5f;
        [SerializeField] private TouchJoystick _touchJoystick;
        private                  Rigidbody     _rb;

        private void Awake()
        {
            //_touchJoystick = GetComponent<TouchJoystick>();
            _rb = GetComponent<Rigidbody>();
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
            Vector3 vector = _speed * Time.deltaTime * new Vector3(position.x, 0f, position.y);
            
            //transform.Translate(v);
            _rb.velocity += vector;
        }
        
        /*private void Update()
        {
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");

            Vector3 v = _speed * Time.deltaTime * new Vector3(x, 0f, z);
            
            transform.Translate(v);
        }*/
    }
}