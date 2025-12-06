using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CLogic.Utils.DataSaving.Sections
{
    public abstract class SettingsSo<T> : ScriptableObject where T : SettingsSo<T>
    {
        private const string BUILD_PATH = "Assets/Resources";
        private const string DEFAULT_CREATION_PATH = "Assets";

        protected internal static string AssetName { get; set; }
        
        protected static string Key { get; set; }

        protected static T settingsCache;
        
        public static T GetOrCreateSettings()
        {
            #if UNITY_EDITOR
            if(settingsCache != null || UnityEditor.EditorBuildSettings.TryGetConfigObject(Key, out settingsCache))
                return settingsCache;

            settingsCache = CreateInstance<T>();
            
            string creationPath = Path.Combine(DEFAULT_CREATION_PATH, AssetName);
            UnityEditor.AssetDatabase.CreateAsset(settingsCache,creationPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            
            UnityEditor.EditorBuildSettings.AddConfigObject(Key, settingsCache, true);
            
            #else
            settingsCache = Resources.Load<T>(Path.Combine(BUILD_PATH, AssetName));
            #endif
            
            return settingsCache;
        }
    }
}
