// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
            // Owns every action in this map, including actions without an enum mapping.
            [NotNull] private readonly Dictionary<InputAction, InputActionInternal> _m_actions;
            // Enable count
            private int _m_enabledCount;


            public InputActionMapInternal([NotNull] _APlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _playerInput,
                [NotNull] InputActionMap _actionMap)
            {
                _m_actions = new Dictionary<InputAction, InputActionInternal>();
                _m_enabledCount = 1;
                foreach (InputAction action in _actionMap.actions)
                    _m_actions.Add(action, new InputActionInternal(_playerInput, this, action));
            }


            public bool isEnabled { get { return _m_enabledCount > 0; } }


            public InputActionInternal GetAction([NotNull] InputAction _action)
            {
                return _m_actions.GetValueOrDefault(_action);
            }
            public void AdvanceLogicFrame(int _logicFrame)
            {
                foreach (InputActionInternal action in _m_actions.Values)
                    action.AdvanceLogicFrame(_logicFrame);
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
                    RefreshActions();
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
                    RefreshActions();
            }

            public void Dispose()
            {
                foreach (InputActionInternal action in _m_actions.Values)
                    action.Dispose();
                _m_actions.Clear();
            }


            private void RefreshActions()
            {
                foreach (InputActionInternal action in _m_actions.Values)
                    action.RefreshEnabled();
            }
        }
    }
}
