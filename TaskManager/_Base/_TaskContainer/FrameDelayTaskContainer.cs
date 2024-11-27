// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityGameFramework.Base
{
    /// <summary>
    /// A frame delay task container.
    /// </summary>
    /// <remarks>
    /// <para>Handles frame delay tasks.</para>
    /// </remarks>
    internal class FrameDelayTaskContainer : _ATaskContainer<_AFrameDelayTask>
    {
        [ItemNotNull, NotNull] private readonly List<_AFrameDelayTask> _m_readyForDealTasks;
        [ItemNotNull, NotNull] private readonly List<_AFrameDelayTask> _m_delayTasks;
        private int _m_frame;
        private int _m_nextDealIndex;
        
        
        internal FrameDelayTaskContainer()
        {
            _m_readyForDealTasks = new List<_AFrameDelayTask>();
            _m_delayTasks = new List<_AFrameDelayTask>();
        }
        
        
        internal override void AddTask(_AFrameDelayTask _task)
        {
            if (_task.DelayFrame <= 0)
            {                
                _task.SetDealFrame(_m_frame);
                _m_readyForDealTasks.Add(_task);
                return;
            }

            _task.SetDealFrame(_m_frame + _task.DelayFrame);
            _m_delayTasks.InsertSorted(_task);
        }
        internal override void RemoveTask(_AFrameDelayTask _task)
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
                Console.LogWarning(SystemNames.TaskSystem, $"-- {_task.name} -- : The task is not in the task container.");
        }
        /// <inheritdoc />
        internal override bool DealTasks()
        {
            if (_m_readyForDealTasks.Count == 0)
                return false;
            
            while (_m_nextDealIndex < _m_readyForDealTasks.Count)
            {
                _AFrameDelayTask task = _m_readyForDealTasks[_m_nextDealIndex++];
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
            _m_frame++;

            foreach (_AFrameDelayTask task in _m_delayTasks)
            {
                if (task.DealFrame > _m_frame)
                    break;
                
                _m_readyForDealTasks.Add(task);
            }
            
            _m_delayTasks.RemoveRange(0, _m_readyForDealTasks.Count);
        }
    }
}