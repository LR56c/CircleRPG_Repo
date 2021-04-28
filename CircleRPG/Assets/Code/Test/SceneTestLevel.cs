using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Test
{
    public class SceneTestLevel : MonoBehaviour
    {
        [SerializeField] private SceneTest _sceneTest;
    
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log($"sceneTestLevel.bulletTest: {_sceneTest.bulletTest.ToString()}");
                _sceneTest.bulletTest = 10;
                Debug.Log($"sceneTestLevel.bulletTest changed: {_sceneTest.bulletTest.ToString()}");
            }
        
            if(Input.GetKeyDown(KeyCode.F2))
            {
                SceneManager.LoadSceneAsync(1);
            }
        }
    }
}
