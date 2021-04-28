using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Test
{
    public class SceneTestHub : MonoBehaviour
    {
        [SerializeField] private SceneTest _sceneTest;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.Log($"sceneTestHub.bulletTest: {_sceneTest.bulletTest.ToString()}");
                _sceneTest.bulletTest = 5;
                Debug.Log($"sceneTestHub.bulletTest changed: {_sceneTest.bulletTest.ToString()}");
            }

            if(Input.GetKeyDown(KeyCode.F1))
            {
                SceneManager.LoadSceneAsync(2);
            }
        }
    }
}
