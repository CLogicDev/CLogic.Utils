using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace CLogic.Utils.UI
{
    public class SceneLoader : MonoBehaviour
    {
        public GraphicsFader graphicsFader;

        public float startDelay;
        public float fadeInDuration = 1f;
        public float fadeOutDuration = 1f;
        
        public bool IsLoading {get; private set;}
        public Action OnLoadingStart;
        public Action OnLoadingComplete;
        public Action<float> OnLoadingProgress;

        private void Awake()
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.I))
                LoadSceneAsync(1);
        }

        private Coroutine loadingCoroutine;
        
        public void LoadSceneAsync(string sceneName) => LoadSceneAsync(sceneName, new LoadSceneParameters(LoadSceneMode.Single));
        public void LoadSceneAsync(string sceneName, LoadSceneParameters parameters) => LoadSceneAsync(SceneManager.GetSceneByName(sceneName), parameters);
        
        public void LoadSceneAsync(int buildIndex) => LoadSceneAsync(buildIndex, new LoadSceneParameters(LoadSceneMode.Single));
        public void LoadSceneAsync(int buildIndex, LoadSceneParameters parameters) => LoadSceneAsync(SceneManager.GetSceneByBuildIndex(buildIndex), parameters);
        
        public void LoadSceneAsync(Scene scene, LoadSceneParameters parameters)
        {
            if(IsLoading)
                return;
            
            IsLoading = true;
            loadingCoroutine = StartCoroutine(LoadSceneCoroutine(scene, parameters));
        }

        private IEnumerator LoadSceneCoroutine(Scene scene, LoadSceneParameters parameters)
        {
            OnLoadingStart?.Invoke();

            float fadeInProgress = 0;
            while (fadeInProgress < fadeInDuration)
            {
                fadeInProgress += Time.deltaTime;
                graphicsFader.SetAlpha(Mathf.Lerp(0, 1, fadeInProgress / fadeInDuration));
                yield return null;
            }
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.buildIndex, parameters);
            
            while (operation.progress <= 0.9)
            {
                OnLoadingProgress?.Invoke(operation.progress);
                yield return null;
            }
            
            
            
            OnLoadingComplete?.Invoke();
        }
    }
}
