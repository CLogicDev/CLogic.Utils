using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CLogic.Utils.Tests
{
    public class SceneChanger : MonoBehaviour, IDisposable
    {
        
        public string sceneName = "SceneTestService";
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Services.Register(this, SceneLifeCycle.Instance);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                SceneManager.LoadScene(sceneName);
            }
        }
        public void Dispose()
        {
            Destroy(this);
        }
    }
}
