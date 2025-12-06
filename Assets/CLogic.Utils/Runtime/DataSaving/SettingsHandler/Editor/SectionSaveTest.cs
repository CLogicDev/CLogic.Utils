using System.Collections.Generic;
using CLogic.Utils.Settings;
using UnityEditor;
using UnityEngine;
namespace CLogic.Utils.DataSaving.Sections
{
    public class SectionSaveTest : CSettingsProviderBase<DataSectionSo, SectionSaveTest>
    {
        public override string SettingsPath { get; } = "Project/CLogic/Saving Settings";
        public override string SettingsLabel { get; } = "Saving Settings";
        public override SettingsScope SettingsScope { get; } = SettingsScope.Project;
        public override HashSet<string> SearchKeywords { get; } = new(new[] { "Data", "Section" });
        
        protected override void OnGUI(string searchContext)
        {
                    
            DataSectionSettings settings = DataSectionSettings.GetOrCreate();
                    
            SerializedObject so = new (settings);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(DataSectionSettings.dataSections)), true);
            EditorGUILayout.PropertyField(so.FindProperty(nameof(DataSectionSettings.defaultSlot)), true);
            so.ApplyModifiedProperties();

            if (GUI.changed)
            {
                UnityEditor.EditorBuildSettings.AddConfigObject(DataSectionSettings.KEY, settings, true);
                GameData.UpdateSections();
            }
        }
    }
}
