using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.UI
{
    public class LoadScene : MonoBehaviour
    {
        public string TargetSceneName;
        public AsyncOperation loadOperation;
        public float loadProgress;

        public void Load()
        {
            loadOperation = SceneManager.LoadSceneAsync(TargetSceneName);
        
            StartCoroutine(LoadCoroutine());
        }

        IEnumerator LoadCoroutine()
        {
            while (!loadOperation.isDone)
            {
                loadProgress = loadOperation.progress;
            }

            yield return null;
        }
    }
}
