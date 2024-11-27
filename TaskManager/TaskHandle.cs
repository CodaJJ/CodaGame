// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;

namespace CodaGame
{
    /// <summary>
    /// The handle for a specify task that can be stopped.
    /// </summary>
    public readonly struct TaskHandle
    {
        private readonly _ABaseTask _m_task;


        internal TaskHandle(_ABaseTask _task)
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