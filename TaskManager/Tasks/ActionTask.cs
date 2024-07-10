
using System;
using UnityGameFramework.Base;

namespace UnityGameFramework.Base.Tasks
{
    /// <summary>
    /// A delegate task.
    /// </summary>
    public class ActionTask : _ATask
    {
        private readonly Action _m_delegate;
        
        
        public ActionTask(Action _delegate, ETaskRunType _runType = ETaskRunType.Update) : base(_runType)
        {
            _m_delegate = _delegate;
        }
        
        
        /// <inheritdoc/>
        public override string name { get { return "ActionTask"; } }
        

        public override void Deal(float _deltaTime)
        {
            _m_delegate?.Invoke();
            Stop();
        }

        
        protected override void OnRun()
        {
        }
        protected override void OnStop()
        {
        }
    }
}