// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using CodaGame.Base;

namespace CodaGame
{
    /// <summary>
    /// A facade class that provides convenient static methods for async operations.
    /// </summary>
    public static class Async
    {
        // ------------------------------------------------------------------ Parallel

        /// <summary>
        /// Operate a list of units in parallel.
        /// </summary>
        /// <param name="_units">The list of units to operate on.</param>
        /// <param name="_unitFunction">The async operation applied to each unit.</param>
        /// <param name="_complete">The callback invoked when all operations are finished.</param>
        /// <param name="_name">An optional name for this parallel operation (used for logging).</param>
        /// <typeparam name="T">The type of each unit.</typeparam>
        public static void Parallel<T>(List<T> _units, AsyncFunction<T> _unitFunction, Action _complete = null, string _name = null)
        {
            if (_units is not { Count: > 0 } || _unitFunction == null)
            {
                _complete?.Invoke();
                return;
            }
            
            Parallel parallel = string.IsNullOrEmpty(_name) ? new Parallel() : new Parallel(_name);
            foreach (T unit in _units)
            {
                parallel.RunFunction(_callback => _unitFunction.Invoke(unit, _callback));
            }
            parallel.AddCompleteCallback(_complete);
        }
        /// <summary>
        /// Operate an array of units in parallel.
        /// </summary>
        /// <param name="_units">The array of units to operate on.</param>
        /// <param name="_unitFunction">The async operation applied to each unit.</param>
        /// <param name="_complete">The callback invoked when all operations are finished.</param>
        /// <param name="_name">An optional name for this parallel operation (used for logging).</param>
        /// <typeparam name="T">The type of each unit.</typeparam>
        public static void Parallel<T>(T[] _units, AsyncFunction<T> _unitFunction, Action _complete = null, string _name = null)
        {
            if (_units is not { Length: > 0 } || _unitFunction == null)
            {
                _complete?.Invoke();
                return;
            }
            
            Parallel parallel = string.IsNullOrEmpty(_name) ? new Parallel() : new Parallel(_name);
            foreach (T unit in _units)
            {
                parallel.RunFunction(_callback => _unitFunction.Invoke(unit, _callback));
            }
            parallel.AddCompleteCallback(_complete);
        }
        /// <summary>
        /// Run a list of async functions in parallel.
        /// </summary>
        /// <param name="_functions">The functions to run.</param>
        /// <param name="_complete">The callback invoked when all functions are finished.</param>
        /// <param name="_name">An optional name for this parallel operation (used for logging).</param>
        public static void Parallel(List<AsyncFunction> _functions, Action _complete = null, string _name = null)
        {
            if (_functions is not { Count: > 0 })
            {
                _complete?.Invoke();
                return;
            }
            
            Parallel parallel = string.IsNullOrEmpty(_name) ? new Parallel() : new Parallel(_name);
            foreach (AsyncFunction function in _functions)
            {
                parallel.RunFunction(function);
            }
            parallel.AddCompleteCallback(_complete);
        }
        /// <summary>
        /// Run an array of async functions in parallel.
        /// </summary>
        /// <param name="_functions">The functions to run.</param>
        /// <param name="_complete">The callback invoked when all functions are finished.</param>
        /// <param name="_name">An optional name for this parallel operation (used for logging).</param>
        public static void Parallel(AsyncFunction[] _functions, Action _complete = null, string _name = null)
        {
            if (_functions is not { Length: > 0 })
            {
                _complete?.Invoke();
                return;
            }
            
            Parallel parallel = string.IsNullOrEmpty(_name) ? new Parallel() : new Parallel(_name);
            foreach (AsyncFunction function in _functions)
            {
                parallel.RunFunction(function);
            }
            parallel.AddCompleteCallback(_complete);
        }

        // ------------------------------------------------------------------ Waterfall

        /// <summary>
        /// Operate a list of units in waterfall (one by one, in order).
        /// </summary>
        /// <param name="_units">The list of units to operate on.</param>
        /// <param name="_unitFunction">The async operation applied to each unit.</param>
        /// <param name="_complete">The callback invoked when all operations are finished.</param>
        /// <param name="_name">An optional name for this waterfall operation (used for logging).</param>
        /// <typeparam name="T">The type of each unit.</typeparam>
        public static void Waterfall<T>(List<T> _units, AsyncFunction<T> _unitFunction, Action _complete = null, string _name = null)
        {
            if (_units is not { Count: > 0 } || _unitFunction == null)
            {
                _complete?.Invoke();
                return;
            }
            
            Waterfall waterfall = string.IsNullOrEmpty(_name) ? new Waterfall() : new Waterfall(_name);
            foreach (T unit in _units)
            {
                waterfall.AddFunction(_callback => _unitFunction.Invoke(unit, _callback));
            }
            waterfall.AddFunction(_complete);
        }
        /// <summary>
        /// Operate an array of units in waterfall (one by one, in order).
        /// </summary>
        /// <param name="_units">The array of units to operate on.</param>
        /// <param name="_unitFunction">The async operation applied to each unit.</param>
        /// <param name="_complete">The callback invoked when all operations are finished.</param>
        /// <param name="_name">An optional name for this waterfall operation (used for logging).</param>
        /// <typeparam name="T">The type of each unit.</typeparam>
        public static void Waterfall<T>(T[] _units, AsyncFunction<T> _unitFunction, Action _complete = null, string _name = null)
        {
            if (_units is not { Length: > 0 } || _unitFunction == null)
            {
                _complete?.Invoke();
                return;
            }
            
            Waterfall waterfall = string.IsNullOrEmpty(_name) ? new Waterfall() : new Waterfall(_name);
            foreach (T unit in _units)
            {
                waterfall.AddFunction(_callback => _unitFunction.Invoke(unit, _callback));
            }
            waterfall.AddFunction(_complete);
        }
        /// <summary>
        /// Run a list of async functions in waterfall (one by one, in order).
        /// </summary>
        /// <param name="_functions">The functions to run.</param>
        /// <param name="_complete">The callback invoked when all functions are finished.</param>
        /// <param name="_name">An optional name for this waterfall operation (used for logging).</param>
        public static void Waterfall(List<AsyncFunction> _functions, Action _complete = null, string _name = null)
        {
            if (_functions is not { Count: > 0 })
            {
                _complete?.Invoke();
                return;
            }
            
            Waterfall waterfall = string.IsNullOrEmpty(_name) ? new Waterfall() : new Waterfall(_name);
            foreach (AsyncFunction function in _functions)
            {
                waterfall.AddFunction(function);
            }
            waterfall.AddFunction(_complete);
        }
        /// <summary>
        /// Run an array of async functions in waterfall (one by one, in order).
        /// </summary>
        /// <param name="_functions">The functions to run.</param>
        /// <param name="_complete">The callback invoked when all functions are finished.</param>
        /// <param name="_name">An optional name for this waterfall operation (used for logging).</param>
        public static void Waterfall(AsyncFunction[] _functions, Action _complete = null, string _name = null)
        {
            if (_functions is not { Length: > 0 })
            {
                _complete?.Invoke();
                return;
            }
            
            Waterfall waterfall = string.IsNullOrEmpty(_name) ? new Waterfall() : new Waterfall(_name);
            foreach (AsyncFunction function in _functions)
            {
                waterfall.AddFunction(function);
            }
            waterfall.AddFunction(_complete);
        }
    }
}