// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using CodaGame.Base;
using JetBrains.Annotations;
using UnityEngine.InputSystem;

namespace CodaGame
{
    /// <summary>
    /// Player input that prefers specific types of input devices with priority order.
    /// </summary>
    /// <remarks>
    /// <para>This player will prioritize devices based on the preferred types list.</para>
    /// <para>It will try to assign devices in order of preference (first choice, second choice, etc.).</para>
    /// <para>When a higher-priority device becomes available, it will automatically switch to it.</para>
    /// </remarks>
    public class PreferredDevicePlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> : _APlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>, _IInputPreferredDeviceUser
        where T_ACTION_MAP_ENUM : Enum
        where T_ACTION_ENUM : Enum
    {
        [NotNull] private readonly List<PreferInputDeviceType> _m_preferredTypes;


        internal PreferredDevicePlayerInput(string _playerName, [NotNull] InputActionAsset _actionAsset,
            [NotNull] _AInputActionPathConfig<T_ACTION_ENUM> _actionPathMappingConfig,
            [NotNull] _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> _actionMapPathMappingConfig,
            [NotNull] List<PreferInputDeviceType> _preferredTypes)
            : base(_playerName, _actionAsset, _actionPathMappingConfig, _actionMapPathMappingConfig)
        {
            _m_preferredTypes = new List<PreferInputDeviceType>(_preferredTypes);
        }


        /// <summary>
        /// The preferred device types in priority order (first = highest priority).
        /// </summary>
        public ReadOnlyList<PreferInputDeviceType> preferredTypes { get { return _m_preferredTypes; } }
        /// <inheritdoc/>
        public sealed override PlayerInputDeviceManagementType deviceManagementType { get { return PlayerInputDeviceManagementType.Preferred; } }
    }
}