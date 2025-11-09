#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace CLogic.Utils.DataSaving.Sections
{
    class PreProcessDataSectionSettings : IPreprocessBuildWithReport
    {
        internal const string RESOURCE_FOLDER_DIR = "Assets/Resources";
        public int callbackOrder => 0;
        
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("[CLogic Build Processor] Setting up DataSectionSettings for Player");
            
            string assetPath = AssetDatabase.GetAssetPath(DataSectionSettings.GetOrCreate());
            DataSectionSettings asset = AssetDatabase.LoadAssetAtPath<DataSectionSettings>(assetPath);
            if (asset == null)
                throw new BuildFailedException("Missing DataSectionSettings.asset!");
            
            if(!Directory.Exists(RESOURCE_FOLDER_DIR))
                Directory.CreateDirectory(RESOURCE_FOLDER_DIR);

            AssetDatabase.CopyAsset(assetPath, DataSectionSettings.SETTINGS_FILE_PATH);
            AssetDatabase.ImportAsset(DataSectionSettings.SETTINGS_FILE_PATH);
        }
    }
    
    class PostProcessDataSectionBuild : IPostprocessBuildWithReport
    {
        public int callbackOrder => int.MaxValue;
    
        public void OnPostprocessBuild(BuildReport report)
        {
            if (AssetDatabase.DeleteAsset(DataSectionSettings.SETTINGS_FILE_PATH))
                Debug.Log("[CLogic Build Processor] Cleaned up temporary DataSectionSettings from Resources");
    
            if (Directory.GetFileSystemEntries(PreProcessDataSectionSettings.RESOURCE_FOLDER_DIR).Length == 0)
            {
                AssetDatabase.DeleteAsset(PreProcessDataSectionSettings.RESOURCE_FOLDER_DIR);
                Debug.Log("[CLogic Build Processor] Deleted empty Resources folder");
            }
    
        }
    }
}
#endif