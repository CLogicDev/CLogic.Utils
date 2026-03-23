using System;
using UnityEngine;
using Object = UnityEngine.Object;
namespace CLogic.Utils
{
    public partial class StaticUtils
    {
        public static void Log(string message, Object context = null) => LogInfo(message, context);
        
        public static void LogDebug(string message, Object context = null) => LogInternal(message, 0, context);
        public static void LogInfo(string message, Object context = null) => LogInternal(message, 1, context);
        public static void LogWarning(string message, Object context = null) => LogInternal(message, 2, context);
        public static void LogError(string message, Object context = null) => LogInternal(message, 3, context);
        public static void LogFatal(string message, Object context = null) => LogInternal(message, 4, context);
        
        private static void LogInternal(string message, int logLevel, Object context)
        {
            // Log levels
            // Debug = 0,
            // Info = 1,
            // Warning = 2,
            // Error = 3,
            // Fatal = 4
            
            #if CLOGIC_LOGGING
            Logging.Log(message, (CLogic.Systems.Logging.LogLevel)logLevel, context);
            #else

            switch (logLevel)
            {
                case 0 or 1:
                    Debug.Log(message, context);
                    break;
                case 2:
                    Debug.LogWarning(message, context);
                    break;
                case 3 or 4:
                    Debug.LogError(message, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), "Invalid log level");
            }
            #endif
        }
    }
}
