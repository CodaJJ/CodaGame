// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    internal abstract class _ATimeDelayTaskContainer : _ATaskContainer<_ATimeDelayTask>
    {
        [ItemNotNull, NotNull] private readonly List<_ATimeDelayTask> _m_readyForExecuteTasks;
        [ItemNotNull, NotNull] private readonly List<_ATimeDelayTask> _m_delayTasks;
        private int _m_nextExecuteIndex;
        
        
        protected _ATimeDelayTaskContainer()
        {
            _m_readyForExecuteTasks = new List<_ATimeDelayTask>();
            _m_delayTasks = new List<_ATimeDelayTask>();
        }
        
        
        public override void AddTask(_ATimeDelayTask _task)
        {
            float nowTime = GetNowTime();
            if (_task.DelayTime <= 0)
            {                
                _task.SetExecuteTime(nowTime);
                _m_readyForExecuteTasks.Add(_task);
                return;
            }

            _task.SetExecuteTime(nowTime + _task.DelayTime);
            _m_delayTasks.InsertSorted(_task);
        }
        public override void RemoveTask(_ATimeDelayTask _task)
        {
            int index = _m_readyForExecuteTasks.IndexOf(_task);
            if (index >= 0)
            {
                _m_readyForExecuteTasks.RemoveAt(index);
                if (index < _m_nextExecuteIndex)
                    _m_nextExecuteIndex--;
                return;
            }

            if (!_m_delayTasks.RemoveSorted(_task))
                Console.LogWarning(SystemNames.TaskSystem, _task.name, "The task is not in the task container.");
        }
        /// <inheritdoc />
        public override bool ExecuteTasks()
        {
            if (_m_readyForExecuteTasks.Count == 0)
                return false;
            
            while (_m_nextExecuteIndex < _m_readyForExecuteTasks.Count)
            {
                _ATimeDelayTask task = _m_readyForExecuteTasks[_m_nextExecuteIndex++];
                task.Execute();
                if (task.isRunning)
                    task.StopBySystem();
            }
            
            _m_readyForExecuteTasks.Clear();
            _m_nextExecuteIndex = 0;
            return true;
        }
        /// <inheritdoc />
        public override void NextFrame()
        {
            float nowTime = GetNowTime();

            foreach (_ATimeDelayTask task in _m_delayTasks)
            {
                if (task.ExecuteTime > nowTime)
                    break;
                
                _m_readyForExecuteTasks.Add(task);
            }
            
            _m_delayTasks.RemoveRange(0, _m_readyForExecuteTasks.Count);
        }


        private protected abstract float GetNowTime();
    }
    internal class TimeDelayTaskContainer : _ATimeDelayTaskContainer
    {
        private protected override float GetNowTime()
        {
            return Time.time;
        }
    }
    internal class UnscaledTimeDelayTaskContainer : _ATimeDelayTaskContainer
    {
        private protected override float GetNowTime()
        {
            return Time.unscaledTime;
        }
    }
    internal class FixedTimeDelayTaskContainer : _ATimeDelayTaskContainer
    {
        private protected override float GetNowTime()
        {
            return Time.fixedTime;
        }
    }
    internal class FixedUnscaledTimeDelayTaskContainer : _ATimeDelayTaskContainer
    {
        private protected override float GetNowTime()
        {
            return Time.fixedUnscaledTime;
        }
    }
}