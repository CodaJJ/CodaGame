// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;

namespace CodaGame
{
    /// <summary>
    /// Base class for value offsets
    /// </summary>
    public abstract class _AValueOffset<T_VALUE>
        where T_VALUE : struct
    {
        // The controller that owns this offset.
        private _AValueController<T_VALUE> _m_controller;
        
        
        /// <summary>
        /// Is the offset finished?
        /// </summary>
        /// <remarks>
        /// <para>True if the offset is finished and will be automatically removed from the controller.</para>
        /// </remarks>
        public abstract bool isFinished { get; }
        
        
        /// <summary>
        /// Update the offset.
        /// </summary>
        protected abstract void Update(float _deltaTime);
        /// <summary>
        /// Evaluate the offset.
        /// </summary>
        protected abstract T_VALUE Evaluate();
        
        
        /// <summary>
        /// Check if the offset is added to a controller.
        /// </summary>
        /// <remarks>
        /// <para><paramref name="_controller"/> is optional. If provided, it checks if this offset is added to that controller.</para>
        /// </remarks>
        internal bool IsOffsetAdded(_AValueController<T_VALUE> _controller = null)
        {
            if (_controller != null)
                return _m_controller == _controller;
            
            return _m_controller != null;    
        }
        /// <summary>
        /// Set the offset to be added to a controller.
        /// </summary>
        internal void SetOffsetAdded(_AValueController<T_VALUE> _controller)
        {
            if (_m_controller != null)
            {
                Console.LogWarning(SystemNames.ValueController, _m_controller.name, "Offset already added to another controller");
                return;
            }

            _m_controller = _controller;
        }
        /// <summary>
        /// Set the offset to be removed from a controller.
        /// </summary>
        internal void SetOffsetRemoved()
        {
            _m_controller = null;   
        }
        /// <summary>
        /// Update the offset.
        /// </summary>
        /// <remarks>
        /// <para>Just keeps the update function as the same as the behaviours'.</para>
        /// </remarks>
        internal void InternalUpdate(float _deltaTime)
        {
            Update(_deltaTime);
        }
        /// <summary>
        /// Evaluate the offset.
        /// </summary>
        /// <remarks>
        /// <para>Just keeps the evaluate function as the same as the behaviours'.</para>
        /// </remarks>
        internal T_VALUE InternalEvaluate()
        {
            return Evaluate();
        }
    }
}