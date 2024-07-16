
using System;
using System.Collections.Generic;
using UnityGameFramework.Base.AsyncOperations;

namespace UnityGameFramework
{
    /// <summary>
    /// A class that contains some async functions.
    /// </summary>
    public static class Async
    {
        /// <summary>
        /// A delegate that is used by async functions. 
        /// </summary>
        public delegate void Function(Action _complete);
        
        
        /// <summary>
        /// Operate a list of units in parallel.
        /// </summary>
        /// <param name="_units">The list of units.</param>
        /// <param name="_unitFunction">The operation for units.</param>
        /// <param name="_complete">The complete callback.</param>
        /// <typeparam name="T">The type of units</typeparam>
        public static void Parallel<T>(List<T> _units, Action<T, Action> _unitFunction, Action _complete = null)
        {
            if (_units is not { Count: > 0 } || _unitFunction == null)
            {
                _complete?.Invoke();
                return;
            }
            
            Parallel parallel = new Parallel();
            foreach (T unit in _units)
            {
                parallel.RunFunction(_callback => _unitFunction.Invoke(unit, _callback));
            }
            parallel.AddCompleteCallback(_complete);
        }
        /// <summary>
        /// Run a list of functions in parallel.
        /// </summary>
        /// <param name="_functions">The functions you want to run.</param>
        /// <param name="_complete">The complete callback.</param>
        public static void Parallel(List<Function> _functions, Action _complete = null)
        {
            if (_functions is not { Count: > 0 })
            {
                _complete?.Invoke();
                return;
            }
            
            Parallel parallel = new Parallel();
            foreach (Function function in _functions)
            {
                parallel.RunFunction(function);
            }
            parallel.AddCompleteCallback(_complete);
        }
        /// <summary>
        /// Operate a list of units in waterfall.
        /// </summary>
        /// <param name="_units">The list of units.</param>
        /// <param name="_unitFunction">The operation for units.</param>
        /// <param name="_complete">The complete callback.</param>
        /// <typeparam name="T">The type of units</typeparam>
        public static void Waterfall<T>(List<T> _units, Action<T, Action> _unitFunction, Action _complete = null)
        {
            if (_units is not { Count: > 0 } || _unitFunction == null)
            {
                _complete?.Invoke();
                return;
            }
            
            Waterfall waterfall = new Waterfall();
            foreach (T unit in _units)
            {
                waterfall.AddFunction(_callback => _unitFunction.Invoke(unit, _callback));
            }
            waterfall.AddFunction(_complete);
        }
        /// <summary>
        /// Run a list of functions in waterfall.
        /// </summary>
        /// <param name="_functions">The functions you want to run.</param>
        /// <param name="_complete">The complete callback.</param>
        public static void Waterfall(List<Function> _functions, Action _complete = null)
        {
            if (_functions is not { Count: > 0 })
            {
                _complete?.Invoke();
                return;
            }
            
            Waterfall waterfall = new Waterfall();
            foreach (Function function in _functions)
            {
                waterfall.AddFunction(function);
            }
            waterfall.AddFunction(_complete);
        }
    }
}