using System;
using CLogic.Core.LifeCycles;
using CLogic.Utils.DataSaving;
using CLogic.Utils.Runtime.DataSaving;
using UnityEngine;
namespace CLogic.Utils.Tests
{
    public class SlotChanger : MonoBehaviour
    {
        private GameData gameData;

        public string saveId;

        private void Start()
        {
            gameData = Services.Resolve<GameData>(UnitySingletonLifeCycle.Instance);
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.T))
            {
                Slotter.CurrentSlotId = saveId;

                Debug.Log(gameData.dataSections[0].GetDataPath().GetPath());
            }
        }
    }
}
