// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace UnityGameFramework.Base
{
    /// <summary>
    /// A continuous task.
    /// </summary>
    public abstract class _AContinuousTask : _ABaseTask
    {
        private readonly bool _m_useUnscaledTime;
        
        
        internal _AContinuousTask(string _name, ETaskRunType _runType, bool _useUnscaledTime) 
            : base(_name, _runType)
        {
            _m_useUnscaledTime = _useUnscaledTime;
        }
        
        
        /// <summary>
        /// Is task using unscaled time.
        /// </summary>
        public bool UseUnscaledTime { get { return _m_useUnscaledTime; } }


        internal abstract void Deal(float _deltaTime);

        
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
    }
}