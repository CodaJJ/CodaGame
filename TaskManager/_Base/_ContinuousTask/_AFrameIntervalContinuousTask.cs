// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame.Base
{
    /// <summary>
    /// Base class for frame interval continuous task.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Frame interval continuous task will execute in a certain frame interval,
    /// and the delta time will be sum of the time of the frame interval.
    /// </para>
    /// <para>Also you can set the frame interval to 0, then the task will execute every frame.</para>
    /// </remarks>
    public abstract class _AFrameIntervalContinuousTask : _AContinuousTask
    {
        private readonly int _m_frameInterval;
        private readonly bool _m_executeOnceImmediately;

        private int _m_frameIntervalCounter;
        private float _m_intervalDeltaTime;
        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// <para>If the <see cref="_frameInterval"/>> is 0 or negative, the task will execute every frame.</para>
        /// <para>The <see cref="_executeOnceImmediately"/> parameter is used to determine whether the task should execute once immediately.</para>
        /// </remarks>
        protected _AFrameIntervalContinuousTask(string _name, int _frameInterval, bool _executeOnceImmediately, UpdateType _runType, bool _useUnscaledTime) 
            : base(_name, _runType, _useUnscaledTime)
        {
            _m_frameInterval = _frameInterval;
            _m_executeOnceImmediately = _executeOnceImmediately;
        }
        
        
        /// <summary>
        /// Frame interval.
        /// </summary>
        /// <remarks>
        /// <para>How many frames to wait before the next tick.</para>
        /// </remarks>
        public int FrameInterval { get { return _m_frameInterval; } }
        
        
        /// <summary>
        /// Execute function.
        /// </summary>
        /// <remarks>
        /// <para>This function will be called continuously during the task is running.</para>
        /// <para>The "_deltaTime" value is determined by the running type of the task, the frame interval and whether using unscaled time.</para>
        /// </remarks>
        protected abstract void OnTick(float _deltaTime);
        
        
        internal override void Tick(float _deltaTime)
        {
            if (_m_frameInterval <= 0)
                OnTick(_deltaTime);
            else
            {
                _m_intervalDeltaTime += _deltaTime;
                if (_m_frameIntervalCounter == 0)
                {
                    float deltaTime = _m_intervalDeltaTime;
                    _m_frameIntervalCounter = _m_frameInterval;
                    _m_intervalDeltaTime = 0;
                    
                    OnTick(deltaTime);
                }
                else
                    _m_frameIntervalCounter -= 1;
            }
        }


        private protected override void OnInternalRun()
        {
            _m_frameIntervalCounter = _m_executeOnceImmediately ? 0 : _m_frameInterval;
        }
        private protected override void OnInternalStop()
        {
        }
    }
}