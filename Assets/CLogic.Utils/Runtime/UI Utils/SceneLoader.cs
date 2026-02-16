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

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.I))
                LoadSceneAsync(0);
        }

        private Coroutine loadingCoroutine;
        
        public void LoadSceneAsync(string sceneName) => LoadSceneAsync(sceneName, new LoadSceneParameters(LoadSceneMode.Single));
        public void LoadSceneAsync(string sceneName, LoadSceneParameters parameters) => LoadSceneAsync(SceneManager.GetSceneByName(sceneName), parameters);
        
        public void LoadSceneAsync(int buildIndex) => LoadSceneAsync(buildIndex, new LoadSceneParameters(LoadSceneMode.Single));
        public void LoadSceneAsync(int buildIndex, LoadSceneParameters parameters)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            LoadSceneAsync(Path.GetFileNameWithoutExtension(scenePath), parameters); //No API to get scene objecy by build id
        }

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
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.buildIndex, parameters);
            
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
