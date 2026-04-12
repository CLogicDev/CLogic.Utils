using System;
using UnityEngine;
using CLogic.Core.DataSaving;
using CLogic.Core.LifeCycles;
using CLogic.Utils.Service;

namespace CLogic.Utils.Tests
{
    public class SingletonTestService : MonoBehaviour, IDisposable
    {
        private PersistentProperty<float> score;
        
        private void Awake()
        {
            if (Services.HasService<SingletonTestService>(SingletonLifeCycle.Instance))
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
