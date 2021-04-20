using System;
using System.Collections.Generic;
using Lean.Touch;
using Rewired;
using Rewired.ComponentControls;
using Rewired.Demos;
using UnityEngine;
using UnityEngine.UI;
using static System.Single;

namespace Code
{
    public class TestStick : MonoBehaviour
    {
        [SerializeField] private TouchJoystick _touchJoystick;
        
        private void Update()
        {
            if(LeanTouch.Fingers.Count == 2)
            {
                _touchJoystick.gameObject.SetActive(false);
            }

            if(LeanTouch.Fingers.Count == 1)
            {
                _touchJoystick.gameObject.SetActive(true);
            }
        }
    }
}