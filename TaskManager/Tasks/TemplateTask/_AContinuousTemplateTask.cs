
namespace UnityGameFramework.Base.Tasks
{
    /// <summary>
    /// A task that you can continuously run a function.
    /// </summary>
    public abstract class _AContinuousTemplateTask : _ATask
    {
        private readonly float _m_duration;
        private float _m_timeCounter;
        
        
        protected _AContinuousTemplateTask(string _name, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update) 
            : base(_name, _runType)
        {
            _m_duration = _duration;
        }
        

        /// <summary>
        /// The duration of task.
        /// </summary>
        public float duration { get { return _m_duration; } }
        /// <summary>
        /// Remaining time (second).
        /// </summary>
        public float remainingTime { get { return _m_duration - _m_timeCounter; } }


        public sealed override void Deal(float _deltaTime)
        {
            if (_m_timeCounter > _m_duration)
            {
                Stop();
                return;
            }
            
            _m_timeCounter += _deltaTime;
            TemplateTaskDeal();
        }

        protected sealed override void OnRun()
        {
            _m_timeCounter = 0;

            OnTemplateTaskRun();
        }
        protected sealed override void OnStop()
        {
            OnTemplateTaskStop();
        }

        protected abstract void TemplateTaskDeal();
        protected abstract void OnTemplateTaskRun();
        protected abstract void OnTemplateTaskStop();
    }
}