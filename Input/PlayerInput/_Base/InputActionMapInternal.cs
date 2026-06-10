// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.InputSystem;

namespace CodaGame.Base
{
    public partial class _APlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>
        where T_ACTION_MAP_ENUM : Enum
        where T_ACTION_ENUM : Enum
    {
        // Internal action map management class
        private class InputActionMapInternal
        {
            // Action map object
            [NotNull] private readonly InputActionMap _m_actionMap;
            // Enable count
            private int _m_enabledCount;
            
            
            public InputActionMapInternal([NotNull] InputActionMap _actionMap)
            {
                _m_actionMap = _actionMap;
                _m_actionMap.Enable();
                _m_enabledCount = 1;
            }


            /// <summary>
            /// Enable the action map, increment count by 1.
            /// </summary>
            /// <remarks>
            /// The count is a net count and may be negative (more outstanding Disables than Enables,
            /// e.g. a blanket input block over a map that was already disabled). The map is active
            /// only while the count is positive, so Enable/Disable pairs from independent callers
            /// interleave safely and restore the map's prior state.
            /// </remarks>
            public void Enable()
            {
                _m_enabledCount++;
                if (_m_enabledCount == 1)
                    _m_actionMap.Enable();
            }
            /// <summary>
            /// Disable the action map, decrement count by 1.
            /// </summary>
            /// <remarks>
            /// Must be paired one-to-one with <see cref="Enable"/> calls; see Enable for the
            /// net-count semantics.
            /// </remarks>
            public void Disable()
            {
                _m_enabledCount--;
                if (_m_enabledCount == 0)
                    _m_actionMap.Disable();
            }

            public void Dispose() 
            {
                // Nothing to do
            }
        }
    }
}