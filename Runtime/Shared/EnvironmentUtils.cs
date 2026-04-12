using System;
using UnityEngine;
using System.Collections.Generic;

namespace CLogic.Utils.Shared
{
    public static class EnvironmentUtils
    {
        public static bool EnvironmentSet { get; private set; } = false;
        
        public static event Action OnEnvironmentSet;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration), UnityEditor.InitializeOnLoadMethod]
        #if UNITY_EDITOR
        #endif
        public static void InjectEnvironmentVariables()
        {
            Dictionary<string, string> environmentVariables = new()
            {
                { "PERSISTENT_DATA", Application.persistentDataPath },
                { "TEMP_DATA", Application.temporaryCachePath },
                { "STREAMING_ASSETS", Application.streamingAssetsPath },
                { "DATA_PATH", Application.dataPath },
                { "PLATFORM", Application.platform.ToString() }
            };
            
            foreach (KeyValuePair<string, string> kvp in environmentVariables)
            {
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(kvp.Key)))
                {
                    Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
                }
            }
            EnvironmentSet = true;
            OnEnvironmentSet?.Invoke();
        }
    }
}
