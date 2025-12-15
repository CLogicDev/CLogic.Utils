using UnityEngine;
namespace CLogic.Utils.Logger
{
    [System.Serializable]
    public class ULogger
    {
        private static readonly string NAME_FORMAT = $"[%{nameof(LoggerName)}%]";
        private const LogLevel DEFAULT_LOG_LEVEL = LogLevel.Info;

        [field: SerializeField]
        public bool Enabled { get; set; } = true;
        
        [field: SerializeField]
        public string LoggerName {get; private set;}
        
        [field: SerializeField]
        public LogLevel DefaultLevel {get; private set;}
        
        [field: SerializeField]
        public Object Context {get; private set;}

        public ULogger(string loggerName, Object context) : this(loggerName, DEFAULT_LOG_LEVEL, context) {}

        public ULogger(string loggerName, LogLevel defaultLevel = DEFAULT_LOG_LEVEL) : this(loggerName, defaultLevel, null){}
        
        public ULogger(string loggerName, LogLevel defaultLevel, Object context)
        {
            LoggerName = loggerName;
            DefaultLevel = defaultLevel;
            Context = context;
        }
        
        //QOL Access
        [IgnoreStackTrace, HideInCallstack]
        public void Log(string message) => Log(message, DefaultLevel, Context);
        [IgnoreStackTrace, HideInCallstack]
        public void Log(string message, LogLevel level) => Log(message, level, null, true, true, Context);
        [IgnoreStackTrace, HideInCallstack]
        public void Log(string message, LogLevel level, Object context) => Log(message, level, context, true, true);

        [IgnoreStackTrace, HideInCallstack]
        public void Log(string message, LogLevel level, Object context, bool doFileLog, bool doConsoleLog, bool? showStackTrace = null)
        {
            if(!Enabled)
                return;
            
            string logName = NAME_FORMAT.Replace($"%{nameof(LoggerName)}%", LoggerName) + " ";
            Logging.Log(message.Insert(0, logName), level, context, doFileLog, doConsoleLog, showStackTrace);
        }
    }
}
