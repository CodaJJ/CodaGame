
using System;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A task that you can run a delegate later.
    /// </summary>
    public class FrameDelayActionTask : _AFrameDelayTemplateTask
    {
        private readonly Action _m_delegate;
        
        
        public FrameDelayActionTask(Action _delegate, int _frameCountDelay, ETaskRunType _runType = ETaskRunType.UnscaledTimeUpdate) 
            : base(_frameCountDelay, _runType)
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