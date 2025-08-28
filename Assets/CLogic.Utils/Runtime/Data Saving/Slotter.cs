using System;
using CLogic.Core.LifeCycles;
using CLogic.Utils.DataSaving;
using UnityEngine;
namespace CLogic.Utils.Runtime.DataSaving
{
    public static class Slotter
    {
        /// <summary>
        /// Retrieves or updates the current slot ID and refreshes the sections if applicable
        /// </summary>
        public static string CurrentSlotId
        {
            get
            {
                return currentSlotId;
            }
            set
            {
                currentSlotId = value;

                SetEnvironment();
                
                if(Services.HasService<GameData>(SingletonLifeCycle.Instance))
                    Services.Resolve<GameData>(SingletonLifeCycle.Instance).RefreshSections();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void SetEnvironment()
        {
            Environment.SetEnvironmentVariable("SLOT_ID", currentSlotId);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetSlot()
        {
            currentSlotId = string.Empty;   
        }
        
        private static string currentSlotId = "NoSlotIDSet";
    }
}
