using UnityEngine;
namespace CLogic.Utils
{
    public partial class Integrations
    {
        /*
         *  Debug = 0,
         *  Info = 1,
         *  Warning = 2,
         *  Error = 3,
         *  Fatal = 4
         */
        
        #if CLOGIC_LOGGING
        [Systems.Logging.IgnoreStackTrace, HideInCallstack]
        #endif
        public static void Log(string message) => Log(message, 1);
        
        #if CLOGIC_LOGGING
        [Systems.Logging.IgnoreStackTrace, HideInCallstack]
        #endif
        public static void Log(string message, Object context) => Log(message, 1, context);
        
        #if CLOGIC_LOGGING
        [Systems.Logging.IgnoreStackTrace, HideInCallstack]
        #endif
        public static void LogWarning(string message) => Log(message, 2);
        
        #if CLOGIC_LOGGING
        [Systems.Logging.IgnoreStackTrace, HideInCallstack]
        #endif
        public static void LogWarning(string message, Object context) => Log(message, 2, context);
        
        #if CLOGIC_LOGGING
        [Systems.Logging.IgnoreStackTrace, HideInCallstack]
        #endif
        public static void LogError(string message) => Log(message, 3);
        
        #if CLOGIC_LOGGING
        [Systems.Logging.IgnoreStackTrace, HideInCallstack]
        #endif
        public static void LogError(string message, Object context) => Log(message, 3, context);
        
        #if CLOGIC_LOGGING
        [Systems.Logging.IgnoreStackTrace, HideInCallstack]
        #endif
        public static void Log(string message, int level) => Log(message, level, null);
        
        #if CLOGIC_LOGGING
        [Systems.Logging.IgnoreStackTrace, HideInCallstack]
        #endif
        public static void Log(string message, int level, Object context) => Log(message, level, context, true, null);
        
        #if CLOGIC_LOGGING
        [Systems.Logging.IgnoreStackTrace, HideInCallstack]
        #endif
        public static void Log(string message, int level, Object context, bool doFileLog, bool? doConsoleLog, bool? showStackTrace = null)
        {
            #if CLOGIC_LOGGING
            Systems.Logging.CLog.Log(message, (Systems.Logging.LogLevel)level, context, doFileLog, doConsoleLog, showStackTrace);
            #else
            switch (level)
            {
                case <= 1:
                    Debug.Log(message, context);
                    break;
                case 2:
                    Debug.LogWarning(message, context);
                    break;
                case >= 3:
                    Debug.LogError(message, context);
                    break;
            }
            #endif
        }
    }
}
