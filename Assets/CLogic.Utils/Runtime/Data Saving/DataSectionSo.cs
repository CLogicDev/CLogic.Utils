using System;
using System.IO;
using CLogic.Core.DataSaving;
using CLogic.Core.DataSaving.Obfuscation;
using CLogic.Utils.Runtime.DataSaving.Obfuscator;
using UnityEngine;
namespace CLogic.Utils.DataSaving.Sections
{
    public abstract class BaseDataSectionSo : ScriptableObject, IDataSection
    {
        public abstract IDataObfuscator GetObfuscator();
        public abstract IDataPath GetDataPath();
        public abstract string GetSectionId();
    }
    
    [CreateAssetMenu(fileName = "Data Section", menuName = "CLogic/Data Saving/Data Section")]
    public class DataSectionSo : BaseDataSectionSo
    {
        public string sectionId;
        
        public string relativePath;
        public string fileName;

        public ObfuscatorSo obfuscator;
        
        protected virtual void Reset()
        {
            sectionId = "default";
            relativePath = "%PERSISTENT_DATA%/CLogic/Testing Saves";
            fileName = "game_data.json";
        }

        public override IDataPath GetDataPath()
        {
            char dirSeparator = Path.DirectorySeparatorChar;
            string expandedPath = Environment.ExpandEnvironmentVariables(relativePath).Replace('\\', dirSeparator).Replace('/', dirSeparator);
            
            return new StringPath(Path.Combine(expandedPath, fileName));
        }
        public override IDataObfuscator GetObfuscator()
        {
            return obfuscator == null ? new StringObfuscator() : obfuscator;
        }
        public override string GetSectionId() => sectionId;
    }
}
