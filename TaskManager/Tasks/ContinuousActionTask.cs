
using System;
using UnityGameFramework.Base.Tasks;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A task that you can continuously run a delegate.
    /// </summary>
    public class ContinuousActionTask : _AContinuousTemplateTask
    {
        private readonly Action _m_delegate;
        
        
        public ContinuousActionTask(string _name, Action _delegate, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update) 
            : base(_name, _duration, _runType)
        {
            _m_delegate = _delegate;
        }
        public ContinuousActionTask(Action _delegate, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update)
            : this($"ContinuousActionTask_{Serialize.NextContinuousTask()}", _delegate, _duration, _runType)
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