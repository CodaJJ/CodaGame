// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// An abstract class for reference-counted synchronous operations.
    /// </summary>
    public abstract class _AReferenceCountSyncOperation
    {
        // Reference count to track the number of active operations.
        private int _m_referenceCount;
        // Flag to indicate if the operation has started.
        private bool _m_isStarted;


        protected _AReferenceCountSyncOperation()
        {
            _m_referenceCount = 0;
            _m_isStarted = false;
        }


        /// <summary>
        /// Starts the synchronous operation, incrementing the reference count.
        /// </summary>
        public void Start()
        {
            _m_referenceCount++;
            TryUpdateOperation();
        }
        /// <summary>
        /// Ends the synchronous operation, decrementing the reference count.
        /// </summary>
        public void End()
        {
            _m_referenceCount--;
            TryUpdateOperation();
        }


        /// <summary>
        /// Called when the operation starts.
        /// </summary>
        protected abstract void OnOperationStart();
        /// <summary>
        /// Called when the operation ends.
        /// </summary>
        protected abstract void OnOperationEnd();


        // Tries to update the operation based on the reference count and current state.
        private void TryUpdateOperation()
        {
            if (!_m_isStarted && _m_referenceCount > 0)
            {
                OnOperationStart();
                _m_isStarted = true;
            }
            else if (_m_isStarted && _m_referenceCount <= 0)
            {
                OnOperationEnd();
                _m_isStarted = false;
            }
        }
    }
}
