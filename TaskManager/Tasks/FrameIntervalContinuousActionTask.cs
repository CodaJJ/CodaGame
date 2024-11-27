// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityGameFramework.Base;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A task that you can run a delegate every frame interval.
    /// </summary>
    /// <remarks>
    /// <para>It will run a delegate every frame interval.</para>
    /// <para>Also, you can set the duration to stop the task, negative or zero means never stop.</para>
    /// </remarks>
    public class FrameIntervalContinuousActionTask : _AFrameIntervalContinuousTask
    {
        private readonly Action<float> _m_delegate;
        private readonly DelayActionTask _m_durationTask;
        
        
        public FrameIntervalContinuousActionTask(string _name, Action<float> _delegate, int _frameInterval, bool _dealOnceImmediately = true, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update, bool _useUnscaledTime = false) 
            : base(_name, _frameInterval, _dealOnceImmediately, _runType, _useUnscaledTime)
        {
            _m_delegate = _delegate;
            if (_duration > 0)
                _m_durationTask = new DelayActionTask(Stop, _duration, _runType, _useUnscaledTime);
        }
        public FrameIntervalContinuousActionTask(Action<float> _delegate, int _frameInterval, bool _dealOnceImmediately = true, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update, bool _useUnscaledTime = false)
            : this($"FrameIntervalContinuousActionTask_{Serialize.NextFrameIntervalContinuousTask()}", _delegate, _frameInterval, _dealOnceImmediately, _duration, _runType, _useUnscaledTime)
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