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

        private static DataSectionSettings cache;

        
        private void OnValidate()
        {
            GameData.UpdateSections();
        }
    }
}
