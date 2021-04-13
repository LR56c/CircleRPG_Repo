using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class THMSV_EditorSceneLoader : EditorWindow
{
    [MenuItem("THMSV/Scene Loader")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = (THMSV_EditorSceneLoader) GetWindow(typeof(THMSV_EditorSceneLoader));
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("HOME", GUILayout.Width(60), GUILayout.Height(25)))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/_THMSV/RPGBuilder/Scenes/MainMenu.unity");
        }

        if (GUILayout.Button("DEMO", GUILayout.Width(60), GUILayout.Height(25)))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/_THMSV/RPGBuilder/Scenes/DEMO1.unity");
        }

        if (GUILayout.Button("ESSL", GUILayout.Width(60), GUILayout.Height(25)))
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/_THMSV/RPGBuilder/Scenes/UI_Scene.unity");
        }

        GUILayout.EndHorizontal();
    }
}