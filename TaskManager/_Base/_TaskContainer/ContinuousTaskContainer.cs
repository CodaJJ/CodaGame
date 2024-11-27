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
        [ItemNotNull, NotNull] private readonly List<_AContinuousTask> _m_readyForDealTasks;
        private int _m_nextDealIndex;
        
        
        internal _AContinuousTaskContainer()
        {
            _m_readyForDealTasks = new List<_AContinuousTask>();
            _m_nextDealIndex = 0;
        }
        
        
        internal override void AddTask(_AContinuousTask _task)
        {
            _m_readyForDealTasks.Add(_task);
        }
        internal override void RemoveTask(_AContinuousTask _task)
        {
            int index = _m_readyForDealTasks.IndexOf(_task);
            if (index < 0)
            {
                Console.LogWarning(SystemNames.TaskSystem, $"-- {_task.name} -- : The task is not in the task container.");
                return;
            }
            
            _m_readyForDealTasks.RemoveAt(index);
            if (index < _m_nextDealIndex)
                _m_nextDealIndex--;
        }
        /// <inheritdoc />
        internal override bool DealTasks()
        {
            if (_m_readyForDealTasks.Count == 0)
                return false;
            
            if (_m_nextDealIndex >= _m_readyForDealTasks.Count)
                return false;

            while (_m_nextDealIndex < _m_readyForDealTasks.Count)
            {
                _AContinuousTask task = _m_readyForDealTasks[_m_nextDealIndex++];
                task.Deal(GetDeltaTime());
            }
            
            return true;
        }
        /// <inheritdoc />
        internal override void NextFrame()
        {
            _m_nextDealIndex = 0;
        }
        
        
        private protected abstract float GetDeltaTime();
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
    }
}