// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using JetBrains.Annotations;
using UnityEngine.InputSystem;

namespace CodaGame
{
    /// <summary>
    /// Player input that automatically uses all available input devices.
    /// </summary>
    /// <remarks>
    /// This player will receive input from all connected devices and automatically
    /// get new devices when they are connected.
    /// </remarks>
    public class AllDevicesPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> : _APlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>
        where T_ACTION_MAP_ENUM : Enum
        where T_ACTION_ENUM : Enum
    {
        internal AllDevicesPlayerInput(string _playerName, [NotNull] InputActionAsset _actionAsset,
            [NotNull] _AInputActionPathConfig<T_ACTION_ENUM> _actionPathMappingConfig,
            [NotNull] _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> _actionMapPathMappingConfig)
            : base(_playerName, _actionAsset, _actionPathMappingConfig, _actionMapPathMappingConfig)
        {
        }
        
        
        /// <inheritdoc/>
        public sealed override PlayerInputDeviceManagementType deviceManagementType { get { return PlayerInputDeviceManagementType.AllDevices; } }
    }
}