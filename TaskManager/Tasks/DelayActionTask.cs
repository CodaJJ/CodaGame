// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;

namespace CodaGame.Tasks
{
    /// <summary>
    /// A delay task which will run a delegate delay.
    /// </summary>
    /// <remarks>
    /// <para>It the delay time is zero or negative, it is exactly the same as <see cref="ActionTask"/>.</para>
    /// </remarks>
    public class DelayActionTask : _ATimeDelayTask
    {
        private readonly Action _m_delegate;
        
        
        public DelayActionTask(string _name, Action _delegate, float _delayTime, UpdateType _runType = UpdateType.Update, bool _useUnscaledTime = false) 
            : base(_name, _delayTime, _runType, _useUnscaledTime)
        {
            _m_delegate = _delegate;
        }
        public DelayActionTask(Action _delegate, float _delayTime, UpdateType _runType = UpdateType.Update, bool _useUnscaledTime = false)
            : this($"DelayActionTask_{Serialize.NextDelayActionTask()}", _delegate, _delayTime, _runType, _useUnscaledTime)
        {
        }
        
        
        protected override void OnDeal()
        {
            _m_delegate?.Invoke();
        }
        protected override void OnRun()
        {
        }
        protected override void OnStop()
        {
        }
    }
}