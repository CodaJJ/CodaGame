// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace UnityGameFramework.Base
{
    /// <summary>
    /// A base class for the time delay tasks.
    /// </summary>
    /// <remarks>
    /// <para>Time delay task will deal after a certain time.</para>
    /// <para>If the delay time is 0 or negative, the task will deal right after next frame.</para>
    /// </remarks>
    public abstract class _ATimeDelayTask : _ABaseTask, IComparable<_ATimeDelayTask>
    {
        private readonly float _m_delayTime;
        private readonly bool _m_useUnscaledTime;

        private float _m_dealTime;
        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// <para>If the "_delayTime" is 0 or negative, the task will deal right after next frame.</para>
        /// </remarks>
        protected _ATimeDelayTask(string _name, float _delayTime, ETaskRunType _runType, bool _useUnscaledTime)
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
        /// <para>How many seconds to wait before the task deal.</para>
        /// <para>If the delay time is 0 or negative, the task will deal right after next frame.</para>
        /// </remarks>
        public float DelayTime { get { return _m_delayTime; } }
        /// <summary>
        /// Deal time.
        /// </summary>
        /// <remarks>
        /// <para>How many seconds have passed when the task deal.</para>
        /// <para>It's determined by running type of the task and whether using unscaled time.</para>
        /// </remarks>
        internal float DealTime { get { return _m_dealTime; } }


        protected abstract void OnDeal();
        
        
        internal void Deal()
        {
            OnDeal();
        }
        internal void SetDealTime(float _dealTime)
        {
            _m_dealTime = _dealTime;
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
            
            return _m_dealTime.CompareTo(_other._m_dealTime);
        }
    }
}