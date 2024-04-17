
using System;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A task that you can continuously run a delegate.
    /// </summary>
    public class ContinuousActionTask : _AContinuousTemplateTask
    {
        private readonly Action _m_delegate;
        
        
        public ContinuousActionTask(Action _delegate, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.UnscaledTimeUpdate) 
            : base(_duration, _runType)
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