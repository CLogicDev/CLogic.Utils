using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace CLogic.Utils.DataSaving.Sections
{
    public class DataSectionSettings : ScriptableObject
    {
        internal const string SETTINGS_FILE_PATH = "Assets/Resources/DataSectionSettings.asset";
        internal const string KEY = "dev.clogic.datasections";

        public List<BaseDataSectionSo> dataSections = new();
        public string defaultSlot = "Default";

        private static DataSectionSettings cache;

        public static DataSectionSettings GetOrCreate()
        {
        #if UNITY_EDITOR
            if(cache != null || UnityEditor.EditorBuildSettings.TryGetConfigObject(KEY, out cache))
                return cache;

            cache = CreateInstance<DataSectionSettings>();
            UnityEditor.AssetDatabase.CreateAsset(cache, "Assets/DataSectionSettings.asset");
            UnityEditor.AssetDatabase.SaveAssets();
                    
            UnityEditor.EditorBuildSettings.AddConfigObject(KEY, cache, true);//
            
        #else    
            cache = Resources.Load<DataSectionSettings>(Path.GetFileNameWithoutExtension(SETTINGS_FILE_PATH));
        #endif

            
            return cache;
        }

        private void OnValidate()
        {
            GameData.UpdateSections();
        }
    }
}
