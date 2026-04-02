using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CLogic.Utils.Shared;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace CLogic.Utils.Settings
{
    public class SettingsPreProcessor : IPreprocessBuildWithReport
    {
        internal const string RESOURCES_FOLDER_DIR = "Assets/" + RESOURCES_FOLDER_NAME;
        internal const string RESOURCES_FOLDER_NAME = "Resources";
        public int callbackOrder => 0;

#pragma warning disable UDR0001 
        //Reset on the post processor
        internal static List<string> buildPaths = new();
#pragma warning restore UDR0001
        
        public void OnPreprocessBuild(BuildReport report)
        {
            List<Type> settingTypes = GetSettingsToProcess();
            buildPaths = new List<string>(settingTypes.Count);

            
            if(settingTypes.Count != 0 && !Directory.Exists(RESOURCES_FOLDER_DIR))
                AssetDatabase.CreateFolder("Assets", RESOURCES_FOLDER_NAME);
            
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

#pragma warning disable UDR0005
            Application.logMessageReceived += HandleLogFailMessage;
#pragma warning restore UDR0005
        }

        void HandleLogFailMessage(string msg, string _, LogType type)
        {
            if(type == LogType.Error && (msg.Contains(nameof(BuildFailedException)) || msg.Contains("Build completed with a result of 'Failed'")))
            {
                new SettingsPostProcessor().DiscardBuildResources();
            }

            if(type == LogType.Log && msg.Contains("Cancelled"))
            {
                new SettingsPostProcessor().DiscardBuildResources();
            }
            
            Application.logMessageReceived -= HandleLogFailMessage;
        }

        public void ProcessType(Type type)
        { 
            Debug.Log("[CLogic Build Processor] Processing " + type.FullName);
            //No graceful error handling, Unity will fail the build on error
            var GetOrCreateMethod = type.GetMethod("GetOrCreateSettings", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            var GetAssetName = type.GetProperty("AssetName", BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public);

            ScriptableObject asset = (ScriptableObject)GetOrCreateMethod.Invoke(null, null);
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string assetName = (string)GetAssetName.GetValue(asset);
            
            if(asset == null)
                throw new BuildFailedException($"[CLogic Build Processor] Missing asset file for {type.FullName}");

            string temporaryBuildPath = Path.Combine(RESOURCES_FOLDER_DIR, assetName);
            AssetDatabase.CopyAsset(assetPath, temporaryBuildPath);
            AssetDatabase.ImportAsset(temporaryBuildPath);
            
            buildPaths.Add(temporaryBuildPath);
        }
        
        public List<Type> GetSettingsToProcess()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.BaseType?.IsGenericType == true && t.BaseType.GetGenericTypeDefinition() == typeof(SettingsSo<>))
                .ToList();
        }

        ~SettingsPreProcessor()
        {
            Application.logMessageReceived -= HandleLogFailMessage;
        }
    }

    class SettingsPostProcessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => int.MaxValue;
        
        public void OnPostprocessBuild(BuildReport report)
        {
            DiscardBuildResources();
        }

        public void DiscardBuildResources()
        {
            foreach (string buildPath in SettingsPreProcessor.buildPaths)
            {
                Debug.Log("[CLogic Build Processor] Processing post build for asset at" + buildPath);
                AssetDatabase.DeleteAsset(buildPath);
            }
            
            SettingsPreProcessor.buildPaths.Clear();

            if(!Directory.Exists(SettingsPreProcessor.RESOURCES_FOLDER_DIR)) //On Unity 6000, test runner automatically deletes the resources folder
                return;
            
            if(Directory.GetFileSystemEntries(Path.GetFullPath(SettingsPreProcessor.RESOURCES_FOLDER_DIR)).Length != 0)
                return;
            // Delete resources folder after use if it was not created previously
            AssetDatabase.DeleteAsset(SettingsPreProcessor.RESOURCES_FOLDER_DIR);
            Debug.Log("[CLogic Build Processor] Deleted empty Resources folder");
        }
    }
}
