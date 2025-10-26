using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;
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

        public static string logFilePath;
        
        private static Queue<string> logQueue = new();
        
        /// <summary>
        /// Whether the logging system has beeen setup
        /// </summary>
        public static bool LogSetup { get; private set; }

        public static event Action OnLogSetup;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Setup()
        {
            if(EnvUtils.EnvironmentSet)
                UpdateSettings();
            else
                EnvUtils.onEnvironmentSet += UpdateSettings;
        }

        
        internal static void UpdateSettings()
        {
            LoggerSettings settings = LoggerSettings.GetOrCreate();
            
            logColors.Clear();
            logColors.Add(LogLevel.Debug, settings.logColors.debug);
            logColors.Add(LogLevel.Info, settings.logColors.info);
            logColors.Add(LogLevel.Warning, settings.logColors.warn);
            logColors.Add(LogLevel.Error, settings.logColors.error);
            logColors.Add(LogLevel.Fatal, settings.logColors.fatal);
            
            logFilePath = Environment.ExpandEnvironmentVariables(settings.logFilePath);

            LogSetup = true;
            OnLogSetup?.Invoke();
        }
        
        [IgnoreStackTrace]
        public static void Log(string message, LogLevel level, Object context, bool doFileLog, bool doConsoleLog)
        {
            StringBuilder builder = new();
            builder.AppendLine(message);

            builder.Insert(0, $"[{Enum.GetName(typeof(LogLevel), level)}] "); // Log Level Prefix

            if(doFileLog)
            {
                StringBuilder fileLogBuilder = new(builder.ToString());
                if(level >= LogLevel.Error)
                {
                    string trace = StackTraceUtils.GetFilteredStackTrace();
                    string indentedTrace = string.Join("\n", trace.Split("\n").Select(line => "    " + line.TrimEnd()));

                    fileLogBuilder.Append(indentedTrace);
                }

                FileLog(fileLogBuilder.ToString()); // File log should not include coloring
            }

            builder.Insert(0, GetLogColor(level)); // Log Coloring

            if(doConsoleLog)
            {
                UnityLogger.SetArrivingLog(); //Prevents dual logging
                ULog(builder + " passed through logger", level, context);
            }
        }
        
        public static void Log(string message, LogLevel level) => Log(message, level, null, true, true);
        public static void Log(string message, LogLevel level, Object context) => Log(message, level, context, true, true);

        private static string GetLogColor(LogLevel level)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(logColors[level])}>";
        }

        /// <summary>
        /// Logs to the unity console
        /// </summary>
        private static void ULog(string finalLog, LogLevel level, Object context)
        {
            switch (level)
            {
                case <= LogLevel.Info:
                    Debug.Log(finalLog, context);
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(finalLog, context);
                    break;
                case >= LogLevel.Error:
                    Debug.LogError(finalLog, context);
                    break;
            }
        }

        private static void FileLog(string finalLog)
        {
            FileStream fs;
            try
            {
                fs = GetFileStream();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                logQueue.Enqueue(finalLog);
                return;
            }

            fs.Seek(0, SeekOrigin.End);
            using StreamWriter sw = new (fs);

            foreach (string line in logQueue)
                sw.WriteLine(line);

            logQueue.Clear();
            
            sw.WriteLine(finalLog);
            sw.Flush();
        }

        private static FileStream GetFileStream()
        {
            string dirName = Path.GetDirectoryName(logFilePath);

            if(!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            return File.Open(logFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreStackTraceAttribute : Attribute { }

    public static class StackTraceUtils
    {
        [IgnoreStackTrace]
        public static string GetFilteredStackTrace()
        {
            StackTrace trace = new (true);
            StackFrame[] frames = trace.GetFrames();
            if (frames == null)
                return string.Empty;

            StringBuilder sb = new ();

            foreach (StackFrame frame in frames)
            {
                MethodBase method = frame.GetMethod();
                if (method.GetCustomAttribute<IgnoreStackTraceAttribute>() != null)
                    continue;

                Type type = method.DeclaringType;
                if (type == null)
                    continue;

                string methodName = method.Name;
                string typeName = type.FullName.Replace('+', '.'); // nested class fix
                
                string paramList = string.Join(",", method.GetParameters()
                    .Select(p => p.ParameterType.FullName));
                sb.AppendLine($"{typeName}:{methodName}({paramList})");
            }

            return sb.ToString();
        }
    }
}
