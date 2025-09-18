using System;
using CLogic.Utils.DataSaving;
using UnityEngine;
namespace CLogic.Utils.Runtime.DataSaving
{
    [ExecuteAlways]
    public class SlotProvider : MonoBehaviour, IDisposable
    {
        public string slotId;

        private string cachedSlotId;
        
        public bool editorSupport = true;
        
        private void Awake()
        {
            if(Application.isEditor && editorSupport)
                UpdateSlot();   
        }
        
        void UpdateSlot()
        {
            if(Application.isEditor && !editorSupport)
                return;
            
            if(!Application.isEditor)
            {
                if(Services.HasService<SlotProvider>(SceneLifeCycle.Instance))
                    return;

                Services.Register(this, SceneLifeCycle.Instance);
            }
            Slotter.CurrentSlotId = slotId;
            GameData.UpdateSections();
            
            cachedSlotId = slotId;
        }
        
        public void Dispose()
        {
            Destroy(this);
        }

        private void OnValidate()
        {
            if(cachedSlotId != slotId)
                UpdateSlot();
        }
    }
}
