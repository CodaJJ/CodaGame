
namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A task that you can run a function later.
    /// </summary>
    public abstract class _ADelayTemplateTask : _ATask
    {
        private readonly float _m_delayTime;
        private float _m_timeCounter;
        
        
        protected _ADelayTemplateTask(float _delayTime, ETaskRunType _runType = ETaskRunType.UnscaledTimeUpdate) 
            : base(_runType)
        {
            _m_delayTime = _delayTime;
        }
        

        /// <summary>
        /// Delay time (second).
        /// </summary>
        public float delayTime { get { return _m_delayTime; } }
        /// <summary>
        /// Remaining time (second).
        /// </summary>
        public float remainingTime { get { return _m_delayTime - _m_timeCounter; } }
        

        public sealed override void Deal(float _deltaTime)
        {
            if (_m_timeCounter >= _m_delayTime)
            {
                TemplateTaskDeal();
                Stop();
                return;
            }
            _m_timeCounter += _deltaTime;
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