using System;
using System.Collections.Generic;
using CLogic.Core.DataSaving;
using CLogic.Core.LifeCycles;
using CLogic.Utils.DataSaving.Sections;
using UnityEngine;
namespace CLogic.Utils.DataSaving
{
    public static class GameData
    {
        public static DataSaver DataSaver { get; private set; }
        
        public static List<BaseDataSectionSo> dataSections = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void UpdateSections()
        {
            DataSectionSettings settings = DataSectionSettings.GetOrCreate();
            dataSections = settings.dataSections;
            
            List<IDataSection> saveSections = new (settings.dataSections);
            DataSaver = new DataSaver(saveSections);
            
        }
    }
}
