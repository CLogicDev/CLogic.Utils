using System;
using CLogic.Core.LifeCycles;
using CLogic.Utils.DataSaving;
using UnityEngine;
namespace CLogic.Utils.DataSaving
{
    public static class Slotter
    {
        private static string currentSlotId = defaultSlotId;
        private const string defaultSlotId = "NoSlotIDSet";
        
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
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void SetEnvironment()
        {
            Environment.SetEnvironmentVariable("SLOT_ID", currentSlotId);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void ResetSlot()
        {
            if(currentSlotId != defaultSlotId)
                currentSlotId = string.Empty;   
        }

    }
}
