using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace CLogic.Utils.UI
{
    public class SceneLoader : MonoBehaviour
    {
        private struct Loader
        {
            public int? buildIndex;
            public string sceneName;

            public LoadSceneParameters sceneParameters;
            
            public Loader(int buildIndex, LoadSceneParameters sceneParameters)
            {
                this.buildIndex = buildIndex;
                this.sceneParameters = sceneParameters;
                sceneName = null;
            }

            public Loader(string sceneName, LoadSceneParameters sceneParameters)
            {
                this.sceneName = sceneName;
                this.sceneParameters = sceneParameters;
                buildIndex = null;
            }
            
            public AsyncOperation LoadAsync()
            {
                return buildIndex.HasValue ? SceneManager.LoadSceneAsync(buildIndex.Value, sceneParameters) : SceneManager.LoadSceneAsync(sceneName, sceneParameters);
            }
        }
        
        public GraphicsFader graphicsFader;
        public Slider loadingBar;
        
        public Countdown fadeIn;
        public Countdown fadeOut;
        public Countdown loadingBarDisplay;
        public Countdown loadDelay;
        
        public bool IsLoading {get; private set;}
        public Action OnLoadingStart;
        public Action OnLoadingComplete;
        public Action<float> OnLoadingProgress;

        private void Awake()
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }

        private Coroutine loadingCoroutine;

        public void LoadSceneAsync(string sceneName) => LoadSceneAsync(sceneName, new LoadSceneParameters(LoadSceneMode.Single));
        public void LoadSceneAsync(string sceneName, LoadSceneParameters sceneParameters) => LoadSceneAsync(new Loader(sceneName, sceneParameters));
        
        public void LoadSceneAsync(int buildIndex) => LoadSceneAsync(buildIndex, new LoadSceneParameters(LoadSceneMode.Single));
        public void LoadSceneAsync(int buildIndex, LoadSceneParameters sceneParameters) => LoadSceneAsync(new Loader(buildIndex, sceneParameters));

        private void LoadSceneAsync(Loader loader)
        {
            if(IsLoading)
                return;
            
            IsLoading = true;
            loadingCoroutine = StartCoroutine(LoadSceneCoroutine(loader));
        }

        private IEnumerator LoadSceneCoroutine(Loader loader)
        {
            OnLoadingStart?.Invoke();

            fadeIn.Start();
            graphicsFader.gameObject.SetActive(true);
            while (!fadeIn.IsFinished)
            {
                fadeIn.Tick(Time.deltaTime);
                graphicsFader.SetAlpha(fadeIn.PercentageCompletion);
                yield return null;
            }
            
            loadingBarDisplay.Start();

            Action showLoadingBar = null;
            showLoadingBar = () =>
            {
                loadingBar.gameObject.SetActive(true);
                loadingBarDisplay.OnComplete -= showLoadingBar;
            };
            loadingBarDisplay.OnComplete += showLoadingBar;

            AsyncOperation operation = loader.LoadAsync();
            
            loadDelay.Start();
            Action allowSceneActivation = null;
            allowSceneActivation = () =>
            {
                operation.allowSceneActivation = true;
                loadDelay.OnComplete -= allowSceneActivation;
            };
            loadDelay.OnComplete += allowSceneActivation;
            
            while (operation.progress <= 0.9 || !loadDelay.IsFinished)
            {
                loadingBarDisplay.Tick(Time.deltaTime);
                loadDelay.Tick(Time.deltaTime);
                OnLoadingProgress?.Invoke(operation.progress);

                loadingBar.value = operation.progress / 0.9f;
                
                yield return null;
            }
            
            fadeOut.Start();
            while (!fadeOut.IsFinished)
            {
                fadeOut.Tick(Time.deltaTime);
                graphicsFader.SetAlpha(1f - fadeOut.PercentageCompletion);
                yield return null;
            }
            graphicsFader.gameObject.SetActive(false);
            loadingBar.gameObject.SetActive(false);
            
            OnLoadingComplete?.Invoke();
        }
    }
}
