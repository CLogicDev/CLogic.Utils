using System;
using System.Collections.Generic;
using CLogic.Core.DataSaving;
using CLogic.Core.LifeCycles;
using CLogic.Utils.DataSaving.Sections;
using UnityEngine;
namespace CLogic.Utils.DataSaving
{
    [ExecuteAlways]
    public class GameData : MonoBehaviour, IDisposable
    {
        public DataSaver DataSaver { get; private set; }

        public List<BaseDataSectionSo> dataSections = new();
        
        private void Awake()
        {
            if(Services.HasService<GameData>(UnitySingletonLifeCycle.Instance))
            {
                Destroy(this);
                return;
            }
            
            Services.Register(this, UnitySingletonLifeCycle.Instance);
            
            Debug.Log(dataSections[0].GetDataPath().GetPath());
        }
        
        
        public void Dispose()
        {
            //TODO: Dispose of data saver?
            Destroy(this);
        }
    }
}
