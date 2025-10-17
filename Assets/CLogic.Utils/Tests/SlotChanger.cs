using System;
using CLogic.Core.LifeCycles;
using CLogic.Utils.DataSaving;
using CLogic.Utils.Runtime.DataSaving;
using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class SlotChanger : MonoBehaviour
    {

        public string saveId;

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log(GameData.dataSections[0].GetDataPath().GetPath());
            }
            if(Input.GetKeyDown(KeyCode.T))
            {
                Slotter.CurrentSlotId = saveId;

                Debug.Log(GameData.dataSections[0].GetDataPath().GetPath());
            }
        }
    }
}
