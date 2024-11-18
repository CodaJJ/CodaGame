
using System;
using UnityGameFramework.Base.Tasks;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A task that you can run a delegate later.
    /// </summary>
    public class FrameDelayActionTask : _AFrameDelayTemplateTask
    {
        private readonly Action _m_delegate;
        
        
        public FrameDelayActionTask(string _name, Action _delegate, int _frameCountDelay, ETaskRunType _runType = ETaskRunType.Update) 
            : base(_name, _frameCountDelay, _runType)
        {
            _m_delegate = _delegate;
        }
        public FrameDelayActionTask(Action _delegate, int _frameCountDelay, ETaskRunType _runType = ETaskRunType.Update)
            : this($"FrameDelayActionTask_{Serialize.NextFrameDelayActionTask()}", _delegate, _frameCountDelay, _runType)
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