// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// A synchronous operation that manages the reference counting of another synchronous operation.
    /// </summary>
    public class ReferenceCountSyncOperation : _AReferenceCountSyncOperation
    {
        // The synchronous operation to be reference counted.
        private readonly _ISyncOperation _m_operation;
        // Callbacks for operation start and end completion.
        private readonly Action _m_operationComplete;
        private readonly Action _m_operationEndComplete;


        public ReferenceCountSyncOperation(_ISyncOperation _operation, Action _operationComplete = null, Action _operationEndComplete = null)
        {
            _m_operation = _operation;
            _m_operationComplete = _operationComplete;
            _m_operationEndComplete = _operationEndComplete;
        }


        /// <summary>
        /// Called when the operation starts.
        /// </summary>
        protected override void OnOperationStart()
        {
            if (_m_operation == null)
            {
                Console.LogWarning(SystemNames.Operation, "Operation is null.");
                return;
            }

            _m_operation.Start();
            _m_operationComplete?.Invoke();
        }
        /// <summary>
        /// Called when the operation ends.
        /// </summary>
        protected override void OnOperationEnd()
        {
            if (_m_operation == null)
            {
                Console.LogWarning(SystemNames.Operation, "Operation is null.");
                return;
            }

            _m_operation.End();
            _m_operationEndComplete?.Invoke();
        }
    }
}
