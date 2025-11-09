using System;
using System.Collections.Generic;
using CLogic.Core.DataSaving;
using CLogic.Core.LifeCycles;
using CLogic.Utils.DataSaving.Sections;
using UnityEngine;
namespace CLogic.Utils.DataSaving
{
    public static class GameData
    {
        public static DataSaver DataSaver { get; private set; } // rest
        
        public static List<BaseDataSectionSo> dataSections = new();

        public static bool IsInitialized => DataSaver != null;
        
        static bool canUpdateSections = true;
        
        public static Action OnSectionsUpdated;
        public static Action OnDataInitialized;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        #endif
        public static void UpdateSections()
        {
            if(!canUpdateSections)
                return;
            if(!EnvUtils.EnvironmentSet)
            {
                EnvUtils.OnEnvironmentSet += UpdateSectionsInternal;
                return;
            }
            UpdateSectionsInternal();
        }

        static void UpdateSectionsInternal()
        {
            bool isFirstInit = DataSaver == null;
            
            canUpdateSections = false; // Prevents updating section twice due to OnValidate
            DataSectionSettings settings = DataSectionSettings.GetOrCreate();
            canUpdateSections = true;
            
            //Ensure default slots from project settings
            if(Slotter.CurrentSlotId == Slotter.defaultSlotId || Environment.GetEnvironmentVariable(Slotter.ENVIRONMENT_VARIABLE_NAME) == null)
                Slotter.CurrentSlotId = settings.defaultSlot;
            
            
            dataSections = settings.dataSections;
            
            List<IDataSection> saveSections = new (settings.dataSections);
            DataSaver = new DataSaver(saveSections);
            
            if(isFirstInit)
                OnDataInitialized?.Invoke();
            
            OnSectionsUpdated?.Invoke();
            EnvUtils.OnEnvironmentSet -= UpdateSectionsInternal;
            
        }
    }
}
