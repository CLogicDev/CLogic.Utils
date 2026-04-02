#if UNITY_EDITOR
using CLogic.Utils;
using UnityEditor;
using UnityEngine;
namespace CLogic.Systems.Logging
{
    public static class LoggerSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new ("Project/CLogic/UI Settings", SettingsScope.Project)
            {
                label = "UI Settings",
                guiHandler = (searchContext) =>
                {
                    UISettings settings = UISettings.GetOrCreateSettings();
                    
                    SerializedObject so = new (settings);
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(UISettings.modalWindow)), true);
                    so.ApplyModifiedProperties();

                    if (GUI.changed)
                    {
                        UnityEditor.EditorBuildSettings.AddConfigObject(UISettings.KEY, settings, true);
                    }
                },
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "UI" })
            };

            return provider;
        }
    }
}
#endif
