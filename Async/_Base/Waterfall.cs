// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame.Base.AsyncOperations
{
    /// <summary>
    /// A waterfall operation that you can run a list of functions in order.
    /// </summary>
    public class Waterfall
    {
        // name of the waterfall operation
        private readonly string _m_name;
        // queue of functions
        [NotNull] private readonly Queue<AsyncFunction> _m_functionQueue;
        // is the operation running
        private bool _m_isRunning;


        /// <summary>
        /// Create a new waterfall operation with a name.
        /// </summary>
        /// <param name="_name">The name of this operation.</param>
        public Waterfall(string _name)
        {
            _m_name = _name;
            
            _m_functionQueue = new Queue<AsyncFunction>();
            _m_isRunning = false;
        }
        /// <summary>
        /// Create a new anonymous waterfall operation.
        /// </summary>
        public Waterfall() : this($"WaterfallOperation_{Serialize.NextAsyncWaterfall()}")
        {
        }
        
        
        /// <summary>
        /// Add a function to the waterfall operation.
        /// </summary>
        /// <remarks>
        /// <para>It will run immediately if there is no function running.</para>
        /// </remarks>
        /// <param name="_function">The function you want to run.</param>
        public void AddFunction(AsyncFunction _function)
        {
            if (_function == null)
            {
                Console.LogWarning(SystemNames.Async, $"-- {_m_name} -- : Trying to run a null function.");
                return;
            }

            _m_functionQueue.Enqueue(_function);
            Console.LogVerbose(SystemNames.Async, $"-- {_m_name} -- : Enqueue a new function, now the function count is {_m_functionQueue.Count}.");
            

            TryToRunTheNextFunction();
        }
        /// <summary>
        /// Add a function to the waterfall operation.
        /// </summary>
        /// <remarks>
        /// <para>It will run immediately if there is no function running.</para>
        /// </remarks>
        /// <param name="_function">The function you want to run.</param>
        public void AddFunction(Action _function)
        {
            if (_function == null)
            {
                Console.LogWarning(SystemNames.Async, $"-- {_m_name} -- : Trying to run a null function.");
                return;
            }
            
            AddFunction(_callback =>
            {
                _function.Invoke();
                _callback?.Invoke();
            });
        }
        
        
        /// <summary>
        /// Try to run the next function in the queue.
        /// </summary>
        private void TryToRunTheNextFunction()
        {
            if (_m_isRunning || _m_functionQueue.Count == 0) 
                return;
                
            _m_isRunning = true;
            
            Console.LogVerbose(SystemNames.Async, $"-- {_m_name} -- : Starts a function, now the function count is {_m_functionQueue.Count}.");
            AsyncFunction function = _m_functionQueue.Dequeue();
            function?.Invoke(() =>
            {
                Console.LogVerbose(SystemNames.Async, $"-- {_m_name} -- : Finishes a function, now the function count is {_m_functionQueue.Count}.");
                _m_isRunning = false;
                
                TryToRunTheNextFunction();       
            });
        }
    }
}