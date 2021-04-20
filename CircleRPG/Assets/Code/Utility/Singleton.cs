using UnityEngine;

namespace Code.Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool _applicationIsQuitting = false;
        private static T    _instance;
        public static T Instance
        {
            get
            {
                if(_applicationIsQuitting)
                {
                    Debug.LogWarning($"Singleton<{typeof(T)}> already destroyed on application quit");
                    return null;
                }
                
                if(_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if(_instance == null)
                    {
                        GameObject gameObject = new GameObject($"{typeof(T)}");
                        _instance = gameObject.AddComponent<T>();
                        DontDestroyOnLoad(gameObject);
                        Debug.Log($"Singleton<{typeof(T)}> is needed in the scene, so {gameObject.name} was created with DontDestroyOnLoad");
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if(_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogError($"Singleton<{typeof(T)}> A instance already exists");
                Destroy(gameObject);
            }
        }
        
        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
        
        public static bool Exists => _instance != null;
    }
}