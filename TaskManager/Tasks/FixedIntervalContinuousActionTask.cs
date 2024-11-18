
using System;
using UnityGameFramework.Base.Tasks;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A task that you can continuously run a delegate at fixed interval.
    /// </summary>
    public class FixedIntervalContinuousActionTask : _AFixedIntervalContinuousTemplateTask
    {
        private readonly Action _m_delegate;
        
        
        public FixedIntervalContinuousActionTask(string _name, Action _delegate, float _fixedInterval, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update) 
            : base(_name, _fixedInterval, _duration, _runType)
        {
            _m_delegate = _delegate;
        }
        public FixedIntervalContinuousActionTask(Action _delegate, float _fixedInterval, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update)
            : this($"FixedIntervalContinuousActionTask_{Serialize.NextFixedIntervalContinuousTask()}", _delegate, _fixedInterval, _duration, _runType)
        {
        }
        
        
        protected override void TemplateTaskDeal()
        {
            _m_delegate?.Invoke();
        }
        protected override void OnTemplateTaskRun()
        {
        }
        protected override void OnTemplateTaskStop()
        {
        }
    }
}