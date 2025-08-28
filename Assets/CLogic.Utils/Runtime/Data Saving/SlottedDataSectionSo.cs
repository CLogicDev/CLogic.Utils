using System;
using System.IO;
using CLogic.Core.DataSaving;
using CLogic.Core.DataSaving.Obfuscation;
using CLogic.Utils.DataSaving.Sections;
using UnityEngine;
namespace CLogic.Utils.Runtime.Data_Saving
{
    [CreateAssetMenu(fileName = "Slotted Data Section", menuName = "CLogic/Data Saving/Slotted Data Section")]
    public class SlottedDataSectionSo : DataSectionSo
    {
        public int currentSlotId = 0;
        
        public string slotPattern;
        
        public override IDataPath GetDataPath()
        {
            if(string.IsNullOrEmpty(slotPattern))
                return base.GetDataPath();
            
            char dirSeparator = Path.DirectorySeparatorChar;
            string expandedPath = Environment.ExpandEnvironmentVariables(relativePath).Replace('\\', dirSeparator).Replace('/', dirSeparator);

            string slotName = slotPattern.Replace("%id%", currentSlotId.ToString());
            string fullPath = Path.Combine(expandedPath, slotName, fileName);
            
            return new StringPath(fullPath);
        }

        protected override void Reset()
        {
            base.Reset();
            slotPattern = "Slot %id%";
        }
    }
}
