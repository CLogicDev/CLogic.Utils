using System;
using System.Collections.Generic;
using System.IO;
using CLogic.Utils.Shared;
using UnityEngine;
namespace CLogic.Utils.Logger
{
    public class LoggerSettings : SettingsSo<LoggerSettings>
    {
        internal const string KEY = "dev.clogic.logger";

        protected override string AssetName { get; set; } = "LoggerSettings.asset";
        protected override string Key { get; set; } = KEY;
        
        
        [Serializable]
        public class LogColors
        {
            public Color debug = Color.gray4;
            public Color info = Color.white;
            public Color warn = Color.yellowNice;
            public Color error = Color.orangeRed;
            public Color fatal = Color.red;
        }

        public LogColors logColors = new ();

        public string logFilePath = "%PERSISTENT_DATA%/Logs/log.txt";
    }
}
