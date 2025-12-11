using System;
using System.Collections.Generic;
using CLogic.Utils.Runtime.DataSaving.Obfuscator;
using CLogic.Utils.Settings;
namespace CLogic.Utils.DataSaving.Sections
{
    [Serializable]
    public class DefaultSection
    {
        [NonSerialized]
        public string sectionId = "default";

        public SavingMode savingMode = SavingMode.Instant;

        public string relativePath = "%PERSISTENT_DATA%";
        public string fileName = "default.bin";

        public ObfuscatorSo obfuscator;
    }
    
    public class DataSectionSettings : SettingsSo<DataSectionSettings>
    {
        internal const string KEY = "dev.clogic.datasections";
        protected internal override string AssetName { get; set; } = "DataSectionSettings.asset";
        protected override string Key { get; set; } = KEY;
        
        public List<BaseDataSectionSo> dataSections = new();

        public DefaultSection defaultSection = new();
        
        public string defaultSlot = "Default";
        public bool doAutoSave;
        public double autoSaveDelaySeconds = GameData.autoSaveInterval.TotalSeconds;
        
        
        private void OnValidate()
        {
            GameData.UpdateSections();
        }
    }
}
