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
        [ItemNotNull, NotNull] private readonly List<_ATimeDelayTask> _m_readyForDealTasks;
        [ItemNotNull, NotNull] private readonly List<_ATimeDelayTask> _m_delayTasks;
        private int _m_nextDealIndex;
        
        
        internal _ATimeDelayTaskContainer()
        {
            _m_readyForDealTasks = new List<_ATimeDelayTask>();
            _m_delayTasks = new List<_ATimeDelayTask>();
        }
        
        
        internal override void AddTask(_ATimeDelayTask _task)
        {
            float nowTime = GetNowTime();
            if (_task.DelayTime <= 0)
            {                
                _task.SetDealTime(nowTime);
                _m_readyForDealTasks.Add(_task);
                return;
            }

            _task.SetDealTime(nowTime + _task.DelayTime);
            _m_delayTasks.InsertSorted(_task);
        }
        internal override void RemoveTask(_ATimeDelayTask _task)
        {
            int index = _m_readyForDealTasks.IndexOf(_task);
            if (index >= 0)
            {
                _m_readyForDealTasks.RemoveAt(index);
                if (index < _m_nextDealIndex)
                    _m_nextDealIndex--;
                return;
            }

            if (!_m_delayTasks.RemoveSorted(_task))
                Console.LogWarning(SystemNames.TaskSystem, _task.name, "The task is not in the task container.");
        }
        /// <inheritdoc />
        internal override bool DealTasks()
        {
            if (_m_readyForDealTasks.Count == 0)
                return false;
            
            while (_m_nextDealIndex < _m_readyForDealTasks.Count)
            {
                _ATimeDelayTask task = _m_readyForDealTasks[_m_nextDealIndex++];
                task.Deal();
                if (task.isRunning)
                    task.StopBySystem();
            }
            
            _m_readyForDealTasks.Clear();
            _m_nextDealIndex = 0;
            return true;
        }
        /// <inheritdoc />
        internal override void NextFrame()
        {
            float nowTime = GetNowTime();

            foreach (_ATimeDelayTask task in _m_delayTasks)
            {
                if (task.DealTime > nowTime)
                    break;
                
                _m_readyForDealTasks.Add(task);
            }
            
            _m_delayTasks.RemoveRange(0, _m_readyForDealTasks.Count);
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