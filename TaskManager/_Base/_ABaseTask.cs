// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame.Base
{
    /// <summary>
    /// A base class for all tasks.
    /// </summary>
    public abstract class _ABaseTask
    {
        // Task name
        private readonly string _m_name;
        // Running type
        private readonly UpdateType _m_runType;
        // Is task running?
        private bool _m_isRunning;
        
        
        internal _ABaseTask(string _name, UpdateType _runType)
        {
            _m_name = _name;
            _m_runType = _runType;
            _m_isRunning = false;
        }
        

        /// <summary>
        /// Is task running.
        /// </summary>
        public bool isRunning { get { return _m_isRunning; } }
        /// <summary>
        /// The name of this task.
        /// </summary>
        /// <remarks>
        /// Usually displayed for debug.
        /// </remarks>
        public string name { get { return _m_name; } }
        

        /// <summary>
        /// Start this task.
        /// </summary>
        public void Run()
        {
            if (_m_isRunning)
            {
                Console.LogWarning(SystemNames.TaskSystem, _m_name, "The task is already running.");
                return;
            }

            _m_isRunning = true;

            switch (_m_runType)
            {
                case UpdateType.Update:
                    AddToUpdateTaskSystem();
                    break;
                case UpdateType.FixedUpdate:
                    AddToFixedUpdateTaskSystem();
                    break;
                case UpdateType.LateUpdate:
                    AddToLateUpdateTaskSystem();
                    break;
                default:
                    Console.LogError(SystemNames.TaskSystem, _m_name, $"Unsupported task run type {_m_runType}.");
                    break;
            }

            OnInternalRun();
            OnRun();
        }
        /// <summary>
        /// Stop this task.
        /// </summary>
        public void Stop()
        {
            if (!_m_isRunning)
            {
                Console.LogWarning(SystemNames.TaskSystem, _m_name, "The task hasn't started running yet, you are trying to stop it.");
                return;
            }

            _m_isRunning = false;

            OnStop();
            OnInternalStop();

            switch (_m_runType)
            {
                case UpdateType.Update:
                    RemoveFromUpdateTaskSystem();
                    break;
                case UpdateType.FixedUpdate:
                    RemoveFromFixedUpdateTaskSystem();
                    break;
                case UpdateType.LateUpdate:
                    RemoveFromLateUpdateTaskSystem();
                    break;
                default:
                    Console.LogError(SystemNames.TaskSystem, _m_name, $"Unsupported task run type {_m_runType}.");
                    break;
            }
        }
        /// <summary>
        /// Stop this task by system.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Called by task system, some tasks like delay tasks will be stopped by system,
        /// at the same time, the system will removed them and invoke this stop function.
        /// So, there aren't any remove process in this function.
        /// </para>
        /// </remarks>
        internal void StopBySystem()
        {
            if (!_m_isRunning)
            {
                Console.LogWarning(SystemNames.TaskSystem, _m_name, "The task hasn't started running yet, you are trying to stop it.");
                return;
            }

            _m_isRunning = false;

            OnStop();
            OnInternalStop();
        }


        /// <summary>
        /// Do something on task run.
        /// </summary>
        protected abstract void OnRun();
        /// <summary>
        /// Do something on task stop.
        /// </summary>
        protected abstract void OnStop();

        
        private protected abstract void AddToUpdateTaskSystem();
        private protected abstract void AddToFixedUpdateTaskSystem();
        private protected abstract void AddToLateUpdateTaskSystem();
        private protected abstract void RemoveFromUpdateTaskSystem();
        private protected abstract void RemoveFromFixedUpdateTaskSystem();
        private protected abstract void RemoveFromLateUpdateTaskSystem();

        private protected abstract void OnInternalRun();
        private protected abstract void OnInternalStop();
    }
}