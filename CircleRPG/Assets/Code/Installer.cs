using System;
using System.Collections.Generic;
using Code.Utility;
using Lean.Touch;
using Rewired;
using Rewired.ComponentControls;
using Rewired.Demos;
using UnityEngine;
using UnityEngine.UI;
using static System.Single;

namespace Code
{
    public class Installer : MonoBehaviour
    {
        [SerializeField] private TouchJoystick _touchJoystick;
        
        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(_touchJoystick);
        }
    }
}