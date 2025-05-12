// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame.Base
{
    /// <summary>
    /// A continuous task that will run every frame.
    /// </summary>
    public abstract class _AEveryFrameContinuousTask : _AContinuousTask
    {
        protected _AEveryFrameContinuousTask(string _name, UpdateType _runType, bool _useUnscaledTime) 
            : base(_name, _runType, _useUnscaledTime)
        {
        }


        /// <summary>
        /// Execute every frame.
        /// </summary>
        /// <remarks>
        /// <para>This function will be called every frame.</para>
        /// <para>The "_deltaTime" value is determined by the running type of the task and whether using unscaled time.</para>
        /// </remarks>
        protected abstract void OnTick(float _deltaTime);
        
        
        internal override void Tick(float _deltaTime)
        {
            OnTick(_deltaTime);
        }
        
        
        private protected override void OnInternalRun()
        {
        }
        private protected override void OnInternalStop()
        {
        }
    }
}