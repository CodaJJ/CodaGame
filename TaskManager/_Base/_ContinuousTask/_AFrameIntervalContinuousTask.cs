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
    /// Frame interval continuous task will deal in a certain frame interval,
    /// and the delta time will be sum of the time of the frame interval.
    /// </para>
    /// <para>Also you can set the frame interval to 0, then the task will deal every frame.</para>
    /// </remarks>
    public abstract class _AFrameIntervalContinuousTask : _AContinuousTask
    {
        private readonly int _m_frameInterval;
        private readonly bool _m_dealOnceImmediately;

        private int _m_frameIntervalCounter;
        private float _m_intervalDeltaTime;
        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// <para>If the "_frameInternal" is 0 or negative, the task will deal every frame.</para>
        /// <para>The "_dealImmediately" parameter is used to determine whether the task should deal once immediately.</para>
        /// </remarks>
        protected _AFrameIntervalContinuousTask(string _name, int _frameInterval, bool _dealOnceImmediately, UpdateType _runType, bool _useUnscaledTime) 
            : base(_name, _runType, _useUnscaledTime)
        {
            _m_frameInterval = _frameInterval;
            _m_dealOnceImmediately = _dealOnceImmediately;
        }
        
        
        /// <summary>
        /// Frame interval.
        /// </summary>
        /// <remarks>
        /// <para>How many frames to wait before the next deal.</para>
        /// </remarks>
        public int FrameInterval { get { return _m_frameInterval; } }
        
        
        /// <summary>
        /// Deal function.
        /// </summary>
        /// <remarks>
        /// <para>This function will be called continuously during the task is running.</para>
        /// <para>The "_deltaTime" value is determined by the running type of the task, the frame interval and whether using unscaled time.</para>
        /// </remarks>
        protected abstract void OnDeal(float _deltaTime);
        
        
        internal override void Deal(float _deltaTime)
        {
            if (_m_frameInterval <= 0)
                OnDeal(_deltaTime);
            else
            {
                _m_intervalDeltaTime += _deltaTime;
                if (_m_frameIntervalCounter == 0)
                {
                    float deltaTime = _m_intervalDeltaTime;
                    _m_frameIntervalCounter = _m_frameInterval;
                    _m_intervalDeltaTime = 0;
                    
                    OnDeal(deltaTime);
                }
                else
                    _m_frameIntervalCounter -= 1;
            }
        }


        private protected override void OnInternalRun()
        {
            _m_frameIntervalCounter = _m_dealOnceImmediately ? 0 : _m_frameInterval;
        }
        private protected override void OnInternalStop()
        {
        }
    }
}