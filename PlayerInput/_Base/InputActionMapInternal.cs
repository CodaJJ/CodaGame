// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.InputSystem;

namespace CodaGame
{
    public partial class PlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>
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
            /// Enable the action map, increment count by 1
            /// </summary>
            public void Enable()
            {
                if (_m_enabledCount == 0)
                    _m_actionMap.Enable();
                _m_enabledCount++;
            }
            /// <summary>
            /// Disable the action map, decrement count by 1
            /// </summary>
            public void Disable()
            {
                _m_enabledCount--;
                if (_m_enabledCount <= 0)
                {
                    _m_enabledCount = 0;
                    _m_actionMap.Disable();
                }
            }

            public void Dispose() 
            {
                // Nothing to do
            }
        }
    }
}