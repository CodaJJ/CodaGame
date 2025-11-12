// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Threading;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// A MonoBehaviour for running tasks.
    /// </summary>
    internal class TaskManager : MonoBehaviour
    {
        /// <summary>
        /// The singleton of TaskManager.
        /// </summary>
        [NotNull] public static TaskManager instance
        {
            get
            {
                if (_g_instance != null)
                    return _g_instance;

                lock (_g_lock)
                {
                    if (_g_instance == null)
                    {
                        GameObject go = new GameObject("TaskManager");
                        DontDestroyOnLoad(go);
                        TaskManager manager = go.AddComponent<TaskManager>();

                        if (manager == null)
                            Console.LogCrush(SystemNames.Task, "Initialize TaskSystem failed, Failed to add TaskMonoBehaviour component.");
                        else
                            Console.LogSystem(SystemNames.Task, "Initialize TaskManager success.");

                        // ReSharper disable once PossibleMultipleWriteAccessInDoubleCheckLocking
                        _g_instance = manager;
                    }
                }

                // ReSharper disable once AssignNullToNotNullAttribute
                return _g_instance;
            }
        }
        private static TaskManager _g_instance;
        [NotNull] private static readonly object _g_lock = new object();


        [NotNull] private readonly DeltaTimeContinuousTaskContainer _m_updateContinuousTaskContainer;
        [NotNull] private readonly UnscaledDeltaTimeContinuousTaskContainer _m_unscaledTimeUpdateContinuousTaskContainer;
        [NotNull] private readonly FixedDeltaTimeContinuousTaskContainer _m_fixedUpdateContinuousTaskContainer;
        [NotNull] private readonly FixedUnscaledDeltaTimeContinuousTaskContainer _m_unscaledTimeFixedUpdateContinuousTaskContainer;
        [NotNull] private readonly DeltaTimeContinuousTaskContainer _m_lateUpdateContinuousTaskContainer;
        [NotNull] private readonly UnscaledDeltaTimeContinuousTaskContainer _m_unscaledTimeLateUpdateContinuousTaskContainer;
        
        [NotNull] private readonly FrameDelayTaskContainer _m_updateFrameDelayTaskContainer;
        [NotNull] private readonly FrameDelayTaskContainer _m_fixedUpdateFrameDelayTaskContainer;
        [NotNull] private readonly FrameDelayTaskContainer _m_lateUpdateFrameDelayTaskContainer;
        
        [NotNull] private readonly TimeDelayTaskContainer _m_updateTimeDelayTaskContainer;
        [NotNull] private readonly UnscaledTimeDelayTaskContainer _m_unscaledTimeUpdateTimeDelayTaskContainer;
        [NotNull] private readonly FixedTimeDelayTaskContainer _m_fixedUpdateTimeDelayTaskContainer;
        [NotNull] private readonly FixedUnscaledTimeDelayTaskContainer _m_unscaledTimeFixedUpdateTimeDelayTaskContainer;
        [NotNull] private readonly TimeDelayTaskContainer _m_lateUpdateTimeDelayTaskContainer;
        [NotNull] private readonly UnscaledTimeDelayTaskContainer _m_unscaledTimeLateUpdateTimeDelayTaskContainer;


        private TaskManager()
        {
            _m_updateContinuousTaskContainer = new DeltaTimeContinuousTaskContainer();
            _m_unscaledTimeUpdateContinuousTaskContainer = new UnscaledDeltaTimeContinuousTaskContainer();
            _m_fixedUpdateContinuousTaskContainer = new FixedDeltaTimeContinuousTaskContainer();
            _m_unscaledTimeFixedUpdateContinuousTaskContainer = new FixedUnscaledDeltaTimeContinuousTaskContainer();
            _m_lateUpdateContinuousTaskContainer = new DeltaTimeContinuousTaskContainer();
            _m_unscaledTimeLateUpdateContinuousTaskContainer = new UnscaledDeltaTimeContinuousTaskContainer();
            
            _m_updateFrameDelayTaskContainer = new FrameDelayTaskContainer();
            _m_fixedUpdateFrameDelayTaskContainer = new FrameDelayTaskContainer();
            _m_lateUpdateFrameDelayTaskContainer = new FrameDelayTaskContainer();
            
            _m_updateTimeDelayTaskContainer = new TimeDelayTaskContainer();
            _m_unscaledTimeUpdateTimeDelayTaskContainer = new UnscaledTimeDelayTaskContainer();
            _m_fixedUpdateTimeDelayTaskContainer = new FixedTimeDelayTaskContainer();
            _m_unscaledTimeFixedUpdateTimeDelayTaskContainer = new FixedUnscaledTimeDelayTaskContainer();
            _m_lateUpdateTimeDelayTaskContainer = new TimeDelayTaskContainer();
            _m_unscaledTimeLateUpdateTimeDelayTaskContainer = new UnscaledTimeDelayTaskContainer();
        }


        public void AddUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateContinuousTaskContainer);
            try
            {
                _m_updateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateContinuousTaskContainer);
            }
        }
        public void AddUnscaledTimeUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeUpdateContinuousTask failed, Task is null.");
                return;    
            }
            
            Monitor.Enter(_m_unscaledTimeUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeUpdateContinuousTaskContainer);
            }
        }
        public void AddFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddFixedUpdateContinuousTask failed, Task is null.");    
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateContinuousTaskContainer);
            try
            {
                _m_fixedUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddFixedUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateContinuousTaskContainer);
            }
        }
        public void AddUnscaledTimeFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeFixedUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeFixedUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeFixedUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
            }
        }
        public void AddLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddLateUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateContinuousTaskContainer);
            try
            {
                _m_lateUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddLateUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateContinuousTaskContainer);
            }
        }
        public void AddUnscaledTimeLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeLateUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeLateUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeLateUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeLateUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeLateUpdateContinuousTaskContainer);
            }
        }
        public void RemoveUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateContinuousTaskContainer);
            try
            {
                _m_updateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateContinuousTaskContainer);
            }
        }
        public void RemoveUnscaledTimeUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeUpdateContinuousTaskContainer);
            }
        }
        public void RemoveFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveFixedUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateContinuousTaskContainer);
            try
            {
                _m_fixedUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveFixedUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateContinuousTaskContainer);
            }
        }
        public void RemoveUnscaledTimeFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeFixedUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeFixedUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeFixedUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
            }
        }
        public void RemoveLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveLateUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateContinuousTaskContainer);
            try
            {
                _m_lateUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveLateUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateContinuousTaskContainer);
            }
        }
        public void RemoveUnscaledTimeLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeLateUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeLateUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeLateUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeLateUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeLateUpdateContinuousTaskContainer);
            }
        }
        
        public void AddUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateFrameDelayTaskContainer);
            try
            {
                _m_updateFrameDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateFrameDelayTaskContainer);
            }
        }
        public void AddFixedUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddFixedUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateFrameDelayTaskContainer);
            try
            {
                _m_fixedUpdateFrameDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddFixedUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateFrameDelayTaskContainer);
            }
        }
        public void AddLateUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddLateUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateFrameDelayTaskContainer);
            try
            {
                _m_lateUpdateFrameDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddLateUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateFrameDelayTaskContainer);
            }
        }
        public void RemoveUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateFrameDelayTaskContainer);
            try
            {
                _m_updateFrameDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateFrameDelayTaskContainer);
            }
        }
        public void RemoveFixedUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveFixedUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateFrameDelayTaskContainer);
            try
            {
                _m_fixedUpdateFrameDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveFixedUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateFrameDelayTaskContainer);
            }
        }
        public void RemoveLateUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveLateUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateFrameDelayTaskContainer);
            try
            {
                _m_lateUpdateFrameDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveLateUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateFrameDelayTaskContainer);
            }
        }
        
        public void AddUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateTimeDelayTaskContainer);
            try
            {
                _m_updateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateTimeDelayTaskContainer);
            }
        }
        public void AddUnscaledTimeUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeUpdateTimeDelayTaskContainer);
            }
        }
        public void AddFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateTimeDelayTaskContainer);
            try
            {
                _m_fixedUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddFixedUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateTimeDelayTaskContainer);
            }
        }
        public void AddUnscaledTimeFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeFixedUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeFixedUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
            }
        }
        public void AddLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateTimeDelayTaskContainer);
            try
            {
                _m_lateUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddLateUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateTimeDelayTaskContainer);
            }
        }
        public void AddUnscaledTimeLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeLateUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeLateUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
            }
        }
        public void RemoveUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateTimeDelayTaskContainer);
            try
            {
                _m_updateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateTimeDelayTaskContainer);
            }
        }
        public void RemoveUnscaledTimeUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeUpdateTimeDelayTaskContainer);
            }
        }
        public void RemoveFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateTimeDelayTaskContainer);
            try
            {
                _m_fixedUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveFixedUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateTimeDelayTaskContainer);
            }
        }
        public void RemoveUnscaledTimeFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeFixedUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeFixedUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
            }
        }
        public void RemoveLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateTimeDelayTaskContainer);
            try
            {
                _m_lateUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveLateUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateTimeDelayTaskContainer);
            }
        }
        public void RemoveUnscaledTimeLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeLateUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeLateUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
            }
        }


        private void Update()
        {
            Monitor.Enter(_m_updateContinuousTaskContainer);
            Monitor.Enter(_m_unscaledTimeUpdateContinuousTaskContainer);
            Monitor.Enter(_m_updateFrameDelayTaskContainer);
            Monitor.Enter(_m_updateTimeDelayTaskContainer);
            Monitor.Enter(_m_unscaledTimeUpdateTimeDelayTaskContainer);
            
            try
            {
                while (true)
                {
                    if (!_m_updateContinuousTaskContainer.ExecuteTasks() &&
                        !_m_unscaledTimeUpdateContinuousTaskContainer.ExecuteTasks() &&
                        !_m_updateFrameDelayTaskContainer.ExecuteTasks() &&
                        !_m_updateTimeDelayTaskContainer.ExecuteTasks() &&
                        !_m_unscaledTimeUpdateTimeDelayTaskContainer.ExecuteTasks())
                        break;
                }
            }
            finally
            {
                _m_unscaledTimeUpdateTimeDelayTaskContainer.NextFrame();
                Monitor.Exit(_m_unscaledTimeUpdateTimeDelayTaskContainer);
                _m_updateTimeDelayTaskContainer.NextFrame();
                Monitor.Exit(_m_updateTimeDelayTaskContainer);
                _m_updateFrameDelayTaskContainer.NextFrame();
                Monitor.Exit(_m_updateFrameDelayTaskContainer);
                _m_unscaledTimeUpdateContinuousTaskContainer.NextFrame();
                Monitor.Exit(_m_unscaledTimeUpdateContinuousTaskContainer);
                _m_updateContinuousTaskContainer.NextFrame();
                Monitor.Exit(_m_updateContinuousTaskContainer);
            }
        }
        private void FixedUpdate()
        {
            Monitor.Enter(_m_fixedUpdateContinuousTaskContainer);
            Monitor.Enter(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
            Monitor.Enter(_m_fixedUpdateFrameDelayTaskContainer);
            Monitor.Enter(_m_fixedUpdateTimeDelayTaskContainer);
            Monitor.Enter(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);

            try
            {
                while (true)
                {
                    if (!_m_fixedUpdateContinuousTaskContainer.ExecuteTasks() &&
                        !_m_unscaledTimeFixedUpdateContinuousTaskContainer.ExecuteTasks() &&
                        !_m_fixedUpdateFrameDelayTaskContainer.ExecuteTasks() &&
                        !_m_fixedUpdateTimeDelayTaskContainer.ExecuteTasks() &&
                        !_m_unscaledTimeFixedUpdateTimeDelayTaskContainer.ExecuteTasks())
                        break;
                }
            }
            finally
            {
                _m_unscaledTimeFixedUpdateTimeDelayTaskContainer.NextFrame();
                Monitor.Exit(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
                _m_fixedUpdateTimeDelayTaskContainer.NextFrame();
                Monitor.Exit(_m_fixedUpdateTimeDelayTaskContainer);
                _m_fixedUpdateFrameDelayTaskContainer.NextFrame();
                Monitor.Exit(_m_fixedUpdateFrameDelayTaskContainer);
                _m_unscaledTimeFixedUpdateContinuousTaskContainer.NextFrame();
                Monitor.Exit(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
                _m_fixedUpdateContinuousTaskContainer.NextFrame();
                Monitor.Exit(_m_fixedUpdateContinuousTaskContainer);
            }
        }
        private void LateUpdate()
        {
            Monitor.Enter(_m_lateUpdateContinuousTaskContainer);
            Monitor.Enter(_m_unscaledTimeLateUpdateContinuousTaskContainer);
            Monitor.Enter(_m_lateUpdateFrameDelayTaskContainer);
            Monitor.Enter(_m_lateUpdateTimeDelayTaskContainer);
            Monitor.Enter(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
            
            try
            {
                while (true)
                {
                    if (!_m_lateUpdateContinuousTaskContainer.ExecuteTasks() &&
                        !_m_unscaledTimeLateUpdateContinuousTaskContainer.ExecuteTasks() &&
                        !_m_lateUpdateFrameDelayTaskContainer.ExecuteTasks() &&
                        !_m_lateUpdateTimeDelayTaskContainer.ExecuteTasks() &&
                        !_m_unscaledTimeLateUpdateTimeDelayTaskContainer.ExecuteTasks())
                        break;
                }
            }
            finally
            {
                _m_unscaledTimeLateUpdateTimeDelayTaskContainer.NextFrame();
                Monitor.Exit(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
                _m_lateUpdateTimeDelayTaskContainer.NextFrame();
                Monitor.Exit(_m_lateUpdateTimeDelayTaskContainer);
                _m_lateUpdateFrameDelayTaskContainer.NextFrame();
                Monitor.Exit(_m_lateUpdateFrameDelayTaskContainer);
                _m_unscaledTimeLateUpdateContinuousTaskContainer.NextFrame();
                Monitor.Exit(_m_unscaledTimeLateUpdateContinuousTaskContainer);
                _m_lateUpdateContinuousTaskContainer.NextFrame();
                Monitor.Exit(_m_lateUpdateContinuousTaskContainer);
            }
        }
    }
}