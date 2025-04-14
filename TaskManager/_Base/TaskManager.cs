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
        [NotNull] internal static TaskManager instance
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
                            Console.LogCrush(SystemNames.TaskSystem, "Initialize TaskSystem failed, Failed to add TaskMonoBehaviour component.");
                        else
                            Console.LogSystem(SystemNames.TaskSystem, "Initialize TaskManager success.");

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


        internal TaskManager()
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


        internal void AddUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateContinuousTaskContainer);
            try
            {
                _m_updateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateContinuousTaskContainer);
            }
        }
        internal void AddUnscaledTimeUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddUnscaledTimeUpdateContinuousTask failed, Task is null.");
                return;    
            }
            
            Monitor.Enter(_m_unscaledTimeUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddUnscaledTimeUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeUpdateContinuousTaskContainer);
            }
        }
        internal void AddFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddFixedUpdateContinuousTask failed, Task is null.");    
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateContinuousTaskContainer);
            try
            {
                _m_fixedUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddFixedUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateContinuousTaskContainer);
            }
        }
        internal void AddUnscaledTimeFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddUnscaledTimeFixedUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeFixedUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddUnscaledTimeFixedUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
            }
        }
        internal void AddLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddLateUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateContinuousTaskContainer);
            try
            {
                _m_lateUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddLateUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateContinuousTaskContainer);
            }
        }
        internal void AddUnscaledTimeLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddUnscaledTimeLateUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeLateUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeLateUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddUnscaledTimeLateUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeLateUpdateContinuousTaskContainer);
            }
        }
        internal void RemoveUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateContinuousTaskContainer);
            try
            {
                _m_updateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateContinuousTaskContainer);
            }
        }
        internal void RemoveUnscaledTimeUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveUnscaledTimeUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveUnscaledTimeUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeUpdateContinuousTaskContainer);
            }
        }
        internal void RemoveFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveFixedUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateContinuousTaskContainer);
            try
            {
                _m_fixedUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveFixedUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateContinuousTaskContainer);
            }
        }
        internal void RemoveUnscaledTimeFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveUnscaledTimeFixedUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeFixedUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveUnscaledTimeFixedUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
            }
        }
        internal void RemoveLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveLateUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateContinuousTaskContainer);
            try
            {
                _m_lateUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveLateUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateContinuousTaskContainer);
            }
        }
        internal void RemoveUnscaledTimeLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveUnscaledTimeLateUpdateContinuousTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeLateUpdateContinuousTaskContainer);
            try
            {
                _m_unscaledTimeLateUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveUnscaledTimeLateUpdateContinuousTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeLateUpdateContinuousTaskContainer);
            }
        }
        
        internal void AddUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateFrameDelayTaskContainer);
            try
            {
                _m_updateFrameDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateFrameDelayTaskContainer);
            }
        }
        internal void AddFixedUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddFixedUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateFrameDelayTaskContainer);
            try
            {
                _m_fixedUpdateFrameDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddFixedUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateFrameDelayTaskContainer);
            }
        }
        internal void AddLateUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddLateUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateFrameDelayTaskContainer);
            try
            {
                _m_lateUpdateFrameDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddLateUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateFrameDelayTaskContainer);
            }
        }
        internal void RemoveUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateFrameDelayTaskContainer);
            try
            {
                _m_updateFrameDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateFrameDelayTaskContainer);
            }
        }
        internal void RemoveFixedUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveFixedUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateFrameDelayTaskContainer);
            try
            {
                _m_fixedUpdateFrameDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveFixedUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateFrameDelayTaskContainer);
            }
        }
        internal void RemoveLateUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveLateUpdateFrameDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateFrameDelayTaskContainer);
            try
            {
                _m_lateUpdateFrameDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveLateUpdateFrameDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateFrameDelayTaskContainer);
            }
        }
        
        internal void AddUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateTimeDelayTaskContainer);
            try
            {
                _m_updateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateTimeDelayTaskContainer);
            }
        }
        internal void AddUnscaledTimeUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddUnscaledTimeUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddUnscaledTimeUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeUpdateTimeDelayTaskContainer);
            }
        }
        internal void AddFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateTimeDelayTaskContainer);
            try
            {
                _m_fixedUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddFixedUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateTimeDelayTaskContainer);
            }
        }
        internal void AddUnscaledTimeFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddUnscaledTimeFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeFixedUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddUnscaledTimeFixedUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
            }
        }
        internal void AddLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateTimeDelayTaskContainer);
            try
            {
                _m_lateUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddLateUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateTimeDelayTaskContainer);
            }
        }
        internal void AddUnscaledTimeLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "AddUnscaledTimeLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeLateUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "AddUnscaledTimeLateUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
            }
        }
        internal void RemoveUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_updateTimeDelayTaskContainer);
            try
            {
                _m_updateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_updateTimeDelayTaskContainer);
            }
        }
        internal void RemoveUnscaledTimeUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveUnscaledTimeUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveUnscaledTimeUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeUpdateTimeDelayTaskContainer);
            }
        }
        internal void RemoveFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_fixedUpdateTimeDelayTaskContainer);
            try
            {
                _m_fixedUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveFixedUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_fixedUpdateTimeDelayTaskContainer);
            }
        }
        internal void RemoveUnscaledTimeFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveUnscaledTimeFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeFixedUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveUnscaledTimeFixedUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
            }
        }
        internal void RemoveLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_lateUpdateTimeDelayTaskContainer);
            try
            {
                _m_lateUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveLateUpdateTimeDelayTask success.");
            }
            finally
            {
                Monitor.Exit(_m_lateUpdateTimeDelayTaskContainer);
            }
        }
        internal void RemoveUnscaledTimeLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.TaskSystem, "RemoveUnscaledTimeLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }
            
            Monitor.Enter(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
            try
            {
                _m_unscaledTimeLateUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.TaskSystem, _task.name, "RemoveUnscaledTimeLateUpdateTimeDelayTask success.");
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
                    if (!_m_updateContinuousTaskContainer.DealTasks() &&
                        !_m_unscaledTimeUpdateContinuousTaskContainer.DealTasks() &&
                        !_m_updateFrameDelayTaskContainer.DealTasks() &&
                        !_m_updateTimeDelayTaskContainer.DealTasks() &&
                        !_m_unscaledTimeUpdateTimeDelayTaskContainer.DealTasks())
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
                    if (!_m_fixedUpdateContinuousTaskContainer.DealTasks() &&
                        !_m_unscaledTimeFixedUpdateContinuousTaskContainer.DealTasks() &&
                        !_m_fixedUpdateFrameDelayTaskContainer.DealTasks() &&
                        !_m_fixedUpdateTimeDelayTaskContainer.DealTasks() &&
                        !_m_unscaledTimeFixedUpdateTimeDelayTaskContainer.DealTasks())
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
                    if (!_m_lateUpdateContinuousTaskContainer.DealTasks() &&
                        !_m_unscaledTimeLateUpdateContinuousTaskContainer.DealTasks() &&
                        !_m_lateUpdateFrameDelayTaskContainer.DealTasks() &&
                        !_m_lateUpdateTimeDelayTaskContainer.DealTasks() &&
                        !_m_unscaledTimeLateUpdateTimeDelayTaskContainer.DealTasks())
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