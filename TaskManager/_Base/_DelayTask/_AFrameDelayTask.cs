// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame.Base
{
    /// <summary>
    /// Base class for frame delay task.
    /// </summary>
    /// <remarks>
    /// <para>Frame delay task will execute after a certain frame.</para>
    /// <para>If the delay frame is 0 or negative, the task will execute right after next frame.</para>
    /// </remarks>
    public abstract class _AFrameDelayTask : _ABaseTask, IComparable<_AFrameDelayTask>
    {
        private readonly int _m_delayFrame;
        private int _m_executeFrame;
        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// <para>If the "_delayFrame" is 0 or negative, the task will execute right after next frame.</para>
        /// </remarks>
        protected _AFrameDelayTask(string _name, int _delayFrame, UpdateType _runType) 
            : base(_name, _runType)
        {
            _m_delayFrame = _delayFrame;
        }
        
        
        /// <summary>
        /// Delay frame.
        /// </summary>
        /// <remarks>
        /// <para>How many frames to wait before the task execute.</para>
        /// <para>If the delay frame is 0 or negative, the task will execute right after next frame.</para>
        /// </remarks>
        public int DelayFrame { get { return _m_delayFrame; } }
        /// <summary>
        /// Execute frame.
        /// </summary>
        /// <remarks>
        /// <para>How many TaskManager frames have passed when the task execute.</para>
        /// <para>It's counted by TaskManager, not Unity's frame.</para>
        /// <para>Just a internal property for delay calculation.</para>
        /// </remarks>
        internal int ExecuteFrame { get { return _m_executeFrame; } }
        
        
        /// <summary>
        /// Execute function.
        /// </summary>
        /// <remarks>
        /// <para>This function will be called after the delay frame.</para>
        /// <para>If the delay frame is 0 or negative, the task will execute right after next frame.</para>
        /// </remarks>
        protected abstract void OnExecute();
        
        
        internal void Execute()
        {
            OnExecute();
        }
        internal void SetExecuteFrame(int _executeFrame)
        {
            _m_executeFrame = _executeFrame;
        }

        
        private protected sealed override void AddToUpdateTaskSystem()
        {
            TaskManager.instance.AddUpdateFrameDelayTask(this);
        }
        private protected sealed override void AddToFixedUpdateTaskSystem()
        {
            TaskManager.instance.AddFixedUpdateFrameDelayTask(this);
        }
        private protected sealed override void AddToLateUpdateTaskSystem()
        {
            TaskManager.instance.AddLateUpdateFrameDelayTask(this);
        }
        private protected sealed override void RemoveFromUpdateTaskSystem()
        {
            TaskManager.instance.RemoveUpdateFrameDelayTask(this);
        }
        private protected sealed override void RemoveFromFixedUpdateTaskSystem()
        {
            TaskManager.instance.RemoveFixedUpdateFrameDelayTask(this);
        }
        private protected sealed override void RemoveFromLateUpdateTaskSystem()
        {
            TaskManager.instance.RemoveLateUpdateFrameDelayTask(this);
        }

        private protected override void OnInternalRun()
        {
        }
        private protected override void OnInternalStop()
        {
        }
        

        int IComparable<_AFrameDelayTask>.CompareTo(_AFrameDelayTask _other)
        {
            if (_other == null)
                return 1;
            
            return _m_executeFrame.CompareTo(_other._m_executeFrame);
        }
    }
}