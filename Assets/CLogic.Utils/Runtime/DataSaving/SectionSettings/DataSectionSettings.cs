using System.Collections.Generic;
using CLogic.Utils.Settings;
namespace CLogic.Utils.DataSaving.Sections
{
    public class DataSectionSettings : SettingsSo<DataSectionSettings>
    {
        internal const string KEY = "dev.clogic.datasections";
        protected internal override string AssetName { get; set; } = "DataSectionSettings.asset";
        protected override string Key { get; set; } = KEY;
        
        public List<BaseDataSectionSo> dataSections = new();
        public string defaultSlot = "Default";
        public bool doAutoSave;
        public double autoSaveDelaySeconds = GameData.autoSaveInterval.TotalSeconds;
        
        private void OnValidate()
        {
            GameData.UpdateSections();
        }
    }
}
