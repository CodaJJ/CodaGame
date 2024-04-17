
namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// A task that you can run a function later.
    /// </summary>
    public abstract class _AFrameDelayTemplateTask : _ATask
    {
        private readonly int _m_frameCountDelay;
        private float _m_frameCounter;
        
        
        protected _AFrameDelayTemplateTask(int _frameCountDelay, ETaskRunType _runType = ETaskRunType.UnscaledTimeUpdate) 
            : base(_runType)
        {
            _m_frameCountDelay = _frameCountDelay;
        }
        

        /// <summary>
        /// frame count delay.
        /// </summary>
        public float frameCountDelay { get { return _m_frameCountDelay; } }
        /// <summary>
        /// Remaining frame count.
        /// </summary>
        public float remainingFrameCount { get { return _m_frameCountDelay - _m_frameCounter; } }
        

        public sealed override void Deal(float _deltaTime)
        {
            if (_m_frameCounter >= _m_frameCountDelay)
            {
                TemplateTaskDeal();
                Stop();
                return;
            }
            
            _m_frameCounter++;
        }

        protected sealed override void OnRun()
        {
            _m_frameCounter = 0;

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