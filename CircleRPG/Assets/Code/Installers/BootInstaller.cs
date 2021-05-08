using Code.UI;
using Code.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Installers
{
    public class BootInstaller : MonoBehaviour
    {
        [SerializeField] private UILoader _uiLoader;
        private                  World    _firstWorld;

        private void Awake()
        {
            var killedEnemyService = new KilledEnemyService();
            ServiceLocator.Instance.RegisterService(killedEnemyService);
            
            if(PlayerPrefs.HasKey("Level"))
            {
                var savedLevel = PlayerPrefs.GetInt("Level");
                _firstWorld = new World(savedLevel);
                //mas scene offset
                _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync(savedLevel + 2));
            }
            else
            {
                _firstWorld = new World(1);
                //se carga escena tutorial, ya que ahi recien se guardara Level
                _uiLoader.LoadSceneAsync(SceneManager.LoadSceneAsync(2));
            }
            
            ServiceLocator.Instance.RegisterService(_firstWorld);
        }
    }
}