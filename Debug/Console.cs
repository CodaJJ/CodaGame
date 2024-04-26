
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UnityGameFramework
{
    /// <summary>
    /// A simple console for logging messages.
    /// </summary>
    public class Console
    {
        /// <summary>
        /// The log level of the console.
        /// </summary>
        /// <remarks>
        /// <para>The log level is used to filter the log messages.</para>
        /// <para>The log messages with a log type lower than the log level will be ignored.(Verbose &lt; Debug &lt; System &lt; Warning &lt; Error &lt; Crush)</para>
        /// </remarks>
        public static ELogLevel logLevel { get { return instance._m_logLevel; } set { instance._m_logLevel = value; } }
        
        
        /// <summary>
        /// Log a verbose message.
        /// </summary>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <param name="_context">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        public static void LogVerbose(string _system, string _message, Object _context = null)
        {
            instance.Log(ELogType.Verbose, _system, _message, _context);
        }
        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <param name="_context">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        public static void LogDebug(string _system, string _message, Object _context = null)
        {
            instance.Log(ELogType.Debug, _system, _message, _context);
        }
        /// <summary>
        /// Log a system message.
        /// </summary>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <param name="_context">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        public static void LogSystem(string _system, string _message, Object _context = null)
        {
            instance.Log(ELogType.System, _system, _message, _context);
        }
        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <param name="_context">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        public static void LogWarning(string _system, string _message, Object _context = null)
        {
            instance.Log(ELogType.Warning, _system, _message, _context);
        }
        /// <summary>
        /// Log a error message.
        /// </summary>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <param name="_context">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        public static void LogError(string _system, string _message, Object _context = null)
        {
            instance.Log(ELogType.Error, _system, _message, _context);
        }
        /// <summary>
        /// Log a crush message.
        /// </summary>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <param name="_context">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        public static void LogCrush(string _system, string _message, Object _context = null)
        {
            instance.Log(ELogType.Crush, _system, _message, _context);
        }
        

        /// <summary>
        /// Make a log string.
        /// </summary>
        /// <param name="_logType">The type of this log.</param>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <returns>A string includes log type, message, system, and if in Editor it'll also show the caller's name.</returns>
        private static string MakeLogString(ELogType _logType, string _system, string _message)
        {
#if UNITY_EDITOR
            string logString = $"<color=orange>[{_system} {_logType.ToString()}]</color> {_message}";
            MethodBase callerMethod = new StackTrace().GetFrame(3)?.GetMethod();
            if (callerMethod != null)
            {
                string className = callerMethod.DeclaringType?.Name;
                string methodName = callerMethod.Name;
                if (string.IsNullOrEmpty(className))
                    className = "Unknown DeclaringType";
                if (string.IsNullOrEmpty(methodName))
                    methodName = "Unknown Method (anonymous function or global function maybe)";

                logString += $"\n <color=cyan>--- by {className}.{methodName}</color>";
            }
            return logString;
#endif
            return $"[{_system} {_logType.ToString()}] {_message}";
        }


        /// <summary>
        /// The singleton of the console.
        /// </summary>
        [NotNull] internal static Console instance { get { return _g_instance ??= new Console(); } }
        private static Console _g_instance;


        /// <summary>
        /// The log level of the console.
        /// </summary>
        /// <remarks>
        /// <para>The log level is used to filter the log messages.</para>
        /// <para>The log messages with a log type lower than the log level will be ignored.(Verbose &lt; Debug &lt; System &lt; Warning &lt; Error &lt; Crush)</para>
        /// </remarks>
        private ELogLevel _m_logLevel;

        
        private Console()
        {
            // Set the default log level to verbose.
            _m_logLevel = ELogLevel.Verbose;
        }


        /// <summary>
        /// Log a message with a system name.
        /// </summary>
        /// <param name="_logType">The type of this log.</param>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <param name="_context">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        private void Log(ELogType _logType, string _system, string _message, Object _context = null)
        {
            if ((int)_logType < (int)logLevel) 
                return;
            
            string logString = MakeLogString(_logType, _system, _message);
            switch (_logType)
            {
                case ELogType.Crush or ELogType.Error:
                    Debug.LogError(logString, _context);
                    break;
                case ELogType.Warning:
                    Debug.LogWarning(logString, _context);
                    break;
                default:
                    Debug.Log(logString, _context);
                    break;
            }
        }
        
        /// <summary>
        /// The type of the log.
        /// </summary>
        private enum ELogType
        {
            Verbose = 10,
            Debug = 20,
            System = 30,
            Warning = 40,
            Error = 50,
            Crush = 60,
        }
    }

    /// <summary>
    /// The log level of the console.
    /// </summary>
    public enum ELogLevel
    {
        Verbose = 10,
        Debug = 20,
        System = 30,
        Warning = 40,
        Error = 50,
        Crush = 60,
        Nothing = 100,
    }
}