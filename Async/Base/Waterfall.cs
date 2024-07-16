
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace UnityGameFramework.Base.AsyncOperations
{
    /// <summary>
    /// A waterfall operation that you can run a list of functions in order.
    /// </summary>
    public class Waterfall
    {
        // unique id for anonymous waterfall operation
        private static uint _g_uid = 0;

        // name of the waterfall operation
        private readonly string _m_name;
        // queue of functions
        [NotNull] private readonly Queue<Async.Function> _m_functionQueue;
        // is the operation running
        private bool _m_isRunning;


        /// <summary>
        /// Create a new waterfall operation with a name.
        /// </summary>
        /// <param name="_name">The name of this operation.</param>
        public Waterfall(string _name)
        {
            _m_name = _name;
            
            _m_functionQueue = new Queue<Async.Function>();
            _m_isRunning = false;
        }
        /// <summary>
        /// Create a new anonymous waterfall operation.
        /// </summary>
        public Waterfall() : this($"AnonymousWaterfallOperation_{_g_uid++}")
        {
        }
        
        
        /// <summary>
        /// Add a function to the waterfall operation.
        /// </summary>
        /// <remarks>
        /// It will run immediately if there is no function running.
        /// </remarks>
        /// <param name="_function">The function you want to run.</param>
        public void AddFunction(Async.Function _function)
        {
            if (_function == null)
            {
                Console.LogWarning(SystemNames.Async, $"{_m_name} is trying to run a null function.");
                return;
            }

            _m_functionQueue.Enqueue(_function);
            Console.LogVerbose(SystemNames.Async, $"{_m_name} enqueue a new function, now the function count is {_m_functionQueue.Count}.");
            

            TryToRunTheNextFunction();
        }
        /// <summary>
        /// Add a function to the waterfall operation.
        /// </summary>
        /// <remarks>
        /// It will run immediately if there is no function running.
        /// </remarks>
        /// <param name="_function">The function you want to run.</param>
        public void AddFunction(Action _function)
        {
            if (_function == null)
            {
                Console.LogWarning(SystemNames.Async, $"{_m_name} is trying to run a null function.");
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
            
            Console.LogVerbose(SystemNames.Async, $"{_m_name} starts a function, now the function count is {_m_functionQueue.Count}.");
            Async.Function function = _m_functionQueue.Dequeue();
            function?.Invoke(() =>
            {
                Console.LogVerbose(SystemNames.Async, $"{_m_name} finishes a function, now the function count is {_m_functionQueue.Count}.");
                _m_isRunning = false;
                
                TryToRunTheNextFunction();       
            });
        }
    }
}