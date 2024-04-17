
using UnityEngine;

namespace UnityGameFramework.Tasks
{
    /// <summary>
    /// Inherit this class, override abstract function so that you can do what you want.
    /// </summary>
    public abstract class _ATask
    {
        // Running type
        private readonly ETaskRunType _m_runType;
        // Is task running?
        private bool _m_isRunning;
        
        
        /// <summary>
        /// Construct this task, choose which type you want.
        /// </summary>
        protected _ATask(ETaskRunType _runType)
        {
            _m_runType = _runType;
            _m_isRunning = false;
        }
        

        /// <summary>
        /// Is task running.
        /// </summary>
        public bool isRunning { get { return _m_isRunning; } }
        

        /// <summary>
        /// Start this task.
        /// </summary>
        public void Run()
        {
            if (_m_isRunning)
            {
                Debug.LogWarning("[Task Warning] : task already running.");
                return;
            }

            _m_isRunning = true;

            switch (_m_runType)
            {
                case ETaskRunType.Update:
                    TaskManager.instance.AddUpdateTask(this);
                    break;
                case ETaskRunType.LateUpdate:
                    TaskManager.instance.AddLateUpdateTask(this);
                    break;
                case ETaskRunType.FixedUpdate:
                    TaskManager.instance.AddFixedUpdateTask(this);
                    break;
                case ETaskRunType.UnscaledFixedUpdate:
                    TaskManager.instance.AddUnscaledFixedUpdateTask(this);
                    break;
                case ETaskRunType.UnscaledTimeUpdate:
                    TaskManager.instance.AddUnscaledTimeUpdateTask(this);
                    break;
                case ETaskRunType.UnscaledTimeLateUpdate:
                    TaskManager.instance.AddUnscaledTimeLateUpdateTask(this);
                    break;
                default:
                    Debug.LogError($"[Task Error] : Unsupported task run type ({_m_runType}).");
                    break;
            }

            OnRun();
        }
        /// <summary>
        /// Stop this task.
        /// </summary>
        public void Stop()
        {
            if (!_m_isRunning)
            {
                Debug.LogWarning("[Task Warning] : task not run yet, you trying to stop it.");
                return;
            }

            OnStop();

            switch (_m_runType)
            {
                case ETaskRunType.Update:
                    TaskManager.instance.RemoveUpdateTask(this);
                    break;
                case ETaskRunType.LateUpdate:
                    TaskManager.instance.RemoveLateUpdateTask(this);
                    break;
                case ETaskRunType.FixedUpdate:
                    TaskManager.instance.RemoveFixedUpdateTask(this);
                    break;
                case ETaskRunType.UnscaledFixedUpdate:
                    TaskManager.instance.RemoveUnscaledFixedUpdateTask(this);
                    break;
                case ETaskRunType.UnscaledTimeUpdate:
                    TaskManager.instance.RemoveUnscaledTimeUpdateTask(this);
                    break;
                case ETaskRunType.UnscaledTimeLateUpdate:
                    TaskManager.instance.RemoveUnscaledTimeLateUpdateTask(this);
                    break;
                default:
                    Debug.LogError($"[Task Error] : Unsupported task run type ({_m_runType}).");
                    break;
            }

            _m_isRunning = false;
        }

        /// <summary>
        /// Deal you task tick.
        /// </summary>
        public abstract void Deal(float _deltaTime);
        
        /// <summary>
        /// Do something on task run.
        /// </summary>
        protected abstract void OnRun();
        /// <summary>
        /// Do something on task stop.
        /// </summary>
        protected abstract void OnStop();
    }
}