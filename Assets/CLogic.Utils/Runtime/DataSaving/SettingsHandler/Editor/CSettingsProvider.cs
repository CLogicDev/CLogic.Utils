using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace CLogic.Utils.Settings
{
    public interface ICSettingsProvider<T> where T : ScriptableObject
    {
        public string SettingsPath { get; }
        public string SettingsLabel { get; }
        
        public SettingsScope SettingsScope { get; }
        
        public HashSet<string> SearchKeywords { get; }
    }
    
    public abstract class CSettingsProviderBase<TSo, TInstance> : ICSettingsProvider<TSo> where TSo : ScriptableObject where TInstance : CSettingsProviderBase<TSo, TInstance>, new()
    {
        public abstract string SettingsPath { get; }
        public abstract string SettingsLabel { get; }
        
        public abstract SettingsScope SettingsScope { get; }
        
        public abstract HashSet<string> SearchKeywords { get; }
        
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            TInstance instance = new();

            SettingsProvider provider = new(instance.SettingsPath, instance.SettingsScope, instance.SearchKeywords)
            {
                label = instance.SettingsLabel,
                guiHandler = instance.OnGUI
            };

            return provider;
        }

        protected abstract void OnGUI(string searchContext);

    }
}
