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

            var killedEnemyService = new KilledEnemyService();
            ServiceLocator.Instance.RegisterService(killedEnemyService);
        }

        private void OnApplicationQuit()
        {
            //TODO: save service locator datas
        }
    }
}