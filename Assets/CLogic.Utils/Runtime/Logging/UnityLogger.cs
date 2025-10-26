using System;
using UnityEngine;
namespace CLogic.Utils.Logger
{
    public static class UnityLogger
    {
        private static int arrivingLog = 0;

        public static void SetArrivingLog()
        {
            arrivingLog++;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Setup()
        {
            Application.logMessageReceived += HandleLog;
        }
        private static void HandleLog(string log, string stackTrace, LogType type)
        {
            if(arrivingLog-- != 0)
                return;
            
            Logging.Log(log, GetLogLevel(type), null,  true, false);
        }

        static LogLevel GetLogLevel(LogType logType)
        {
            return logType switch
            {
                LogType.Assert => LogLevel.Info,
                LogType.Error => LogLevel.Error,
                LogType.Exception => LogLevel.Fatal,
                LogType.Log => LogLevel.Info,
                LogType.Warning => LogLevel.Warning,
                _ => throw new ArgumentOutOfRangeException(nameof(logType), logType, null)
            };
        }


    }
}
