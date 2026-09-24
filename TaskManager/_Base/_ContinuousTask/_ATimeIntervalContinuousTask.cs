// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame.Base
{
    /// <summary>
    /// Base class of a continuous task with time interval.
    /// </summary>
    /// <remarks>
    /// <para>Time interval continuous task will execute in a certain time interval.</para>
    /// <para>Make sure the time interval is greater than 0.</para>
    /// </remarks>
    public abstract class _ATimeIntervalContinuousTask : _AContinuousTask
    {
        private const double _k_relativeTimeTolerance = 1e-6;
        private const double _k_maxIntervalTolerance = 1e-4;


        private readonly float _m_timeInterval;
        private readonly bool _m_executeOnceImmediately;
        
        private double _m_intervalTimeCounter;
        private double _m_totalDeltaTime;
        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// <para>Makes sure the <see cref="_timeInterval"/>> is greater than 0, otherwise it will throw an exception.</para>
        /// <para>The <see cref="_executeOnceImmediately"/>> parameter is used to determine whether the task should execute once immediately.</para>
        /// </remarks>
        protected _ATimeIntervalContinuousTask(string _name, float _timeInterval, bool _executeOnceImmediately, UpdateType _runType, bool _useUnscaledTime, float _duration = -1)
            : base(_name, _runType, _useUnscaledTime, _duration)
        {
            if (_timeInterval <= 0)
                Console.LogCrush(SystemNames.Task, _name, "Time interval must be greater than 0.");

            _m_timeInterval = _timeInterval;
            _m_executeOnceImmediately = _executeOnceImmediately;
        }
        
        
        /// <summary>
        /// Time interval.
        /// </summary>
        /// <remarks>
        /// <para>How many seconds to wait before the next tick.</para>
        /// </remarks>
        public float TimeInterval { get { return _m_timeInterval; } }
        
        
        /// <summary>
        /// Execute function.
        /// </summary>
        /// <remarks>
        /// <para>This function will be called continuously during the task is running.</para>
        /// <para>How long between each tick is determined by the time interval and whether using unscaled time.</para>
        /// </remarks>
        protected abstract void OnTick();


        protected override void TickInternal(float _deltaTime)
        {
            // Defensive guard: a non-positive interval would cause an infinite loop below.
            // The constructor already rejects this, but keep the guard in case LogCrush did not terminate execution.
            if (_m_timeInterval <= 0)
                return;

            uint capturedStopVersion = stopVersion;
            _m_intervalTimeCounter += _deltaTime;
            _m_totalDeltaTime += _deltaTime;

            // Float inputs can accumulate rounding error across many intervals.
            // Bound the allowance to 0.01% of an interval to limit early execution.
            double tolerance = Math.Min(_m_timeInterval * _k_maxIntervalTolerance,
                Math.Max(_m_timeInterval, _m_totalDeltaTime) * _k_relativeTimeTolerance);

            while (_m_intervalTimeCounter + tolerance >= _m_timeInterval)
            {
                OnTick();

                // Stop ends this catch-up loop. A new Run owns its own counter,
                // even if the callback stopped and restarted this same task.
                if (stopVersion != capturedStopVersion)
                    return;

                // Keep a small negative remainder: later elapsed time must repay it.
                _m_intervalTimeCounter -= _m_timeInterval;
            }
        }


        private protected override void OnInternalRun()
        {
            base.OnInternalRun();
            _m_intervalTimeCounter = _m_executeOnceImmediately ? _m_timeInterval : 0;
            _m_totalDeltaTime = 0;
        }
        private protected override void OnInternalStop()
        {
        }
    }
}
