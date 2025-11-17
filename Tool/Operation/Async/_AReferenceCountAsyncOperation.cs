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


        protected _AReferenceCountAsyncOperation()
        {
            _m_referenceCount = 0;
            _m_isProcessing = false;
            _m_isStarted = false;
        }


        /// <summary>
        /// Starts the asynchronous operation, incrementing the reference count.
        /// </summary>
        public void Start()
        {
            _m_referenceCount++;
            TryUpdateOperation();
        }
        /// <summary>
        /// Ends the asynchronous operation, decrementing the reference count.
        /// </summary>
        public void End()
        {
            _m_referenceCount--;
            TryUpdateOperation();
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