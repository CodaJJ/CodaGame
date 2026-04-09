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
    internal class TaskManager : MonoBehaviour, _ITaskManager
    {
        /// <summary>
        /// The singleton of TaskManager.
        /// </summary>
        [NotNull] public static _ITaskManager instance
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
        private static _ITaskManager _g_instance;
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

            lock (_m_updateContinuousTaskContainer)
            {
                _m_updateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUpdateContinuousTask success.");
            }
        }
        public void AddUnscaledTimeUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeUpdateContinuousTaskContainer)
            {
                _m_unscaledTimeUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeUpdateContinuousTask success.");
            }
        }
        public void AddFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddFixedUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_fixedUpdateContinuousTaskContainer)
            {
                _m_fixedUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddFixedUpdateContinuousTask success.");
            }
        }
        public void AddUnscaledTimeFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeFixedUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeFixedUpdateContinuousTaskContainer)
            {
                _m_unscaledTimeFixedUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeFixedUpdateContinuousTask success.");
            }
        }
        public void AddLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddLateUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_lateUpdateContinuousTaskContainer)
            {
                _m_lateUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddLateUpdateContinuousTask success.");
            }
        }
        public void AddUnscaledTimeLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeLateUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeLateUpdateContinuousTaskContainer)
            {
                _m_unscaledTimeLateUpdateContinuousTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeLateUpdateContinuousTask success.");
            }
        }
        public void RemoveUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_updateContinuousTaskContainer)
            {
                _m_updateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUpdateContinuousTask success.");
            }
        }
        public void RemoveUnscaledTimeUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeUpdateContinuousTaskContainer)
            {
                _m_unscaledTimeUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeUpdateContinuousTask success.");
            }
        }
        public void RemoveFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveFixedUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_fixedUpdateContinuousTaskContainer)
            {
                _m_fixedUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveFixedUpdateContinuousTask success.");
            }
        }
        public void RemoveUnscaledTimeFixedUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeFixedUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeFixedUpdateContinuousTaskContainer)
            {
                _m_unscaledTimeFixedUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeFixedUpdateContinuousTask success.");
            }
        }
        public void RemoveLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveLateUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_lateUpdateContinuousTaskContainer)
            {
                _m_lateUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveLateUpdateContinuousTask success.");
            }
        }
        public void RemoveUnscaledTimeLateUpdateContinuousTask(_AContinuousTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeLateUpdateContinuousTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeLateUpdateContinuousTaskContainer)
            {
                _m_unscaledTimeLateUpdateContinuousTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeLateUpdateContinuousTask success.");
            }
        }

        public void AddUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUpdateFrameDelayTask failed, Task is null.");
                return;
            }

            lock (_m_updateFrameDelayTaskContainer)
            {
                _m_updateFrameDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUpdateFrameDelayTask success.");
            }
        }
        public void AddFixedUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddFixedUpdateFrameDelayTask failed, Task is null.");
                return;
            }

            lock (_m_fixedUpdateFrameDelayTaskContainer)
            {
                _m_fixedUpdateFrameDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddFixedUpdateFrameDelayTask success.");
            }
        }
        public void AddLateUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddLateUpdateFrameDelayTask failed, Task is null.");
                return;
            }

            lock (_m_lateUpdateFrameDelayTaskContainer)
            {
                _m_lateUpdateFrameDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddLateUpdateFrameDelayTask success.");
            }
        }
        public void RemoveUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUpdateFrameDelayTask failed, Task is null.");
                return;
            }

            lock (_m_updateFrameDelayTaskContainer)
            {
                _m_updateFrameDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUpdateFrameDelayTask success.");
            }
        }
        public void RemoveFixedUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveFixedUpdateFrameDelayTask failed, Task is null.");
                return;
            }

            lock (_m_fixedUpdateFrameDelayTaskContainer)
            {
                _m_fixedUpdateFrameDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveFixedUpdateFrameDelayTask success.");
            }
        }
        public void RemoveLateUpdateFrameDelayTask(_AFrameDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveLateUpdateFrameDelayTask failed, Task is null.");
                return;
            }

            lock (_m_lateUpdateFrameDelayTaskContainer)
            {
                _m_lateUpdateFrameDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveLateUpdateFrameDelayTask success.");
            }
        }

        public void AddUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_updateTimeDelayTaskContainer)
            {
                _m_updateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUpdateTimeDelayTask success.");
            }
        }
        public void AddUnscaledTimeUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeUpdateTimeDelayTaskContainer)
            {
                _m_unscaledTimeUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeUpdateTimeDelayTask success.");
            }
        }
        public void AddFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_fixedUpdateTimeDelayTaskContainer)
            {
                _m_fixedUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddFixedUpdateTimeDelayTask success.");
            }
        }
        public void AddUnscaledTimeFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeFixedUpdateTimeDelayTaskContainer)
            {
                _m_unscaledTimeFixedUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeFixedUpdateTimeDelayTask success.");
            }
        }
        public void AddLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_lateUpdateTimeDelayTaskContainer)
            {
                _m_lateUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddLateUpdateTimeDelayTask success.");
            }
        }
        public void AddUnscaledTimeLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "AddUnscaledTimeLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeLateUpdateTimeDelayTaskContainer)
            {
                _m_unscaledTimeLateUpdateTimeDelayTaskContainer.AddTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "AddUnscaledTimeLateUpdateTimeDelayTask success.");
            }
        }
        public void RemoveUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_updateTimeDelayTaskContainer)
            {
                _m_updateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUpdateTimeDelayTask success.");
            }
        }
        public void RemoveUnscaledTimeUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeUpdateTimeDelayTaskContainer)
            {
                _m_unscaledTimeUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeUpdateTimeDelayTask success.");
            }
        }
        public void RemoveFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_fixedUpdateTimeDelayTaskContainer)
            {
                _m_fixedUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveFixedUpdateTimeDelayTask success.");
            }
        }
        public void RemoveUnscaledTimeFixedUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeFixedUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeFixedUpdateTimeDelayTaskContainer)
            {
                _m_unscaledTimeFixedUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeFixedUpdateTimeDelayTask success.");
            }
        }
        public void RemoveLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_lateUpdateTimeDelayTaskContainer)
            {
                _m_lateUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveLateUpdateTimeDelayTask success.");
            }
        }
        public void RemoveUnscaledTimeLateUpdateTimeDelayTask(_ATimeDelayTask _task)
        {
            if (_task == null)
            {
                Console.LogWarning(SystemNames.Task, "RemoveUnscaledTimeLateUpdateTimeDelayTask failed, Task is null.");
                return;
            }

            lock (_m_unscaledTimeLateUpdateTimeDelayTaskContainer)
            {
                _m_unscaledTimeLateUpdateTimeDelayTaskContainer.RemoveTask(_task);
                Console.LogVerbose(SystemNames.Task, _task.name, "RemoveUnscaledTimeLateUpdateTimeDelayTask success.");
            }
        }


        private void Update()
        {
            // Use the `bool lockTaken` pattern so that if any Monitor.Enter throws
            // (ThreadAbortException, OOM, etc.) the finally block only releases the
            // locks that were actually acquired. A plain sequence of Monitor.Enter
            // calls before `try` would leak locks on partial acquisition.
            bool continuousTaken = false;
            bool unscaledContinuousTaken = false;
            bool frameDelayTaken = false;
            bool timeDelayTaken = false;
            bool unscaledTimeDelayTaken = false;

            try
            {
                Monitor.Enter(_m_updateContinuousTaskContainer, ref continuousTaken);
                Monitor.Enter(_m_unscaledTimeUpdateContinuousTaskContainer, ref unscaledContinuousTaken);
                Monitor.Enter(_m_updateFrameDelayTaskContainer, ref frameDelayTaken);
                Monitor.Enter(_m_updateTimeDelayTaskContainer, ref timeDelayTaken);
                Monitor.Enter(_m_unscaledTimeUpdateTimeDelayTaskContainer, ref unscaledTimeDelayTaken);

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
                if (unscaledTimeDelayTaken)
                {
                    _m_unscaledTimeUpdateTimeDelayTaskContainer.NextFrame();
                    Monitor.Exit(_m_unscaledTimeUpdateTimeDelayTaskContainer);
                }
                if (timeDelayTaken)
                {
                    _m_updateTimeDelayTaskContainer.NextFrame();
                    Monitor.Exit(_m_updateTimeDelayTaskContainer);
                }
                if (frameDelayTaken)
                {
                    _m_updateFrameDelayTaskContainer.NextFrame();
                    Monitor.Exit(_m_updateFrameDelayTaskContainer);
                }
                if (unscaledContinuousTaken)
                {
                    _m_unscaledTimeUpdateContinuousTaskContainer.NextFrame();
                    Monitor.Exit(_m_unscaledTimeUpdateContinuousTaskContainer);
                }
                if (continuousTaken)
                {
                    _m_updateContinuousTaskContainer.NextFrame();
                    Monitor.Exit(_m_updateContinuousTaskContainer);
                }
            }
        }
        private void FixedUpdate()
        {
            bool continuousTaken = false;
            bool unscaledContinuousTaken = false;
            bool frameDelayTaken = false;
            bool timeDelayTaken = false;
            bool unscaledTimeDelayTaken = false;

            try
            {
                Monitor.Enter(_m_fixedUpdateContinuousTaskContainer, ref continuousTaken);
                Monitor.Enter(_m_unscaledTimeFixedUpdateContinuousTaskContainer, ref unscaledContinuousTaken);
                Monitor.Enter(_m_fixedUpdateFrameDelayTaskContainer, ref frameDelayTaken);
                Monitor.Enter(_m_fixedUpdateTimeDelayTaskContainer, ref timeDelayTaken);
                Monitor.Enter(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer, ref unscaledTimeDelayTaken);

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
                if (unscaledTimeDelayTaken)
                {
                    _m_unscaledTimeFixedUpdateTimeDelayTaskContainer.NextFrame();
                    Monitor.Exit(_m_unscaledTimeFixedUpdateTimeDelayTaskContainer);
                }
                if (timeDelayTaken)
                {
                    _m_fixedUpdateTimeDelayTaskContainer.NextFrame();
                    Monitor.Exit(_m_fixedUpdateTimeDelayTaskContainer);
                }
                if (frameDelayTaken)
                {
                    _m_fixedUpdateFrameDelayTaskContainer.NextFrame();
                    Monitor.Exit(_m_fixedUpdateFrameDelayTaskContainer);
                }
                if (unscaledContinuousTaken)
                {
                    _m_unscaledTimeFixedUpdateContinuousTaskContainer.NextFrame();
                    Monitor.Exit(_m_unscaledTimeFixedUpdateContinuousTaskContainer);
                }
                if (continuousTaken)
                {
                    _m_fixedUpdateContinuousTaskContainer.NextFrame();
                    Monitor.Exit(_m_fixedUpdateContinuousTaskContainer);
                }
            }
        }
        private void LateUpdate()
        {
            bool continuousTaken = false;
            bool unscaledContinuousTaken = false;
            bool frameDelayTaken = false;
            bool timeDelayTaken = false;
            bool unscaledTimeDelayTaken = false;

            try
            {
                Monitor.Enter(_m_lateUpdateContinuousTaskContainer, ref continuousTaken);
                Monitor.Enter(_m_unscaledTimeLateUpdateContinuousTaskContainer, ref unscaledContinuousTaken);
                Monitor.Enter(_m_lateUpdateFrameDelayTaskContainer, ref frameDelayTaken);
                Monitor.Enter(_m_lateUpdateTimeDelayTaskContainer, ref timeDelayTaken);
                Monitor.Enter(_m_unscaledTimeLateUpdateTimeDelayTaskContainer, ref unscaledTimeDelayTaken);

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
                if (unscaledTimeDelayTaken)
                {
                    _m_unscaledTimeLateUpdateTimeDelayTaskContainer.NextFrame();
                    Monitor.Exit(_m_unscaledTimeLateUpdateTimeDelayTaskContainer);
                }
                if (timeDelayTaken)
                {
                    _m_lateUpdateTimeDelayTaskContainer.NextFrame();
                    Monitor.Exit(_m_lateUpdateTimeDelayTaskContainer);
                }
                if (frameDelayTaken)
                {
                    _m_lateUpdateFrameDelayTaskContainer.NextFrame();
                    Monitor.Exit(_m_lateUpdateFrameDelayTaskContainer);
                }
                if (unscaledContinuousTaken)
                {
                    _m_unscaledTimeLateUpdateContinuousTaskContainer.NextFrame();
                    Monitor.Exit(_m_unscaledTimeLateUpdateContinuousTaskContainer);
                }
                if (continuousTaken)
                {
                    _m_lateUpdateContinuousTaskContainer.NextFrame();
                    Monitor.Exit(_m_lateUpdateContinuousTaskContainer);
                }
            }
        }
        private void OnApplicationQuit()
        {
            _g_instance = new NullTaskManager();
        }
    }
}
