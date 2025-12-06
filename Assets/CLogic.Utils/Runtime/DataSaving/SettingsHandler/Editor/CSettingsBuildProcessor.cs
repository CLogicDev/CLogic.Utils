using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CLogic.Utils.DataSaving.Sections;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace CLogic.Utils.Settings
{
    public class CSettingsPreProcessor : IPreprocessBuildWithReport
    {
        internal const string RESOURCE_FOLDER_DIR = "Assets/Resources";
        public int callbackOrder => 0;

        internal static List<string> buildPaths = new();
        
        public void OnPreprocessBuild(BuildReport report)
        {
            List<Type> settingTypes = GetSettingsToProcess();
            buildPaths = new List<string>(settingTypes.Count);

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (Type type in settingTypes)
                {
                    ProcessType(type);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            Application.logMessageReceived += HandleLogFailMessage;
        }

        void HandleLogFailMessage(string msg, string _, LogType type)
        {
            if(type == LogType.Error && (msg.Contains(nameof(BuildFailedException)) || msg.Contains("Build completed with a result of 'Failed'")))
            {
                new CSettingsPostProcessor().DiscardBuildResources();
            }
            
            Application.logMessageReceived -= HandleLogFailMessage;
        }

        public void ProcessType(Type type)
        { 
            //No graceful error handling, Unity will fail the build on error
            var GetOrCreateMethod = type.GetMethod("GetOrCreateSettings", BindingFlags.Static | BindingFlags.Public);
            var GetAssetName = type.GetProperty(nameof(TestSettingSo.AssetName), BindingFlags.Static);
            
            string assetPath = AssetDatabase.GetAssetPath((ScriptableObject)GetOrCreateMethod.Invoke(null, null));
            string assetName = (string)GetAssetName.GetValue(null);
            
            ScriptableObject asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
            if(asset == null)
                throw new BuildFailedException($"[CLogic Build Processor] Missing asset file for {type.FullName}");
            
            if(!Directory.Exists(RESOURCE_FOLDER_DIR))
                Directory.CreateDirectory(RESOURCE_FOLDER_DIR);

            string temporaryBuildPath = Path.Combine(RESOURCE_FOLDER_DIR, assetName);
            AssetDatabase.CopyAsset(assetPath, temporaryBuildPath);
            AssetDatabase.ImportAsset(temporaryBuildPath);
        }
        
        public List<Type> GetSettingsToProcess()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.BaseType?.IsGenericType == true && t.BaseType.GetGenericTypeDefinition() == typeof(SettingsSo<>))
                .ToList();
        }
    }

    class CSettingsPostProcessor : IPostprocessBuildWithReport
    {

        public int callbackOrder => int.MaxValue;
        
        public void OnPostprocessBuild(BuildReport report)
        {
            DiscardBuildResources();
        }

        public void DiscardBuildResources()
        {
            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (string buildPath in CSettingsPreProcessor.buildPaths)
                {
                    AssetDatabase.DeleteAsset(buildPath);
                }

                if(Directory.GetFileSystemEntries(CSettingsPreProcessor.RESOURCE_FOLDER_DIR).Length != 0)
                    return;
                // Delete resources folder after use if it was not created previously
                AssetDatabase.DeleteAsset(CSettingsPreProcessor.RESOURCE_FOLDER_DIR);
                Debug.Log("[CLogic Build Processor] Deleted empty Resources folder");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}
