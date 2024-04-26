
using UnityGameFramework.TaskBase;

namespace UnityGameFramework
{
    /// <summary>
    /// The handle for a specify task that can be stopped.
    /// </summary>
    public readonly struct TaskHandle
    {
        private readonly _ATask _m_task;


        internal TaskHandle(_ATask _task)
        {
            _m_task = _task;
        }


        /// <summary>
        /// Stop the task!
        /// </summary>
        public void StopTask()
        {
            if (_m_task is { isRunning: true })
                _m_task.Stop();
        }
    }
}