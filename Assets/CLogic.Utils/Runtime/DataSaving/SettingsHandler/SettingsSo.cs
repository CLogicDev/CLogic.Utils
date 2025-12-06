using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CLogic.Utils.Settings
{
    public abstract class SettingsSo<T> : ScriptableObject where T : SettingsSo<T>
    {
        private const string DEFAULT_CREATION_PATH = "Assets";

        protected internal abstract string AssetName { get; set; }
        
        protected abstract string Key { get; set; }

        protected static T settingsCache;
        
        public static T GetOrCreateSettings()
        {
            T instance = CreateInstance<T>();
            #if UNITY_EDITOR

            if(settingsCache != null || UnityEditor.EditorBuildSettings.TryGetConfigObject(instance.Key, out settingsCache))
            {
                DestroyImmediate(instance);
                return settingsCache;
            }

            settingsCache = instance;

            string creationPath = Path.Combine(DEFAULT_CREATION_PATH, instance.AssetName);
            UnityEditor.AssetDatabase.CreateAsset(settingsCache,creationPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            
            UnityEditor.EditorBuildSettings.AddConfigObject(instance.Key, settingsCache, true);
            
            #else
            Debug.LogWarning(instance.AssetName);
            settingsCache = Resources.Load<T>(Path.GetFileNameWithoutExtension(instance.AssetName));
            DestroyImmediate(instance);
            #endif
            
            if(settingsCache != null)
                Debug.LogWarning("Asset exists");
            
            return settingsCache;
        }
    }
}
