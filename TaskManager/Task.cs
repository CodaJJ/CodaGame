// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Threading;
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
        /// <summary>
        /// Run a action on a separate thread.
        /// </summary>
        /// <param name="_action">The action you want to run on a separate thread.</param>
        /// <param name="_complete">The action to run on the main thread when the separate thread action complete.</param>
        public static void RunThread(Action _action, Action _complete = null)
        {
            if (_action == null)
            {
                RunActionTask(_complete);
                return;
            }
            
            System.Threading.Tasks.Task.Run(_action)
                .ContinueWith(_t =>
                {
                    if (_t.IsFaulted)
                        Console.LogError(SystemNames.Task, $"RunThread Exception: {_t.Exception.InnerException}");

                    RunActionTask(_complete);
                });
        }
        /// <summary>
        /// Run a action with cancellation token on a separate thread.
        /// </summary>
        /// <remarks>
        /// <para>If you want to cancel the action, you can call Cancel() on the returned CancellationTokenSource.</para>
        /// <para>Make sure your action support cancellation by checking the CancellationToken.IsCancellationRequested property.</para>
        /// </remarks>
        /// <param name="_action">The action you want to run on a separate thread.</param>
        /// <param name="_complete">The action to run on the main thread when the separate thread action complete.</param>
        /// <returns>The cancellation token source to cancel the action.</returns>
        public static CancellationTokenSource RunThread(Action<CancellationToken> _action, Action _complete = null)
        {
            if (_action == null)
            {
                RunActionTask(_complete);
                return null;
            }
            
            CancellationTokenSource cts = new CancellationTokenSource();
            System.Threading.Tasks.Task.Run(() => _action?.Invoke(cts.Token), cts.Token)
                .ContinueWith(_t => 
                {
                    if (_t.IsFaulted)
                        Console.LogError(SystemNames.Task, $"RunThread Exception: {_t.Exception.InnerException}");
            
                    RunActionTask(_complete);
                });
    
            return cts;
        }
        /// <summary>
        /// Run a function on a separate thread.
        /// </summary>
        /// <param name="_action">The function you want to run on a separate thread.</param>
        /// <param name="_complete">>The action to run on the main thread when the separate thread function complete, with the function result as parameter.</param>
        /// <typeparam name="T_RESULT">The result type of the function.</typeparam>
        public static void RunThread<T_RESULT>(Func<T_RESULT> _action, Action<T_RESULT> _complete)
        {
            if (_action == null)
            {
                RunActionTask(() => _complete?.Invoke(default));
                return;
            }
            
            System.Threading.Tasks.Task.Run(_action)
                .ContinueWith(_t =>
                {
                    if (_t.IsFaulted)
                        Console.LogError(SystemNames.Task, $"RunThread Exception: {_t.Exception.InnerException}");
                    
                    T_RESULT result = default;
                    if (_t.IsCompletedSuccessfully)
                        result = _t.Result;
                    
                    RunActionTask(() => _complete?.Invoke(result));
                });
        }
        /// <summary>
        /// Run a function with cancellation token on a separate thread.
        /// </summary>
        /// <remarks>
        /// <para>If you want to cancel the function, you can call Cancel() on the returned CancellationTokenSource.</para>
        /// <para>Make sure your function support cancellation by checking the CancellationToken.IsCancellationRequested property.</para>
        /// </remarks>
        /// <param name="_action">The function you want to run on a separate thread.</param>
        /// <param name="_complete">>The action to run on the main thread when the separate thread function complete, with the function result as parameter.</param>
        /// <typeparam name="T_RESULT">The result type of the function.</typeparam>
        /// <returns>The cancellation token source to cancel the function.</returns>
        public static CancellationTokenSource RunThread<T_RESULT>(Func<CancellationToken, T_RESULT> _action, Action<T_RESULT> _complete = null)
        {
            if (_action == null)
            {
                RunActionTask(() => _complete?.Invoke(default));
                return null;
            }
            
            CancellationTokenSource cts = new CancellationTokenSource();
            System.Threading.Tasks.Task.Run(() => _action.Invoke(cts.Token), cts.Token)
                .ContinueWith(_t =>
                {
                    if (_t.IsFaulted)
                        Console.LogError(SystemNames.Task, $"RunThread Exception: {_t.Exception.InnerException}");
                    
                    T_RESULT result = default;
                    if (_t.IsCompletedSuccessfully)
                        result = _t.Result;
                    
                    RunActionTask(() => _complete?.Invoke(result));
                });

            return cts;
        }
    }
}