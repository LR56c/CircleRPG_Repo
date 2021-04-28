using Code.UI;
using Code.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Installers
{
    public class BootInstaller : MonoBehaviour
    {
        [SerializeField] private UILoader _uiLoader;

        private void Awake()
        {
            //aqui en caso de necesitar cargar algo guardado de los servicios

            var teamStats = new TeamStats();
            ServiceLocator.Instance.RegisterService(teamStats);

            var world = new World();
            ServiceLocator.Instance.RegisterService(world);

            var playerStats = new PlayerStats();
            ServiceLocator.Instance.RegisterService(playerStats);
        }

        private void Start()
        {
            /*
             * TODO: ver si pasa al tutorial
             * dependiendo de esto retrasar carga % UILoader
             */

            _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync(1));

            /*if(PlayerPrefs.HasKey("FirstPlay"))
            {
                SceneManager.LoadSceneAsync(UnityConstants.Scenes.Mauricio_Hub);
            }
            else
            {
                PlayerPrefs.SetInt("FirstPlay", 1);
                SceneManager.LoadSceneAsync(UnityConstants.Scenes.Mauricio_LevelTest);
            }*/
        }
    }
}