using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Content.Global
{
    public class GameManager : MonoBehaviour
    {
        public UnityEvent sceneLoadStartEvent;
        public UnityEvent sceneLoadCompleteEvent;
        
        public static GameManager Instance;

        [NonSerialized]
        public float SceneLoadProgress;
        
        public string mainSceneName;
        public string loginSceneName;

        private void Awake()
        {
            //Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.Log("Instance already exists");
                Destroy(this);
            }
            
            UnityEngine.Object.DontDestroyOnLoad(Instance);
        }
        
        public void ChangeScene(string sceneName)
        {
            StartCoroutine(LoadScene(sceneName));
        }

        IEnumerator LoadScene(string sceneName)
        {
            sceneLoadStartEvent.Invoke();
            var operation = SceneManager.LoadSceneAsync(sceneName);
            while (!operation.isDone)
            {
                SceneLoadProgress = operation.progress;
                yield return null;
            }
            sceneLoadCompleteEvent.Invoke();
        }
    }
}
