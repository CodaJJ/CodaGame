// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;

namespace CodaGame.Tasks
{
    /// <summary>
    /// A task that you can continuously run a delegate.
    /// </summary>
    /// <remarks>
    /// <para>It will run a delegate every frame.</para>
    /// <para>Also, you can set the duration to stop the task, negative or zero means never stop.</para>
    /// </remarks>
    public class ContinuousActionTask : _AEveryFrameContinuousTask
    {
        private readonly Action<float> _m_delegate;
        private readonly DelayActionTask _m_durationTask;


        public ContinuousActionTask(string _name, Action<float> _delegate, float _duration = -1, UpdateType _runType = UpdateType.Update, bool _useUnscaledTime = false) 
            : base(_name, _runType, _useUnscaledTime)
        {
            _m_delegate = _delegate;
            if (_duration > 0)
                _m_durationTask = new DelayActionTask(Stop, _duration, _runType, _useUnscaledTime);
        }
        public ContinuousActionTask(Action<float> _delegate, float _duration = -1, UpdateType _runType = UpdateType.Update, bool _useUnscaledTime = false)
            : this($"ContinuousActionTask_{Serialize.NextContinuousTask()}", _delegate, _duration, _runType, _useUnscaledTime)
        {
        }


        protected override void OnDeal(float _deltaTime)
        {
            _m_delegate?.Invoke(_deltaTime);
        }
        protected override void OnRun()
        {
            _m_durationTask?.Run();
        }
        protected override void OnStop()
        {
            if (_m_durationTask is { isRunning: true })
                _m_durationTask.Stop();
        }
    }
}