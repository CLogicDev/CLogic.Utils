using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CLogic.Utils.Shared
{
    public abstract class SettingsSo<T> : ScriptableObject where T : SettingsSo<T>
    {
        private const string DEFAULT_CREATION_PATH = "Assets";
        
        protected abstract string AssetName { get; set; }
        
        protected abstract string Key { get; set; }
        
        protected static T settingsCache;
        
        public static T GetOrCreateSettings()
        {
            var instance = CreateInstance<T>();
            #if UNITY_EDITOR
            
            if (settingsCache != null || UnityEditor.EditorBuildSettings.TryGetConfigObject(instance.Key, out settingsCache))
            {
                DestroyImmediate(instance);
                return settingsCache;
            }
            
            settingsCache = instance;
            
            instance.OnSettingsCreated();
            
            string creationPath = Path.Combine(DEFAULT_CREATION_PATH, instance.AssetName);
            UnityEditor.AssetDatabase.CreateAsset(settingsCache, creationPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            
            UnityEditor.EditorBuildSettings.AddConfigObject(instance.Key, settingsCache, true);
            
            #else
            settingsCache = Resources.Load<T>(Path.GetFileNameWithoutExtension(instance.AssetName));
            DestroyImmediate(instance);
            #endif
            
            return settingsCache;
        }
        
        protected virtual void OnSettingsCreated() {}
    }
}
