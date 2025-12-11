#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace CLogic.Utils.Logger
{
    public static class LoggerSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new ("Project/CLogic/Logger Settings", SettingsScope.Project)
            {
                label = "Logger Settings",
                guiHandler = (searchContext) =>
                {
                    LoggerSettings settings = LoggerSettings.GetOrCreateSettings();
                    
                    SerializedObject so = new (settings);
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(LoggerSettings.logColors)), true);
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(LoggerSettings.logFilePath)), true);
                    so.ApplyModifiedProperties();

                    if (GUI.changed)
                    {
                        UnityEditor.EditorBuildSettings.AddConfigObject(LoggerSettings.KEY, settings, true);
                        Logging.UpdateSettings();
                    }
                },
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "Log", "Logger", "Logging" })
            };

            return provider;
        }
    }
}
#endif
