using System;
using CLogic.Utils.DataSaving;
using UnityEngine;
namespace CLogic.Utils.Runtime.DataSaving
{
    public class SlotProvider : MonoBehaviour, IDisposable
    {
        public string slotId;

        private void Awake()
        {
            if(Services.HasService<SlotProvider>(SceneLifeCycle.Instance))
                return;
            
            Services.Register(this, SceneLifeCycle.Instance);
            
            Slotter.CurrentSlotId = slotId;
        }
        public void Dispose()
        {
            Destroy(this);
        }
    }
}
