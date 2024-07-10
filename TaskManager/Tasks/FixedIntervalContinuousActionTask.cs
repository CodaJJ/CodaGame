
using System;
using UnityGameFramework.Base;

namespace UnityGameFramework.Base.Tasks
{
    /// <summary>
    /// A task that you can continuously run a delegate at fixed interval.
    /// </summary>
    public class FixedIntervalContinuousActionTask : _AFixedIntervalContinuousTemplateTask
    {
        private readonly Action _m_delegate;
        
        
        public FixedIntervalContinuousActionTask(Action _delegate, float _fixedInterval, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update) 
            : base(_fixedInterval, _duration, _runType)
        {
            _m_delegate = _delegate;
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