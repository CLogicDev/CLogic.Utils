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
                    EditorGUILayout.HelpBox("No settings available yet.", MessageType.Info);
                    
                    DataSectionSettings settings = DataSectionSettings.GetOrCreate();
                    
                    SerializedObject so = new (settings);
                    EditorGUILayout.PropertyField(so.FindProperty(nameof(DataSectionSettings.dataSections)), true);
                    so.ApplyModifiedProperties();

                    if (GUI.changed)
                    {
                        UnityEditor.EditorBuildSettings.AddConfigObject(DataSectionSettings.KEY, settings, true);
                        GameData.UpdateSections();
                    }
                },
                keywords = new System.Collections.Generic.HashSet<string>(new[] { "Data", "Section", "Settings" })
            };

            return provider;
        }
    }
}
