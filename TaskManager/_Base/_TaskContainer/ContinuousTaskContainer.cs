// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// A continuous task container.
    /// </summary>
    /// <remarks>
    /// <para>Handles continuous tasks.</para>
    /// </remarks>
    internal abstract class _AContinuousTaskContainer : _ATaskContainer<_AContinuousTask>
    {
        [ItemNotNull, NotNull] private readonly List<_AContinuousTask> _m_readyForExecuteTasks;
        private int _m_nextExecuteIndex;


        protected _AContinuousTaskContainer()
        {
            _m_readyForExecuteTasks = new List<_AContinuousTask>();
            _m_nextExecuteIndex = 0;
        }
        
        
        public override void AddTask(_AContinuousTask _task)
        {
            _task.SetStartTime(GetNowTime());
            _m_readyForExecuteTasks.Add(_task);
        }
        public override void RemoveTask(_AContinuousTask _task)
        {
            int index = _m_readyForExecuteTasks.IndexOf(_task);
            if (index < 0)
            {
                Console.LogWarning(SystemNames.Task, _task.name, "The task is not in the task container.");
                return;
            }
            
            _m_readyForExecuteTasks.RemoveAt(index);
            if (index < _m_nextExecuteIndex)
                _m_nextExecuteIndex--;
        }
        /// <inheritdoc />
        public override bool ExecuteTasks()
        {
            if (_m_readyForExecuteTasks.Count == 0)
                return false;
            
            if (_m_nextExecuteIndex >= _m_readyForExecuteTasks.Count)
                return false;

            while (_m_nextExecuteIndex < _m_readyForExecuteTasks.Count)
            {
                _AContinuousTask task = _m_readyForExecuteTasks[_m_nextExecuteIndex++];
                try
                {
                    task.Tick(GetDeltaTime(), GetNowTime());
                }
                catch (System.Exception _exception)
                {
                    Console.LogError(SystemNames.Task, task.name, "An exception was thrown while ticking the task.");
                    UnityEngine.Debug.LogException(_exception);
                }
            }
            
            return true;
        }
        /// <inheritdoc />
        public override void NextFrame()
        {
            _m_nextExecuteIndex = 0;
        }
        
        
        private protected abstract float GetDeltaTime();
        private protected abstract float GetNowTime();
    }
    /// <summary>
    /// A continuous task container that uses Time.deltaTime.
    /// </summary>
    internal class DeltaTimeContinuousTaskContainer : _AContinuousTaskContainer
    {
        private protected override float GetDeltaTime()
        {
            return Time.deltaTime;
        }
        private protected override float GetNowTime()
        {
            return Time.time;
        }
    }
    /// <summary>
    /// A continuous task container that uses Time.unscaledDeltaTime.
    /// </summary>
    internal class UnscaledDeltaTimeContinuousTaskContainer : _AContinuousTaskContainer
    {
        private protected override float GetDeltaTime()
        {
            return Time.unscaledDeltaTime;
        }
        private protected override float GetNowTime()
        {
            return Time.unscaledTime;
        }
    }
    /// <summary>
    /// A continuous task container that uses Time.fixedDeltaTime.
    /// </summary>
    internal class FixedDeltaTimeContinuousTaskContainer : _AContinuousTaskContainer
    {
        private protected override float GetDeltaTime()
        {
            return Time.fixedDeltaTime;
        }
        private protected override float GetNowTime()
        {
            return Time.fixedTime;
        }
    }
    /// <summary>
    /// A continuous task container that uses Time.fixedUnscaledDeltaTime.
    /// </summary>
    internal class FixedUnscaledDeltaTimeContinuousTaskContainer : _AContinuousTaskContainer
    {
        private protected override float GetDeltaTime()
        {
            return Time.fixedUnscaledDeltaTime;
        }
        private protected override float GetNowTime()
        {
            return Time.fixedUnscaledTime;
        }
    }
}
