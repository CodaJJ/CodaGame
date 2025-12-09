// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using PlayerInputManager = CodaGame.Base.PlayerInputManager;

namespace CodaGame
{
    /// <summary>
    /// Public API for the CodaGame input system.
    /// </summary>
    /// <remarks>
    /// This class wraps the internal PlayerInputManager and provides a clean public interface.
    /// </remarks>
    public static class Input
    {
        [NotNull] private static PlayerInputManager manager { get { return PlayerInputManager.instance; } }
        
            
        /// <summary>
        /// Event fired when a device is connected.
        /// </summary>
        public static event Action<InputDevice> onDeviceConnected { add { manager.onDeviceConnected += value; } remove { manager.onDeviceConnected -= value; } }
        /// <summary>
        /// Event fired when a device is disconnected.
        /// </summary>
        public static event Action<InputDevice> onDeviceDisconnected { add { manager.onDeviceDisconnected += value; } remove { manager.onDeviceDisconnected -= value; } }

        /// <summary>
        /// All connected input devices.
        /// </summary>
        [ItemNotNull] public static ReadOnlyList<InputDevice> allDevices { get { return manager.allDevices; } }


        /// <summary>
        /// Get the number of input users using the specified device.
        /// </summary>
        /// <param name="_device">The input device.</param>
        /// <param name="_deviceUserTypeMask">The device user type mask to filter by. Defaults to all types.</param>
        /// <returns>The number of users using this device that match the type mask.</returns>
        public static int GetDeviceUserCount(InputDevice _device, PlayerInputDeviceManagementType _deviceUserTypeMask = PlayerInputDeviceManagementType.Manual | PlayerInputDeviceManagementType.Preferred | PlayerInputDeviceManagementType.AllDevices)
        {
            return manager.GetDeviceUserCount(_device, _deviceUserTypeMask);
        }
        /// <summary>
        /// Adds an all-devices player input user that automatically receives input from all connected devices.
        /// </summary>
        /// <remarks>
        /// <para>All-devices players will automatically receive new devices when they are connected.</para>
        /// <para>When devices are disconnected, they will be automatically removed from the player.</para>
        /// </remarks>
        /// <param name="_playerName">The name of the player.</param>
        /// <param name="_actionAsset">The InputActionAsset for the player.</param>
        /// <param name="_actionPathMappingConfig">The action path mapping configuration.</param>
        /// <param name="_actionMapPathMappingConfig">The action map path mapping configuration.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        /// <returns>The created all-devices player input.</returns>
        public static AllDevicesPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> AddAllDevicesPlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(
            string _playerName,
            InputActionAsset _actionAsset,
            _AInputActionPathConfig<T_ACTION_ENUM> _actionPathMappingConfig,
            _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> _actionMapPathMappingConfig)
            where T_ACTION_MAP_ENUM : Enum
            where T_ACTION_ENUM : Enum
        {
            return manager.AddAllDevicesPlayer(_playerName, _actionAsset, _actionPathMappingConfig, _actionMapPathMappingConfig);
        }
        /// <summary>
        /// Removes an all-devices player input user.
        /// </summary>
        /// <param name="_player">The all-devices player to remove.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        public static void RemoveAllDevicesPlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(
            [NotNull] AllDevicesPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _player)
            where T_ACTION_MAP_ENUM : Enum
            where T_ACTION_ENUM : Enum
        {
            manager.RemoveAllDevicesPlayer(_player);
        }
        /// <summary>
        /// Adds a preferred device player input user that prioritizes specific types of input devices.
        /// </summary>
        /// <remarks>
        /// <para>The player will try to claim devices based on the priority list (first = highest priority).</para>
        /// <para>When a higher-priority device becomes available, it will automatically switch to it.</para>
        /// <para>Devices can be claimed from lower-priority users in the user list.</para>
        /// </remarks>
        /// <param name="_playerName">The name of the player.</param>
        /// <param name="_actionAsset">The InputActionAsset for the player.</param>
        /// <param name="_actionPathMappingConfig">The action path mapping configuration.</param>
        /// <param name="_actionMapPathMappingConfig">The action map path mapping configuration.</param>
        /// <param name="_preferredTypes">
        /// The preferred device types in priority order (first = highest priority).
        /// If null, defaults to [KeyboardMouse, Gamepad, Touch].
        /// </param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        /// <returns>The created preferred device player input.</returns>
        public static PreferredDevicePlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> AddPreferredDevicePlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(
            string _playerName,
            InputActionAsset _actionAsset,
            _AInputActionPathConfig<T_ACTION_ENUM> _actionPathMappingConfig,
            _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> _actionMapPathMappingConfig,
            List<PreferInputDeviceType> _preferredTypes = null)
            where T_ACTION_MAP_ENUM : Enum
            where T_ACTION_ENUM : Enum
        {
            return manager.AddPreferredDevicePlayer(_playerName, _actionAsset, _actionPathMappingConfig, _actionMapPathMappingConfig, _preferredTypes);
        }
        /// <summary>
        /// Removes a preferred device player input user.
        /// </summary>
        /// <param name="_player">The preferred device player to remove.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        public static void RemovePreferredDevicePlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(
            [NotNull] PreferredDevicePlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _player)
            where T_ACTION_MAP_ENUM : Enum
            where T_ACTION_ENUM : Enum
        {
            manager.RemovePreferredDevicePlayer(_player);
        }
        /// <summary>
        /// Adds a manual player input user that requires manual device management.
        /// </summary>
        /// <remarks>
        /// Use the returned ManualPlayerInput's AddDevice/RemoveDevice methods to manage devices.
        /// </remarks>
        /// <param name="_playerName">The name of the player.</param>
        /// <param name="_actionAsset">The InputActionAsset for the player.</param>
        /// <param name="_actionPathMappingConfig">The action path mapping configuration.</param>
        /// <param name="_actionMapPathMappingConfig">The action map path mapping configuration.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        /// <returns>The created manual player input.</returns>
        public static ManualPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> AddManualPlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(
            string _playerName,
            InputActionAsset _actionAsset,
            _AInputActionPathConfig<T_ACTION_ENUM> _actionPathMappingConfig,
            _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> _actionMapPathMappingConfig)
            where T_ACTION_MAP_ENUM : Enum
            where T_ACTION_ENUM : Enum
        {
            return manager.AddManualPlayer(_playerName, _actionAsset, _actionPathMappingConfig, _actionMapPathMappingConfig);
        }
        /// <summary>
        /// Removes a manual player input user.
        /// </summary>
        /// <param name="_player">The manual player to remove.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        public static void RemoveManualPlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(ManualPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _player)
            where T_ACTION_MAP_ENUM : Enum
            where T_ACTION_ENUM : Enum
        {
            manager.RemoveManualPlayer(_player);
        }
    }
}