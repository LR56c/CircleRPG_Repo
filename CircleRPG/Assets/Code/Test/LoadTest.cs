using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Test
{
    public class LoadTest : Singleton<LoadTest>
    {
        //private List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
        {
            Debug.Log("OnSceneLoaded");
        }

        private void Start()
        {
            Debug.Log("start");
        }

        /* orden
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        [RuntimeInitializeOnLoadMethod]
        */

        /*public IEnumerator SceneLoadProgress()
        {
            foreach(var operation in _scenesLoading)
            {
                while(!operation.isDone)
                {
                    yield return null;
                }
            }
        }*/
    }
}