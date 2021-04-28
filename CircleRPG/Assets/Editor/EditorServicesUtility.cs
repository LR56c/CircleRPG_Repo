using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorServicesUtility
{
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Run()
    {
        GameObject bootCanvas = Object.Instantiate(Resources.Load("BootCanvasEditorOnly")) as GameObject;
        Debug.Log("EditorServicesUtility is active");
    }
#endif
}