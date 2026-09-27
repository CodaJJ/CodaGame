// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame.Base
{
    /// <summary>
    /// A continuous task. A positive duration limits elapsed time using its container's clock.
    /// </summary>
    public abstract class _AContinuousTask : _ABaseTask
    {
        private readonly bool _m_useUnscaledTime;
        private readonly float _m_duration;

        private double _m_startTime;
        private double _m_elapsedTime;
        
        
        internal _AContinuousTask(string _name, UpdateType _runType, bool _useUnscaledTime, float _duration = -1)
            : base(_name, _runType)
        {
            _m_useUnscaledTime = _useUnscaledTime;
            _m_duration = _duration;
        }
        
        
        /// <summary>
        /// Is task using unscaled time.
        /// </summary>
        public bool UseUnscaledTime { get { return _m_useUnscaledTime; } }


        protected abstract void TickInternal(float _deltaTime);


        internal void Tick(float _deltaTime, double _nowTime)
        {
            if (_m_duration <= 0)
            {
                TickInternal(_deltaTime);
                return;
            }

            uint capturedStopVersion = stopVersion;
            // Measure from Run, so a task added during an update cannot consume time before it started.
            double elapsedTime = Math.Min(_m_duration, Math.Max(_m_elapsedTime, _nowTime - _m_startTime));
            float deltaTime = (float)(elapsedTime - _m_elapsedTime);
            _m_elapsedTime = elapsedTime;
            bool durationComplete = elapsedTime >= _m_duration;

            try
            {
                TickInternal(deltaTime);
            }
            finally
            {
                // A callback may stop/restart this task. Only finish the original run.
                if (durationComplete && stopVersion == capturedStopVersion)
                    Stop();
            }
        }
        internal void SetStartTime(double _startTime)
        {
            _m_startTime = _startTime;
            _m_elapsedTime = 0;
        }

        
        private protected sealed override void AddToUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.AddUnscaledTimeUpdateContinuousTask(this);
            else
                TaskManager.instance.AddUpdateContinuousTask(this);
        }
        private protected sealed override void AddToFixedUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.AddUnscaledTimeFixedUpdateContinuousTask(this);
            else
                TaskManager.instance.AddFixedUpdateContinuousTask(this);
        }
        private protected sealed override void AddToLateUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.AddUnscaledTimeLateUpdateContinuousTask(this);
            else
                TaskManager.instance.AddLateUpdateContinuousTask(this);
        }
        private protected sealed override void RemoveFromUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.RemoveUnscaledTimeUpdateContinuousTask(this);
            else
                TaskManager.instance.RemoveUpdateContinuousTask(this);
        }
        private protected sealed override void RemoveFromFixedUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.RemoveUnscaledTimeFixedUpdateContinuousTask(this);
            else
                TaskManager.instance.RemoveFixedUpdateContinuousTask(this);
        }
        private protected sealed override void RemoveFromLateUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.RemoveUnscaledTimeLateUpdateContinuousTask(this);
            else
                TaskManager.instance.RemoveLateUpdateContinuousTask(this);
        }
        private protected override void OnInternalRun()
        {
        }
    }
}
