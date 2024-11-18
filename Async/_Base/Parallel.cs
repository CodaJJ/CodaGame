
using System;

namespace UnityGameFramework.Base.AsyncOperations
{
    /// <summary>
    /// A parallel operation that can run multiple functions at the same time.
    /// </summary>
    public class Parallel
    {
        // name of the parallel operation
        private readonly string _m_name;
        // count of functions that are running
        private uint _m_functionCount;
        // complete callback
        private Action _m_completeCallback;
        
        
        /// <summary>
        /// Create a new parallel operation with a name.
        /// </summary>
        /// <param name="_name">The name of this operation.</param>
        public Parallel(string _name)
        {
            _m_name = _name;
            _m_functionCount = 0;
            _m_completeCallback = null;
        }
        /// <summary>
        /// Create a new anonymous parallel operation.
        /// </summary>
        public Parallel() : this($"ParallelOperation_{Serialize.NextAsyncParallel()}")
        {
        }
        

        /// <summary>
        /// Run a function in the parallel operation.
        /// </summary>
        /// <param name="_function">The function you want to run.</param>
        public void RunFunction(AsyncFunction _function)
        {
            if (_function == null)
            {
                Console.LogWarning(SystemNames.Async, $"-- {_m_name} -- : Trying to run a null function.");
                return;
            }

            _m_functionCount++;
            Console.LogVerbose(SystemNames.Async, $"-- {_m_name} -- : Starts a new function, now the function count is {_m_functionCount}.");
            
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
                Console.LogWarning(SystemNames.Async, $"-- {_m_name} -- : Trying to add a null complete callback.");
                return;
            }

            Console.LogVerbose(SystemNames.Async, $"-- {_m_name} -- : Adds a complete callback.");
            
            if (_m_functionCount == 0)
                _completeCallback.Invoke();
            else
                _m_completeCallback += _completeCallback;
        }


        /// <summary>
        /// A function is finished.
        /// </summary>
        private void FunctionComplete()
        {
            Action callback = null;

            if (--_m_functionCount == 0) 
            { 
                callback = _m_completeCallback; 
                _m_completeCallback = null;
            }
                
            Console.LogVerbose(SystemNames.Async, $"-- {_m_name} -- : Finishes a function, now the function count is {_m_functionCount}.");
            callback?.Invoke();
        }
    }
}