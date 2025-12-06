#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace CLogic.Utils.DataSaving.Sections
{
    public static class DataSectionSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            SettingsProvider provider = new ("Project/CLogic/Saving Settings", SettingsScope.Project)
            {
                label = "Saving Settings",
                guiHandler = (searchContext) =>
                {
                    DataSectionSettings settings = DataSectionSettings.GetOrCreateSettings();
                    
                    SerializedObject so = new (settings);
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(DataSectionSettings.dataSections)), true);
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(DataSectionSettings.defaultSlot)), true);
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(DataSectionSettings.doAutoSave)), true);
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(DataSectionSettings.autoSaveDelaySeconds)), true);
                    so.ApplyModifiedProperties();

                    if (GUI.changed)
                    {
                        EditorBuildSettings.AddConfigObject(DataSectionSettings.KEY, settings, true);
                        GameData.UpdateSections();
                    }
                },
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "Data", "Section", "Settings" })
            };

            return provider;
        }
    }
}
#endif
