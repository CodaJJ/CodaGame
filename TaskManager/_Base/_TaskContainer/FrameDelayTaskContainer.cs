// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame.Base
{
    /// <summary>
    /// A frame delay task container.
    /// </summary>
    /// <remarks>
    /// <para>Handles frame delay tasks.</para>
    /// </remarks>
    internal class FrameDelayTaskContainer : _ATaskContainer<_AFrameDelayTask>
    {
        [ItemNotNull, NotNull] private readonly List<_AFrameDelayTask> _m_readyForExecuteTasks;
        [ItemNotNull, NotNull] private readonly List<_AFrameDelayTask> _m_delayTasks;
        private int _m_frame;
        private int _m_nextExecuteIndex;
        
        
        public FrameDelayTaskContainer()
        {
            _m_readyForExecuteTasks = new List<_AFrameDelayTask>();
            _m_delayTasks = new List<_AFrameDelayTask>();
        }
        
        
        public override void AddTask(_AFrameDelayTask _task)
        {
            if (_task.DelayFrame <= 0)
            {                
                _task.SetExecuteFrame(_m_frame);
                _m_readyForExecuteTasks.Add(_task);
                return;
            }

            _task.SetExecuteFrame(_m_frame + _task.DelayFrame);
            _m_delayTasks.InsertSorted(_task);
        }
        public override void RemoveTask(_AFrameDelayTask _task)
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
                Console.LogWarning(SystemNames.Task, _task.name, "The task is not in the task container.");
        }
        /// <inheritdoc />
        public override bool ExecuteTasks()
        {
            if (_m_readyForExecuteTasks.Count == 0)
                return false;
            
            while (_m_nextExecuteIndex < _m_readyForExecuteTasks.Count)
            {
                _AFrameDelayTask task = _m_readyForExecuteTasks[_m_nextExecuteIndex++];
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
            _m_frame++;

            foreach (_AFrameDelayTask task in _m_delayTasks)
            {
                if (task.ExecuteFrame > _m_frame)
                    break;
                
                _m_readyForExecuteTasks.Add(task);
            }
            
            _m_delayTasks.RemoveRange(0, _m_readyForExecuteTasks.Count);
        }
    }
}