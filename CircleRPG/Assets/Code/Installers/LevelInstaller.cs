using System;
using Code.Utility;
using Rewired.ComponentControls;
using UnityEngine;

namespace Code.Installers
{
    public class LevelInstaller : MonoBehaviour
    {
        [SerializeField] private TouchJoystick _touchJoystick;
        
        private void Awake()
        {
            ServiceLocator.Instance.RegisterService(_touchJoystick);
        }

        private void OnApplicationQuit()
        {
            //save service locator datas
        }
    }
}