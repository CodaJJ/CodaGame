// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using PlayerInputManager = CodaGame.Base.PlayerInputManager;

namespace CodaGame
{
    /// <summary>
    /// Player input that requires manual device management.
    /// </summary>
    public class ManualPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> : _APlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>
        where T_ACTION_MAP_ENUM : Enum
        where T_ACTION_ENUM : Enum
    {
        internal ManualPlayerInput(string _playerName, [NotNull] InputActionAsset _actionAsset,
            [NotNull] _AInputActionPathConfig<T_ACTION_ENUM> _actionPathMappingConfig, 
            [NotNull] _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> _actionMapPathMappingConfig) 
            : base(_playerName, _actionAsset, _actionPathMappingConfig, _actionMapPathMappingConfig)
        {
        }
        
        
        /// <inheritdoc/>
        public sealed override PlayerInputDeviceManagementType deviceManagementType { get { return PlayerInputDeviceManagementType.Manual; } }
        
        
        /// <summary>
        /// Add an input device to this player.
        /// </summary>
        public bool AddDevice(InputDevice _device)
        {
            return PlayerInputManager.instance.AddDeviceTo(this, _device);
        }
        /// <summary>
        /// Remove an input device from this player.
        /// </summary>
        public bool RemoveDevice(InputDevice _device)
        {
            return PlayerInputManager.instance.RemoveDeviceFrom(this, _device);
        }
    }
}