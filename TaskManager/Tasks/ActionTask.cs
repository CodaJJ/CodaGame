// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;

namespace CodaGame.Tasks
{
    /// <summary>
    /// A delegate task.
    /// </summary>
    /// <remarks>
    /// <para>It's useful turn your process into main thread.</para>
    /// </remarks>
    public class ActionTask : _AFrameDelayTask
    {
        private readonly Action _m_delegate;
        
        
        public ActionTask(string _name, Action _delegate, UpdateType _runType = UpdateType.Update) 
            : base(_name, 0, _runType)
        {
            _m_delegate = _delegate;
        }
        public ActionTask(Action _delegate, UpdateType _runType = UpdateType.Update)
            : this($"ActionTask_{Serialize.NextActionTask()}", _delegate, _runType)
        {
        }


        protected override void OnExecute()
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