
using System;
using UnityGameFramework.Base;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A delegate task.
    /// </summary>
    public class ActionTask : _ATask
    {
        private readonly Action _m_delegate;
        
        
        public ActionTask(string _name, Action _delegate, ETaskRunType _runType = ETaskRunType.Update) : base(_name, _runType)
        {
            _m_delegate = _delegate;
        }
        public ActionTask(Action _delegate, ETaskRunType _runType = ETaskRunType.Update)
            : this($"ActionTask_{Serialize.NextActionTask()}", _delegate, _runType)
        {
        }


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