// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;

namespace CodaGame.Tasks
{
    /// <summary>
    /// A task that you can run a delegate later.
    /// </summary>
    /// <remarks>
    /// <para>If the delay frame count is zero or negative, it is exactly the same as <see cref="ActionTask"/>.</para>
    /// <para>If the delay frame count is one, it is exactly the same as <see cref="NextFrameActionTask"/>.</para>
    /// </remarks>
    public class FrameDelayActionTask : _AFrameDelayTask
    {
        private readonly Action _m_delegate;
        
        
        public FrameDelayActionTask(string _name, Action _delegate, int _frameCountDelay, UpdateType _runType = UpdateType.Update) 
            : base(_name, _frameCountDelay, _runType)
        {
            _m_delegate = _delegate;
        }
        public FrameDelayActionTask(Action _delegate, int _frameCountDelay, UpdateType _runType = UpdateType.Update)
            : this($"FrameDelayActionTask_{Serialize.NextFrameDelayActionTask()}", _delegate, _frameCountDelay, _runType)
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