using System;
using System.Collections.Generic;
using UnityEngine;
namespace CLogic.Utils.DataSaving.Sections
{
    public class DataSectionSettings : ScriptableObject
    {
        internal const string KEY = "dev.clogic.datasections";
        
        public List<BaseDataSectionSo> dataSections;

        private static DataSectionSettings cache;

        public static DataSectionSettings GetOrCreate()
        {
            #if UNITY_EDITOR
            if(cache != null || UnityEditor.EditorBuildSettings.TryGetConfigObject(KEY, out cache))
                return cache;

            cache = CreateInstance<DataSectionSettings>();
            UnityEditor.AssetDatabase.CreateAsset(cache, "Assets/DataSectionSettings.asset");
            UnityEditor.AssetDatabase.SaveAssets();
                    
            UnityEditor.EditorBuildSettings.AddConfigObject(KEY, cache, true);
            #endif
            
            return cache;
        }

        private void OnValidate()
        {
            GameData.UpdateSections();
        }
    }
}
