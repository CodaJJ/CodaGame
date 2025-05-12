// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

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
        private readonly float _m_timeInterval;
        private readonly bool _m_executeOnceImmediately;
        
        private float _m_intervalTimeCounter;
        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// <para>Makes sure the <see cref="_timeInterval"/>> is greater than 0, otherwise it will throw an exception.</para>
        /// <para>The <see cref="_executeOnceImmediately"/>> parameter is used to determine whether the task should execute once immediately.</para>
        /// </remarks>
        protected _ATimeIntervalContinuousTask(string _name, float _timeInterval, bool _executeOnceImmediately, UpdateType _runType, bool _useUnscaledTime) 
            : base(_name, _runType, _useUnscaledTime)
        {
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


        internal override void Tick(float _deltaTime)
        {
            while (_m_intervalTimeCounter >= _m_timeInterval)
            {
                OnTick();

                _m_intervalTimeCounter -= _m_timeInterval;
            }
            
            _m_intervalTimeCounter += _deltaTime;
        }


        private protected override void OnInternalRun()
        {
            _m_intervalTimeCounter = _m_executeOnceImmediately ? _m_timeInterval : 0;
            if (_m_timeInterval <= 0)
                Console.LogCrush(SystemNames.TaskSystem, name, "Time interval must be greater than 0.");
        }
        private protected override void OnInternalStop()
        {
        }
    }
}