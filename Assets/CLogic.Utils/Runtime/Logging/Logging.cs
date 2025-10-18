using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using CLogic.Core;
using CLogic.Utils;
namespace CLogic.Utils.Logger
{
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4
    }
    public static class Logging
    {
        private static Dictionary<LogLevel, Color> logColors = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void UpdateSettings()
        {
            LoggerSettings settings = LoggerSettings.GetOrCreate();
            
            logColors.Clear();
            logColors.Add(LogLevel.Debug, settings.logColors.debug);
            logColors.Add(LogLevel.Info, settings.logColors.info);
            logColors.Add(LogLevel.Warning, settings.logColors.warn);
            logColors.Add(LogLevel.Error, settings.logColors.error);
            logColors.Add(LogLevel.Fatal, settings.logColors.fatal);
        }
        
        public static void Log(string message, LogLevel level)
        {
            StringBuilder builder = new();
            builder.AppendLine(message);

            builder.Insert(0, $"[{Enum.GetName(typeof(LogLevel), level)}] "); // Log Level Prefix
            builder.Insert(0, GetLogColor(level)); // Log Coloring
            
            ULog(builder + " passed through logger", level);
        }

        static string GetLogColor(LogLevel level)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(logColors[level])}>";
        }

        static void ULog(string finalLog, LogLevel level)
        {
            switch (level)
            {
                case <= LogLevel.Info:
                    Debug.Log(finalLog);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(finalLog);
                    break;
                case >= LogLevel.Error:
                    Debug.LogError(finalLog);
                    break;
            }
        }
    }
}
