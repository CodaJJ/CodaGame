
using System;
using JetBrains.Annotations;

namespace UnityGameFramework.Base.AsyncOperations
{
    /// <summary>
    /// A parallel operation that can run multiple functions at the same time.
    /// </summary>
    public class Parallel
    {
        // unique id for anonymous parallel operation
        private static uint _g_uid = 0;
        
        // name of the parallel operation
        private readonly string _m_name;
        // count of functions that are running
        private uint _m_functionCount;
        // complete callback
        private Action _m_completeCallback;
        // lock for thread safety
        [NotNull] private readonly object _m_lock;
        
        
        /// <summary>
        /// Create a new parallel operation with a name.
        /// </summary>
        /// <param name="_name">The name of this operation.</param>
        public Parallel(string _name)
        {
            _m_name = _name;
            _m_functionCount = 0;
            _m_completeCallback = null;
            
            _m_lock = new object();
        }
        /// <summary>
        /// Create a new anonymous parallel operation.
        /// </summary>
        public Parallel() : this($"AnonymousParallelOperation_{_g_uid++}")
        {
        }
        

        /// <summary>
        /// Run a function in the parallel operation.
        /// </summary>
        /// <param name="_function">The function you want to run.</param>
        public void RunFunction(Async.Function _function)
        {
            if (_function == null)
            {
                Console.LogWarning(SystemNames.Async, $"{_m_name} is trying to run a null function.");
                return;
            }

            lock (_m_lock)
            {
                _m_functionCount++;
                Console.LogVerbose(SystemNames.Async, $"{_m_name} starts a new function, now the function count is {_m_functionCount}.");
            }
            
            _function.Invoke(FunctionComplete);
        }
        /// <summary>
        /// Add a complete callback to the parallel operation.
        /// </summary>
        /// <remarks>
        /// <para>The complete callback will be invoked when all functions are finished.</para>
        /// <para>Also the complete callback will be invoked immediately if there are nothing to run.</para>
        /// </remarks>
        /// <param name="_completeCallback">The complete callback you want.</param>
        public void AddCompleteCallback(Action _completeCallback)
        {
            if (_completeCallback == null)
            {
                Console.LogWarning(SystemNames.Async, $"{_m_name} is trying to add a null complete callback.");
                return;
            }

            Console.LogVerbose(SystemNames.Async, $"{_m_name} adds a new complete callback, now the function count is {_m_functionCount}.");
            
            bool invokeImmediately = false;
            lock (_m_lock)
            {
                if (_m_functionCount == 0)
                    invokeImmediately = true;
                else
                    _m_completeCallback += _completeCallback;
            }
            
            if (invokeImmediately)
                _completeCallback.Invoke();
        }


        /// <summary>
        /// A function is finished.
        /// </summary>
        private void FunctionComplete()
        {
            Action callback = null;

            lock (_m_lock)
            {
                if (--_m_functionCount == 0)
                {
                    callback = _m_completeCallback;
                    _m_completeCallback = null;
                }
                
                Console.LogVerbose(SystemNames.Async, $"{_m_name} finishes a function, now the function count is {_m_functionCount}.");
            }

            callback?.Invoke();
        }
    }
}