using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CLogic.Utils.Logger
{
    public class LoggerSettings : ScriptableObject
    {
        internal const string SETTINGS_FILE_PATH = "Assets/Resources/LoggerSettings.asset";
        internal const string KEY = "dev.clogic.logger";
        
        private static LoggerSettings cache;
        
        [Serializable]
        public class LogColors
        {
            public Color debug = Color.gray4;
            public Color info = Color.white;
            public Color warn = Color.yellowNice;
            public Color error = Color.orangeRed;
            public Color fatal = Color.red;
        }

        public LogColors logColors = new LogColors();
        
        public static LoggerSettings GetOrCreate()
        {
        #if UNITY_EDITOR
            if(cache != null || UnityEditor.EditorBuildSettings.TryGetConfigObject(KEY, out cache))
                return cache;

            cache = CreateInstance<LoggerSettings>();
            UnityEditor.AssetDatabase.CreateAsset(cache, "Assets/LoggerSettings.asset");
            UnityEditor.AssetDatabase.SaveAssets();
                    
            UnityEditor.EditorBuildSettings.AddConfigObject(KEY, cache, true);
            
        #else    
            cache = Resources.Load<LoggerSettings>(Path.GetFileNameWithoutExtension(SETTINGS_FILE_PATH));
        #endif

            
            return cache;
        }

        private void OnValidate()
        {
            Logging.UpdateSettings();
        }
    }
}
