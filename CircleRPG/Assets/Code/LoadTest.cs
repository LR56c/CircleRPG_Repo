using System;
using System.Collections;
using System.Collections.Generic;
using Code.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    public class LoadTest : Singleton<LoadTest>
    {
        //private List<AsyncOperation> _scenesLoading = new List<AsyncOperation>();
        
        /*protected override void Awake()
        {
            base.Awake();
            Debug.Log("awake");

            if(PlayerPrefs.HasKey("first"))
            {
                Debug.Log("have first");
            }
            else
            {
                Debug.Log("no first");
                PlayerPrefs.SetInt("first", 1);
                SceneManager.LoadSceneAsync(1);
            }
        }

        private void OnEnable()
        {
            Debug.Log("onEnable");
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

        [RuntimeInitializeOnLoadMethod]
        public static void NormalRuntime()
        {
            Debug.Log("NormalRuntime");
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void SubsystemRegistration()
        {
            Debug.Log("SubsystemRegistration");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void AfterAssembliesLoaded()
        {
            Debug.Log("AfterAssembliesLoaded");
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void AfterSceneLoad()
        {
            Debug.Log("AfterSceneLoad");
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void BeforeSceneLoad()
        {
            Debug.Log("BeforeSceneLoad");
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        public static void BeforeSplashScreen()
        {
            Debug.Log("BeforeSplashScreen");
        }*/
        

        /*private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("Scene Loaded");
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }*/
        
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