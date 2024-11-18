
using UnityEngine;

namespace UnityGameFramework.Base.Tasks
{
    /// <summary>
    /// A task that you can continuously run a function at fixed interval.
    /// </summary>
    public abstract class _AFixedIntervalContinuousTemplateTask : _ATask
    {
        private readonly float _m_duration;
        private readonly float _m_fixedInterval;
        private float _m_timeCounter;
        private float _m_intervalTimeCounter;
        private ulong _m_runCount;
        
        
        protected _AFixedIntervalContinuousTemplateTask(string _name, float _fixedInterval, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update) 
            : base(_name, _runType)
        {
            _m_fixedInterval = _fixedInterval;
            _m_duration = _duration;
            
            if (_m_fixedInterval <= 0)
                Console.LogCrush(SystemNames.TaskSystem, "Fixed interval must be greater than 0.");
        }
        

        /// <summary>
        /// The duration of task.
        /// </summary>
        public float duration { get { return _m_duration; } }
        /// <summary>
        /// The fixed interval of task. (This is not Time.fixedDeltaTime or Time.fixedUnscaledDeltaTime from Unity)
        /// </summary>
        public float fixedInterval { get { return _m_fixedInterval; } }
        /// <summary>
        /// Remaining time (second).
        /// </summary>
        public float remainingTime { get { return _m_duration - _m_timeCounter; } }
        /// <summary>
        /// Count of run.
        /// </summary>
        public ulong runCount { get { return _m_runCount; } }
        
        

        public sealed override void Deal(float _deltaTime)
        {
            while (_m_intervalTimeCounter > _m_fixedInterval)
            {
                TemplateTaskDeal();
                _m_runCount++;

                _m_intervalTimeCounter -= _m_fixedInterval;
            }
            
            if (_m_timeCounter > _m_duration)
            {
                Stop();
                return;
            }
            
            _m_intervalTimeCounter = Mathf.Min(remainingTime, _m_intervalTimeCounter + _deltaTime);
            _m_timeCounter += _deltaTime;
        }

        protected sealed override void OnRun()
        {
            _m_timeCounter = 0;
            _m_intervalTimeCounter = 0;
            _m_runCount = 0;

            OnTemplateTaskRun();
        }
        protected sealed override void OnStop()
        {
            OnTemplateTaskStop();
        }

        protected abstract void TemplateTaskDeal();
        protected abstract void OnTemplateTaskRun();
        protected abstract void OnTemplateTaskStop();
    }
}