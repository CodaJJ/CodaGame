// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace CodaGame
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
        /// <param name="_contextObj">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        [HideInCallstack, Conditional("UNITY_EDITOR")]
        public static void LogVerbose(string _system, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.Verbose, _system, string.Empty, _message, _contextObj);
        }
        /// <summary>
        /// Log a verbose message.
        /// </summary>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_contextName">A custom name that relates to this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <param name="_contextObj">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        [HideInCallstack, Conditional("UNITY_EDITOR")]
        public static void LogVerbose(string _system, string _contextName, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.Verbose, _system, _contextName, _message, _contextObj);
        }
        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,UnityEngine.Object)"/>
        [HideInCallstack, Conditional("UNITY_EDITOR")]
        public static void LogDebug(string _system, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.Debug, _system, string.Empty, _message, _contextObj);
        }
        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,string,UnityEngine.Object)"/>
        [HideInCallstack, Conditional("UNITY_EDITOR")]
        public static void LogDebug(string _system, string _contextName, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.Debug, _system, _contextName, _message, _contextObj);
        }
        /// <summary>
        /// Log a system message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,UnityEngine.Object)"/>
        [HideInCallstack]
        public static void LogSystem(string _system, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.System, _system, string.Empty, _message, _contextObj);
        }
        /// <summary>
        /// Log a system message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,string,UnityEngine.Object)"/>
        [HideInCallstack]
        public static void LogSystem(string _system, string _contextName, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.System, _system, _contextName, _message, _contextObj);
        }
        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,UnityEngine.Object)"/>
        [HideInCallstack]
        public static void LogWarning(string _system, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.Warning, _system, string.Empty, _message, _contextObj);
        }
        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,string,UnityEngine.Object)"/>
        [HideInCallstack]
        public static void LogWarning(string _system, string _contextName, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.Warning, _system, _contextName, _message, _contextObj);
        }
        /// <summary>
        /// Log a error message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,UnityEngine.Object)"/>
        [HideInCallstack]
        public static void LogError(string _system, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.Error, _system, string.Empty, _message, _contextObj);
        }
        /// <summary>
        /// Log a error message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,string,UnityEngine.Object)"/>
        [HideInCallstack]
        public static void LogError(string _system, string _contextName, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.Error, _system, _contextName, _message, _contextObj);
        }
        /// <summary>
        /// Log a crush message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,UnityEngine.Object)"/>
        [HideInCallstack, DoesNotReturn]
        public static void LogCrush(string _system, string _message, Object _context = null)
        {
            instance.Log(ELogType.Crush, _system, string.Empty, _message, _context);
        }
        /// <summary>
        /// Log a crush message.
        /// </summary>
        /// <inheritdoc cref="LogVerbose(string,string,string,UnityEngine.Object)"/>
        [HideInCallstack, DoesNotReturn]
        public static void LogCrush(string _system, string _contextName, string _message, Object _contextObj = null)
        {
            instance.Log(ELogType.Crush, _system, _contextName, _message, _contextObj);
        }
        

        /// <summary>
        /// Make a log string.
        /// </summary>
        /// <param name="_logType">The type of this log.</param>
        /// <param name="_system">The system that logs this message.</param>
        /// <param name="_contextName">A custom name that relates to this message.</param>
        /// <param name="_message">The message you want to log.</param>
        /// <returns>A string includes log type, message, system, and if in Editor it'll also show the caller's name.</returns>
        [HideInCallstack]
        private static string MakeLogString(ELogType _logType, string _system, string _contextName, string _message)
        {
#if UNITY_EDITOR
            string logString = string.IsNullOrEmpty(_contextName) ? 
                $"<color=orange>[{_system} {_logType}]</color> : {_message}" : 
                $"<color=orange>[{_system} {_logType}]</color> -- {_contextName} -- : {_message}";
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
#else
            return string.IsNullOrEmpty(_contextName) ? 
                $"[{_system} {_logType}] : {_message}" : 
                $"[{_system} {_logType}] -- {_contextName} -- : {_message}";
#endif
        }


        /// <summary>
        /// The singleton of the console.
        /// </summary>
        [JetBrains.Annotations.NotNull]
        internal static Console instance
        {
            get
            {
                if (_g_instance != null)
                    return _g_instance;

                lock (_g_lock) _g_instance ??= new Console();
                return _g_instance;
            }
        }
        private static Console _g_instance;
        [JetBrains.Annotations.NotNull] private static readonly object _g_lock = new object();


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
        /// <param name="_contextName">A custom name that relates to this message.</param>
        /// <param name="_contextObj">A object reference that relates to this message. Clicking on this message will highlight the object.</param>
        [HideInCallstack]
        private void Log(ELogType _logType, string _system, string _contextName, string _message, Object _contextObj = null)
        {
            if ((int)_logType < (int)_m_logLevel) 
                return;
            
            string logString = MakeLogString(_logType, _system, _contextName, _message);
            switch (_logType)
            {
                case ELogType.Crush:
                    Debug.LogError(logString, _contextObj);
                    throw new Exception("fatal error: \n" + logString);
                case ELogType.Error:
                    Debug.LogError(logString, _contextObj);
                    break;
                case ELogType.Warning:
                    Debug.LogWarning(logString, _contextObj);
                    break;
                default:
                    Debug.Log(logString, _contextObj);
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