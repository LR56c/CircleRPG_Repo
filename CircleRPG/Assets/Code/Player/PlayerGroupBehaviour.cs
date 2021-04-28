using Code.Utility;
using Lean.Touch;
using Rewired.ComponentControls;
using UnityEngine;

namespace Code.Player
{
    public class PlayerGroupBehaviour : MonoBehaviour
    {
        [SerializeField] private float         _speed = 5f;
        private TouchJoystick _touchJoystick;
        private                  Rigidbody     _rb;
        private                   Vector3       _joystickValue;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _touchJoystick = ServiceLocator.Instance.GetService<TouchJoystick>();
            _touchJoystick.ValueChangedEvent += OnValueChangedEvent;
        }

        private void OnDisable()
        {
            _touchJoystick.ValueChangedEvent -= OnValueChangedEvent;
        }

        private void OnValueChangedEvent(Vector2 position)
        {
            _joystickValue = new Vector3(position.x, 0f, position.y);
        }
        
        private void Update()
        {
            Vector3 vector = _speed * Time.deltaTime * _joystickValue;
            _rb.velocity += vector;

            //Debug.Log($"count: {LeanTouch.Fingers.Count.ToString()}");
            _touchJoystick.gameObject.SetActive(LeanTouch.Fingers.Count != 3);
        }
    }
}