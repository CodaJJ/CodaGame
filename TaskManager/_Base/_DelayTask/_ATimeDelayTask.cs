// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame.Base
{
    /// <summary>
    /// A base class for the time delay tasks.
    /// </summary>
    /// <remarks>
    /// <para>Time delay task will execute after a certain time.</para>
    /// <para>If the delay time is 0 or negative, the task will execute right after next frame.</para>
    /// </remarks>
    public abstract class _ATimeDelayTask : _ABaseTask, IComparable<_ATimeDelayTask>
    {
        private readonly float _m_delayTime;
        private readonly bool _m_useUnscaledTime;

        private float _m_executeTime;
        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// <para>If the "_delayTime" is 0 or negative, the task will execute right after next frame.</para>
        /// </remarks>
        protected _ATimeDelayTask(string _name, float _delayTime, UpdateType _runType, bool _useUnscaledTime)
            : base(_name, _runType)
        {
            _m_delayTime = _delayTime;
            _m_useUnscaledTime = _useUnscaledTime;
        }
        
        
        
        /// <summary>
        /// Is task using unscaled time.
        /// </summary>
        public bool UseUnscaledTime { get { return _m_useUnscaledTime; } }
        /// <summary>
        /// Delay time.
        /// </summary>
        /// <remarks>
        /// <para>How many seconds to wait before the task execute.</para>
        /// <para>If the delay time is 0 or negative, the task will execute right after next frame.</para>
        /// </remarks>
        public float DelayTime { get { return _m_delayTime; } }
        /// <summary>
        /// Execute time.
        /// </summary>
        /// <remarks>
        /// <para>How many seconds have passed when the task execute.</para>
        /// <para>It's determined by running type of the task and whether using unscaled time.</para>
        /// </remarks>
        internal float ExecuteTime { get { return _m_executeTime; } }


        protected abstract void OnExecute();
        
        
        internal void Execute()
        {
            OnExecute();
        }
        internal void SetExecuteTime(float _executeTime)
        {
            _m_executeTime = _executeTime;
        }
        

        private protected sealed override void AddToUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.AddUnscaledTimeUpdateTimeDelayTask(this);
            else
                TaskManager.instance.AddUpdateTimeDelayTask(this);
        }
        private protected sealed override void AddToFixedUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.AddUnscaledTimeFixedUpdateTimeDelayTask(this);
            else
                TaskManager.instance.AddFixedUpdateTimeDelayTask(this);
        }
        private protected sealed override void AddToLateUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.AddUnscaledTimeLateUpdateTimeDelayTask(this);
            else
                TaskManager.instance.AddLateUpdateTimeDelayTask(this);
        }
        private protected sealed override void RemoveFromUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.RemoveUnscaledTimeUpdateTimeDelayTask(this);
            else
                TaskManager.instance.RemoveUpdateTimeDelayTask(this);
        }
        private protected sealed override void RemoveFromFixedUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.RemoveUnscaledTimeFixedUpdateTimeDelayTask(this);
            else
                TaskManager.instance.RemoveFixedUpdateTimeDelayTask(this);
        }
        private protected sealed override void RemoveFromLateUpdateTaskSystem()
        {
            if (_m_useUnscaledTime)
                TaskManager.instance.RemoveUnscaledTimeLateUpdateTimeDelayTask(this);
            else
                TaskManager.instance.RemoveLateUpdateTimeDelayTask(this);
        }
        
        private protected override void OnInternalRun()
        {
        }
        private protected override void OnInternalStop()
        {
        }
        
        
        int IComparable<_ATimeDelayTask>.CompareTo(_ATimeDelayTask _other)
        {
            if (_other == null)
                return 1;
            
            return _m_executeTime.CompareTo(_other._m_executeTime);
        }
    }
}