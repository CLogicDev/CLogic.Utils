using System;
using CLogic.Utils.DataSaving;
using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class DirtySaveTest : MonoBehaviour
    {
        public string dataId = "test.dirty";
        public string sectionId = "dirty";

        public string dataToSave;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
                GameData.SetData(dataId, dataToSave, sectionId);
            
            if(Input.GetKeyDown(KeyCode.C))
                Debug.Log(GameData.GetData(dataId, sectionId, "no data saved"));
            
            if(Input.GetKeyDown(KeyCode.V))
                GameData.SaveDirtyToDisk();

            GameData.AutoSaveDirtySections = true;
            GameData.autoSaveInterval = TimeSpan.FromSeconds(5);
        }

        private void Start()
        {
            GameData.AutoSaveDirtySections = true;
            Debug.Log("setting data " + dataToSave);
            GameData.SetData(dataId, dataToSave, sectionId);
        }
    }
}
