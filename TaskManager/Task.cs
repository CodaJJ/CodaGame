// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Tasks;

namespace CodaGame
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
        /// <para>
        /// You can use this task to run a delegate on the main thread,
        /// specially Unity's function which is only runnable on the main thread.
        /// </para>
        /// </remarks>
        /// <param name="_delegate">The delegate you want to run</param>
        /// <param name="_runType">When will the task run</param>
        public static void RunActionTask(Action _delegate, UpdateType _runType = UpdateType.Update)
        {
            new ActionTask(_delegate, _runType).Run();
        }
        /// <summary>
        /// Run a delay action task.
        /// </summary>
        /// <param name="_delegate">The delegate you want to run.</param>
        /// <param name="_delayTime">The delay time before running (seconds).</param>
        /// <param name="_runType">Where does the task run on.</param>
        /// <param name="_useUnscaledTime">Use unscaled time or not.</param>
        /// <returns>The handle of this task.</returns>
        public static TaskHandle RunDelayActionTask(Action _delegate, float _delayTime, UpdateType _runType = UpdateType.Update, bool _useUnscaledTime = false)
        {
            DelayActionTask task = new DelayActionTask(_delegate, _delayTime, _runType, _useUnscaledTime);
            task.Run();
            return new TaskHandle(task);
        }
        /// <summary>
        /// Run a continuous action task.
        /// </summary>
        /// <param name="_delegate">The delegate you want to run, the parameter is the time passed since the last invoke.</param>
        /// <param name="_duration">The duration of this task.</param>
        /// <param name="_runType">Where does the task run on.</param>
        /// <param name="_useUnscaledTime">Use unscaled time or not.</param>
        /// <returns>The handle of this task.</returns>
        public static TaskHandle RunContinuousActionTask(Action<float> _delegate, float _duration = -1, UpdateType _runType = UpdateType.Update, bool _useUnscaledTime = false)
        {
            ContinuousActionTask task = new ContinuousActionTask(_delegate, _duration, _runType, _useUnscaledTime);
            task.Run();
            return new TaskHandle(task);
        }

        /// <summary>
        /// Run a time interval continuous action task.
        /// </summary>
        /// <param name="_delegate">The delegate you want to run, the parameter is the time passed since the last invoke.</param>
        /// <param name="_timeInterval">The time interval of this task.</param>
        /// <param name="_executeOnceImmediately">Execute once immediately or not.</param>
        /// <param name="_duration">The duration of this task, negative or zero means never stop.</param>
        /// <param name="_runType">Where does the task run on.</param>
        /// <param name="_useUnscaledTime">Use unscaled time or not.</param>
        /// <returns>The handle of this task.</returns>
        public static TaskHandle RunTimeIntervalContinuousActionTask(Action<float> _delegate, float _timeInterval, bool _executeOnceImmediately = true, float _duration = -1, UpdateType _runType = UpdateType.Update, bool _useUnscaledTime = false)
        {
            TimeIntervalContinuousActionTask task = new TimeIntervalContinuousActionTask(_delegate, _timeInterval, _executeOnceImmediately, _duration, _runType, _useUnscaledTime);
            task.Run();
            return new TaskHandle(task);
        }
        /// <summary>
        /// Run a frame interval continuous action task.
        /// </summary>
        /// <param name="_delegate">The delegate you want to run, the parameter is the time passed since the last invoke.</param>
        /// <param name="_frameInterval">The frame interval of this task.</param>
        /// <param name="_executeOnceImmediately">Execute once immediately or not.</param>
        /// <param name="_duration">The duration of this task, negative or zero means never stop.</param>
        /// <param name="_runType">Where does the task run on.</param>
        /// <param name="_useUnscaledTime">Use unscaled time or not.</param>
        /// <returns>The handle of this task.</returns>
        public static TaskHandle RunFrameIntervalContinuousActionTask(Action<float> _delegate, int _frameInterval, bool _executeOnceImmediately = true, float _duration = -1, UpdateType _runType = UpdateType.Update, bool _useUnscaledTime = false)
        {
            FrameIntervalContinuousActionTask task = new FrameIntervalContinuousActionTask(_delegate, _frameInterval, _executeOnceImmediately, _duration, _runType, _useUnscaledTime);
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
        public static TaskHandle RunFrameDelayActionTask(Action _delegate, int _frameCountDelay, UpdateType _runType = UpdateType.Update)
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
        public static TaskHandle RunNextFrameActionTask(Action _delegate, UpdateType _runType = UpdateType.Update)
        {
            NextFrameActionTask task = new NextFrameActionTask(_delegate, _runType);
            task.Run();
            return new TaskHandle(task);
        }
    }
}