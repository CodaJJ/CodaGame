// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// An abstract class for reference-counted asynchronous operations.
    /// </summary>
    public abstract class _AReferenceCountAsyncOperation
    {
        // Reference count to track the number of active operations.
        private int _m_referenceCount;
        // Flag to indicate if an operation is currently being processed.
        private bool _m_isProcessing;
        // Flag to indicate if the operation has started.
        private bool _m_isStarted;
        // Delegate for operation started event.
        private Action _m_operationStartedDelegate;


        protected _AReferenceCountAsyncOperation()
        {
            _m_referenceCount = 0;
            _m_isProcessing = false;
            _m_isStarted = false;
        }


        public int referenceCount { get { return _m_referenceCount; } }
        public bool isStarted { get { return _m_isStarted; } }
        public bool isProcessing { get { return _m_isProcessing; } }


        /// <summary>
        /// Starts the asynchronous operation, incrementing the reference count.
        /// </summary>
        public void Start(Action _complete = null)
        {
            _m_referenceCount++;
            TryUpdateOperation();
            if (_complete != null)
                AddOperationStartedDelegate(_complete);
        }
        /// <summary>
        /// Ends the asynchronous operation, decrementing the reference count.
        /// </summary>
        public void End()
        {
            _m_referenceCount--;
            if (_m_referenceCount < 0)
            {
                Console.LogWarning(SystemNames.Operation, "Reference count went below zero. Resetting to zero.");
                _m_referenceCount = 0;
            }
            
            TryUpdateOperation();
        }
        /// <summary>
        /// Adds a delegate to be called when the operation starts completely.
        /// </summary>
        /// <remarks>
        /// <para>If the operation has already started, the delegate is invoked immediately.</para>
        /// </remarks>
        public void AddOperationStartedDelegate(Action _delegate)
        {
            if (_delegate == null)
                return;

            if (_m_isProcessing)
                return;
            
            if (_m_isStarted)
            {
                _delegate.Invoke();
                return;
            }
            
            _m_operationStartedDelegate += _delegate;
        }
        
        
        /// <summary>
        /// Called when the operation starts.
        /// </summary>
        /// <param name="_complete">Call this action when the operation is complete.</param>
        protected abstract void OnOperationStart([NotNull] Action _complete);
        /// <summary>
        /// Called when the operation ends.
        /// </summary>
        /// <param name="_complete">Call this action when the operation is complete.</param>
        protected abstract void OnOperationEnd([NotNull] Action _complete);


        // Tries to update the operation based on the reference count and current state.
        private void TryUpdateOperation()
        {
            if (_m_isProcessing)
                return;

            if (!_m_isStarted && _m_referenceCount > 0)
            {
                _m_isProcessing = true;
                OnOperationStart(() =>
                {
                    _m_isProcessing = false;
                    _m_isStarted = true;
                    
                    Action delegates = _m_operationStartedDelegate;
                    _m_operationStartedDelegate = null;
                    delegates?.Invoke();
                    
                    TryUpdateOperation();
                });
            }
            else if (_m_isStarted && _m_referenceCount <= 0)
            {
                _m_isProcessing = true;
                OnOperationEnd(() =>
                {
                    _m_isProcessing = false;
                    _m_isStarted = false;
                    TryUpdateOperation();
                });
            }
        }
    }
}