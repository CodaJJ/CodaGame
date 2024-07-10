
using System;
using UnityGameFramework.Base.Tasks;
using UnityGameFramework.Base;

namespace UnityGameFramework
{
    /// <summary>
    /// All sorts of tasks you can find here.
    /// </summary>
    public static class Task
    {
        /// <summary>
        /// Run a action task.
        /// </summary>
        /// <remarks>
        /// You can use this task to run a delegate on the main thread, specially Unity's function which is only runnable on the main thread.
        /// </remarks>
        /// <param name="_delegate">The delegate you want to run</param>
        /// <param name="_runType">When will the task run</param>
        public static void RunActionTask(Action _delegate, ETaskRunType _runType = ETaskRunType.Update)
        {
            new ActionTask(_delegate, _runType).Run();
        }
        /// <summary>
        /// Run a delay action task.
        /// </summary>
        /// <param name="_delegate">The delegate you want to run.</param>
        /// <param name="_delayTime">The delay time before running (seconds).</param>
        /// <param name="_runType">Where does the task run on.</param>
        /// <returns>The handle of this task.</returns>
        public static TaskHandle RunDelayActionTask(Action _delegate, float _delayTime, ETaskRunType _runType = ETaskRunType.Update)
        {
            DelayActionTask task = new DelayActionTask(_delegate, _delayTime, _runType);
            task.Run();
            return new TaskHandle(task);
        }
        /// <summary>
        /// Run a continuous action task.
        /// </summary>
        /// <param name="_delegate">The delegate you want to run.</param>
        /// <param name="_duration">The duration of this task.</param>
        /// <param name="_runType">Where does the task run on.</param>
        /// <returns>The handle of this task.</returns>
        public static TaskHandle RunContinuousActionTask(Action _delegate, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update)
        {
            ContinuousActionTask task = new ContinuousActionTask(_delegate, _duration, _runType);
            task.Run();
            return new TaskHandle(task);
        }
        /// <summary>
        /// Run a fixed interval continuous action task.
        /// </summary>
        /// <param name="_delegate">The delegate you want to run.</param>
        /// <param name="_fixedInterval">The fixed interval of this task.</param>
        /// <param name="_duration">The duration of this task.</param>
        /// <param name="_runType">Where does the task run on.</param>
        /// <returns>The handle of this task.</returns>
        public static TaskHandle RunFixedIntervalContinuousActionTask(Action _delegate, float _fixedInterval, float _duration = float.PositiveInfinity, ETaskRunType _runType = ETaskRunType.Update)
        {
            FixedIntervalContinuousActionTask task = new FixedIntervalContinuousActionTask(_delegate, _fixedInterval, _duration, _runType);
            task.Run();
            return new TaskHandle(task);
        }
        /// <summary>
        /// Run a frame delay action task.
        /// </summary>
        /// <param name="_delegate">The delegate you want to run.</param>
        /// <param name="_frameCountDelay">The delay frame count before running.</param>
        /// <param name="_runType">Where does the task run on.</param>
        /// <returns>The handle of this task.</returns>
        public static TaskHandle RunFrameDelayActionTask(Action _delegate, int _frameCountDelay, ETaskRunType _runType = ETaskRunType.Update)
        {
            FrameDelayActionTask task = new FrameDelayActionTask(_delegate, _frameCountDelay, _runType);
            task.Run();
            return new TaskHandle(task);
        }
        /// <summary>
        /// Run a next frame action task.
        /// </summary>
        /// <param name="_delegate">The delegate you want to run.</param>
        /// <param name="_runType">Where does the task run on.</param>
        /// <returns>The handle of this task.</returns>
        public static TaskHandle RunNextFrameActionTask(Action _delegate, ETaskRunType _runType = ETaskRunType.Update)
        {
            NextFrameActionTask task = new NextFrameActionTask(_delegate, _runType);
            task.Run();
            return new TaskHandle(task);
        }
    }
}