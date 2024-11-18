
using System;
using UnityGameFramework.Base.Tasks;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A delay task which will run a delegate delay.
    /// </summary>
    public class DelayActionTask : _ADelayTemplateTask
    {
        private readonly Action _m_delegate;
        
        
        public DelayActionTask(string _name, Action _delegate, float _delayTime, ETaskRunType _runType = ETaskRunType.Update) 
            : base(_name, _delayTime, _runType)
        {
            _m_delegate = _delegate;
        }
        public DelayActionTask(Action _delegate, float _delayTime, ETaskRunType _runType = ETaskRunType.Update)
            : this($"DelayActionTask_{Serialize.NextDelayActionTask()}", _delegate, _delayTime, _runType)
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