
using System;
using UnityGameFramework.TaskBase;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A task that you can run a delegate at next frame.
    /// </summary>
    public class NextFrameActionTask : _AFrameDelayTemplateTask
    {
        private readonly Action _m_delegate;
        
        
        public NextFrameActionTask(Action _delegate, ETaskRunType _runType = ETaskRunType.Update) 
            : base(1, _runType)
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