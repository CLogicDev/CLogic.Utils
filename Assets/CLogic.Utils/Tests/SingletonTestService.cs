using System;
using CLogic.Core.LifeCycles;
using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class SingletonTestService : MonoBehaviour, IDisposable
    {
        private void Awake()
        {
            if(Services.HasService<SingletonTestService>(SingletonLifeCycle.Instance))
            {
                Dispose();
                return;
            }
            
            Services.Register(this, SingletonLifeCycle.Instance);
            
            DontDestroyOnLoad(gameObject);
        }
        public void Dispose()
        {
            Debug.Log("Singleton Dispose");
            Destroy(this);
        }
    }
}
