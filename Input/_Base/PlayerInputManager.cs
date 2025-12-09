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
    /// <summary>
    /// Player input manager
    /// </summary>
    /// <remarks>
    /// <para>Has the same name as InputSystem's PlayerInputManager, but different functionality.</para>
    /// <para>It manages three types of input device users:</para>
    /// <list type="bullet">
    ///     <item>
    ///         <term>All device users: </term>
    ///         <description>Users that want to use all input devices.</description>
    ///     </item>
    ///     <item>
    ///         <term>Preferred device users: </term>
    ///         <description>
    ///         Users that want to use specific types of input devices (e.g. gamepad, keyboard).
    ///         When a device of the preferred type is available (not occupied by other preferred device users), it will be assigned to the user.
    ///         If no preferred device is available, try to assign other available devices.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>Manual device users: </term>
    ///         <description>Users that manually specify which input devices to use.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    internal class PlayerInputManager
    {
        [NotNull] public static PlayerInputManager instance { get { return _g_instance ??= new PlayerInputManager(); } }
        private static PlayerInputManager _g_instance;

        
        // All connected devices
        [ItemNotNull, NotNull] private readonly List<InputDevice> _m_allDevices;
        // Key: InputDevice, Value: Users occupying this device
        [NotNull] private readonly Dictionary<InputDevice, HashSet<_IInputDeviceUser>> _m_deviceOccupancyForAllUserDic;
        [NotNull] private readonly Dictionary<InputDevice, _IInputPreferredDeviceUser> _m_deviceOccupancyForPreferredUsersDic;
        // Users occupying any device
        [ItemNotNull, NotNull] private readonly List<_IInputDeviceUser> _m_allDeviceUsers;
        // Users preferring specific device types with priority order
        // Devices are automatically allocated based on user's priority list
        [ItemNotNull, NotNull] private readonly List<_IInputPreferredDeviceUser> _m_preferredUsers;
        // Users who manually specify devices
        [ItemNotNull, NotNull] private readonly List<_IInputDeviceUser> _m_manualUsers;
        

        private PlayerInputManager()
        {
            _m_allDevices = new List<InputDevice>(InputSystem.devices);
            _m_deviceOccupancyForAllUserDic = new Dictionary<InputDevice, HashSet<_IInputDeviceUser>>();
            _m_deviceOccupancyForPreferredUsersDic = new Dictionary<InputDevice, _IInputPreferredDeviceUser>();
            _m_allDeviceUsers = new List<_IInputDeviceUser>();
            _m_preferredUsers = new List<_IInputPreferredDeviceUser>();
            _m_manualUsers = new List<_IInputDeviceUser>();

            foreach (InputDevice device in _m_allDevices)
            {
                if (device == null)
                    continue;

                _m_deviceOccupancyForAllUserDic[device] = new HashSet<_IInputDeviceUser>();
            }

            InputSystem.onDeviceChange += OnDeviceChange;
        }
        ~PlayerInputManager()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }
        
        
        /// <summary>
        /// Event fired when a device is connected.
        /// </summary>
        public event Action<InputDevice> onDeviceConnected;
        /// <summary>
        /// Event fired when a device is disconnected.
        /// </summary>
        public event Action<InputDevice> onDeviceDisconnected;
        
        /// <summary>
        /// All connected input devices.
        /// </summary>
        [ItemNotNull] public ReadOnlyList<InputDevice> allDevices { get { return _m_allDevices; } }


        /// <summary>
        /// Get the number of input users using the specified device.
        /// </summary>
        /// <param name="_device">The input device.</param>
        /// <param name="_deviceUserTypeMask">The device user type mask to filter by. Defaults to All.</param>
        /// <returns>The number of users using this device that match the type mask.</returns>
        public int GetDeviceUserCount(InputDevice _device, PlayerInputDeviceManagementType _deviceUserTypeMask = PlayerInputDeviceManagementType.Manual | PlayerInputDeviceManagementType.Preferred | PlayerInputDeviceManagementType.AllDevices)
        {
            if (_device == null || !_m_deviceOccupancyForAllUserDic.TryGetValue(_device, out HashSet<_IInputDeviceUser> users))
                return 0;

            int count = 0;
            foreach (_IInputDeviceUser user in users)
            {
                if ((_deviceUserTypeMask & user.deviceManagementType) != 0)
                    count++;
            }

            return count;
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
        /// <returns>The created all-devices player input, or null if creation fails.</returns>
        public AllDevicesPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> AddAllDevicesPlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(
            string _playerName, InputActionAsset _actionAsset,
            _AInputActionPathConfig<T_ACTION_ENUM> _actionPathMappingConfig, 
            _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> _actionMapPathMappingConfig)
            where T_ACTION_ENUM : Enum
            where T_ACTION_MAP_ENUM : Enum
        {
            if (string.IsNullOrEmpty(_playerName))
            {
                Console.LogWarning(SystemNames.Input, "Player name cannot be null or empty.");
                return null;
            }
            if (_actionAsset == null)
            {
                Console.LogWarning(SystemNames.Input, "Action asset cannot be null.");
                return null;
            }
            if (_actionPathMappingConfig == null)
            {
                Console.LogWarning(SystemNames.Input, "Action path mapping config cannot be null.");
                return null;
            }
            if (_actionMapPathMappingConfig == null)
            {
                Console.LogWarning(SystemNames.Input, "Action map path mapping config cannot be null.");
                return null;
            }
            
            AllDevicesPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> playerInput = new AllDevicesPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(_playerName, _actionAsset, _actionPathMappingConfig, _actionMapPathMappingConfig);
            _m_allDeviceUsers.Add(playerInput);
            foreach (InputDevice device in _m_deviceOccupancyForAllUserDic.Keys)
                AddDeviceTo(playerInput, device);
            return playerInput;
        }
        /// <summary>
        /// Removes an all-devices player input user and cleans up all associated devices.
        /// </summary>
        /// <remarks>
        /// This will remove all devices from the player and dispose the player input.
        /// </remarks>
        /// <param name="_player">The all-devices player input user to remove.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        public void RemoveAllDevicesPlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(AllDevicesPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _player)
            where T_ACTION_ENUM : Enum
            where T_ACTION_MAP_ENUM : Enum
        {
            if (_player == null)
            {
                Console.LogWarning(SystemNames.Input, "Cannot remove null player.");
                return;
            }

            if (!_m_allDeviceUsers.Remove(_player))
            {
                Console.LogWarning(SystemNames.Input, "The player to remove is not found in all device users.");
                return;
            }

            List<InputDevice> devicesToRemove = new List<InputDevice>(_player.devices);
            foreach (InputDevice device in devicesToRemove)
                RemoveDeviceFrom(_player, device);
            
            _player.Dispose();
        }
        /// <summary>
        /// Adds a preferred device player input user with a priority list of device types.
        /// </summary>
        /// <remarks>
        /// <para>Preferred device players will try to use devices in priority order (first choice, second choice, etc.).</para>
        /// <para>When devices are connected or disconnected, the system will automatically reassign devices based on priority.</para>
        /// </remarks>
        /// <param name="_playerName">The name of the player.</param>
        /// <param name="_actionAsset">The InputActionAsset for the player.</param>
        /// <param name="_actionPathMappingConfig">The action path mapping configuration.</param>
        /// <param name="_actionMapPathMappingConfig">The action map path mapping configuration.</param>
        /// <param name="_preferredTypes">The list of preferred device types in priority order (first = highest priority). If null or empty, will use all types.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        /// <returns>The created preferred device player input, or null if creation fails.</returns>
        public PreferredDevicePlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> AddPreferredDevicePlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(
            string _playerName,
            InputActionAsset _actionAsset,
            _AInputActionPathConfig<T_ACTION_ENUM> _actionPathMappingConfig,
            _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> _actionMapPathMappingConfig,
            List<PreferInputDeviceType> _preferredTypes = null)
            where T_ACTION_ENUM : Enum
            where T_ACTION_MAP_ENUM : Enum
        {
            if (string.IsNullOrEmpty(_playerName))
            {
                Console.LogWarning(SystemNames.Input, "Player name cannot be null or empty.");
                return null;
            }
            if (_actionAsset == null)
            {
                Console.LogWarning(SystemNames.Input, "Action asset cannot be null.");
                return null;
            }
            if (_actionPathMappingConfig == null)
            {
                Console.LogWarning(SystemNames.Input, "Action path mapping config cannot be null.");
                return null;
            }
            if (_actionMapPathMappingConfig == null)
            {
                Console.LogWarning(SystemNames.Input, "Action map path mapping config cannot be null.");
                return null;
            }

            // If no preferred types specified, use all types in default order
            if (_preferredTypes == null || _preferredTypes.Count == 0)
            {
                _preferredTypes = new List<PreferInputDeviceType>();
                for (int i = 0; i < EnumArray<PreferInputDeviceType>.Length; i++)
                    _preferredTypes.Add((PreferInputDeviceType)i);
            }

            PreferredDevicePlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> playerInput = new PreferredDevicePlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(_playerName, _actionAsset, _actionPathMappingConfig, _actionMapPathMappingConfig, _preferredTypes);
            _m_preferredUsers.Add(playerInput);
            TryAssignPreferredDevices();
            return playerInput;
        }
        /// <summary>
        /// Removes a preferred device player input user and cleans up all associated devices.
        /// </summary>
        /// <remarks>
        /// This will remove all devices from the player, dispose the player input, and reassign devices to other waiting players.
        /// </remarks>
        /// <param name="_player">The preferred device player input user to remove.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        public void RemovePreferredDevicePlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(PreferredDevicePlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _player)
            where T_ACTION_ENUM : Enum
            where T_ACTION_MAP_ENUM : Enum
        {
            if (_player == null)
            {
                Console.LogWarning(SystemNames.Input, "Cannot remove null player.");
                return;
            }

            if (!_m_preferredUsers.Remove(_player))
            {
                Console.LogWarning(SystemNames.Input, "The player to remove is not found in preferred device users.");
                return;
            }

            List<InputDevice> devicesToRemove = new List<InputDevice>(_player.devices);
            foreach (InputDevice device in devicesToRemove)
            {
                RemoveDeviceFrom(_player, device);
                // Remove from preferred occupancy dictionary
                _m_deviceOccupancyForPreferredUsersDic.Remove(device);
            }

            _player.Dispose();
            TryAssignPreferredDevices();
        }
        /// <summary>
        /// Adds a manual player input user that requires manual device management.
        /// </summary>
        /// <remarks>
        /// <para>Manual player input users allow you to specify which input devices to use.</para>
        /// <para>You can add or remove devices dynamically using the ManualPlayerInput methods.</para>
        /// </remarks>
        /// <param name="_playerName">The name of the player.</param>
        /// <param name="_actionAsset">The InputActionAsset for the player.</param>
        /// <param name="_actionPathMappingConfig">The action path mapping configuration.</param>
        /// <param name="_actionMapPathMappingConfig">The action map path mapping configuration.</param>
        /// <param name="_devices">The list of input devices to assign to the player. Can be null or empty.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        /// <returns>The created manual player input, or null if creation fails.</returns>
        public ManualPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> AddManualPlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(
            string _playerName, InputActionAsset _actionAsset,
            _AInputActionPathConfig<T_ACTION_ENUM> _actionPathMappingConfig, 
            _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> _actionMapPathMappingConfig,
            List<InputDevice> _devices = null)
            where T_ACTION_ENUM : Enum
            where T_ACTION_MAP_ENUM : Enum
        {
            if (string.IsNullOrEmpty(_playerName))
            {
                Console.LogWarning(SystemNames.Input, "Player name cannot be null or empty.");
                return null;
            }
            if (_actionAsset == null)
            {
                Console.LogWarning(SystemNames.Input, "Action asset cannot be null.");
                return null;
            }
            if (_actionPathMappingConfig == null)
            {
                Console.LogWarning(SystemNames.Input, "Action path mapping config cannot be null.");
                return null;
            }
            if (_actionMapPathMappingConfig == null)
            {
                Console.LogWarning(SystemNames.Input, "Action map path mapping config cannot be null.");
                return null;
            }
            
            ManualPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> playerInput = new ManualPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(_playerName, _actionAsset, _actionPathMappingConfig, _actionMapPathMappingConfig);
            _m_manualUsers.Add(playerInput);
            if (_devices != null)
            {
                foreach (InputDevice device in _devices)
                    AddDeviceTo(playerInput, device);
            }
            return playerInput;
        }
        /// <summary>
        /// Removes a manual player input user and cleans up all associated devices.
        /// </summary>
        /// <remarks>
        /// This will remove all devices from the player and dispose the player input.
        /// </remarks>
        /// <param name="_player">The manual player input user to remove.</param>
        /// <typeparam name="T_ACTION_MAP_ENUM">The action map enum type.</typeparam>
        /// <typeparam name="T_ACTION_ENUM">The action enum type.</typeparam>
        public void RemoveManualPlayer<T_ACTION_MAP_ENUM, T_ACTION_ENUM>(ManualPlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _player)
            where T_ACTION_ENUM : Enum
            where T_ACTION_MAP_ENUM : Enum
        {
            if (_player == null)
            {
                Console.LogWarning(SystemNames.Input, "Cannot remove null player.");
                return;
            }

            if (!_m_manualUsers.Remove(_player))
            {
                Console.LogWarning(SystemNames.Input, "The player to remove is not found in manual users.");
                return;
            }

            List<InputDevice> devicesToRemove = new List<InputDevice>(_player.devices);
            foreach (InputDevice device in devicesToRemove)
                RemoveDeviceFrom(_player, device);
            
            _player.Dispose();
        }
        
        
        /// <summary>
        /// Add device to player input user
        /// </summary>
        internal bool AddDeviceTo(_IInputDeviceUser _playerInput, InputDevice _device)
        {
            if (_device == null)
            {
                Console.LogWarning(SystemNames.Input, "Cannot add null device to player.");
                return false;
            }
            if (_playerInput == null)
            {
                Console.LogWarning(SystemNames.Input, "Cannot add device to null player input user.");
                return false;
            }
            if (!_m_deviceOccupancyForAllUserDic.TryGetValue(_device, out HashSet<_IInputDeviceUser> users))
            {
                Console.LogWarning(SystemNames.Input, $"Device {_device.displayName} not recognized by PlayerInputManager.");
                return false;
            }
            if (!users.Add(_playerInput))
            {
                Console.LogWarning(SystemNames.Input, $"Device {_device.displayName} is already assigned to the player.");
                return false;
            }
            
            _playerInput.AddDevice(_device);
            return true;
        }
        /// <summary>
        /// Remove device from player input user
        /// </summary>
        internal bool RemoveDeviceFrom(_IInputDeviceUser _playerInput, InputDevice _device)
        {
            if (_device == null)
            {
                Console.LogWarning(SystemNames.Input, "Cannot remove null device from player.");
                return false;
            }
            if (_playerInput == null)
            {
                Console.LogWarning(SystemNames.Input, "Cannot remove device from null player input user.");
                return false;
            }
            if (!_m_deviceOccupancyForAllUserDic.TryGetValue(_device, out HashSet<_IInputDeviceUser> users))
            {
                Console.LogWarning(SystemNames.Input, $"Device {_device.displayName} not recognized by PlayerInputManager.");
                return false;
            }
            if (!users.Remove(_playerInput))
            {
                Console.LogWarning(SystemNames.Input, $"Device {_device.displayName} is not assigned to the player.");
                return false;
            }

            _playerInput.RemoveDevice(_device);
            return true;
        }


        // Try to assign preferred devices to all preferred users based on their priority lists
        private void TryAssignPreferredDevices()
        {
            Dictionary<InputDevice, _IInputPreferredDeviceUser> occupancy = new Dictionary<InputDevice, _IInputPreferredDeviceUser>();
            foreach (_IInputPreferredDeviceUser user in _m_preferredUsers)
            {
                // Try to find a better device for this user, for keyboard/mouse type it will return two devices (one keyboard and one mouse)
                InputDevice[] bestDevice = FindBestAvailableDeviceForUser(user, occupancy);
                
                // Check who is occupying the best device(s), free them up
                if (bestDevice != null)
                {
                    foreach (InputDevice device in bestDevice)
                    {
                        if (_m_deviceOccupancyForPreferredUsersDic.TryGetValue(device, out _IInputPreferredDeviceUser userOccupying))
                        {
                            // If device is already occupied by this user, skip
                            if (userOccupying == user)
                                continue;

                            RemoveDeviceFrom(userOccupying, device);
                        }

                        // Assign device to current user
                        AddDeviceTo(user, device);
                        _m_deviceOccupancyForPreferredUsersDic[device] = user;
                    }
                }

                // Clean up other devices assigned to this user that are not in bestDevice
                List<InputDevice> devicesToRemove = new List<InputDevice>(user.devices);
                if (bestDevice != null)
                {
                    foreach (InputDevice device in bestDevice)
                        devicesToRemove.Remove(device);
                }
                foreach (InputDevice device in devicesToRemove)
                {
                    RemoveDeviceFrom(user, device);
                    _m_deviceOccupancyForPreferredUsersDic.Remove(device);
                }
            }
        }
        // Find the best available device for a preferred user based on their priority list
        [ItemNotNull]
        private InputDevice[] FindBestAvailableDeviceForUser(_IInputPreferredDeviceUser _user, [NotNull] Dictionary<InputDevice, _IInputPreferredDeviceUser> _currentOccupancy)
        {
            // Iterate through user's preferred types in priority order
            foreach (PreferInputDeviceType preferredType in _user.preferredTypes)
            {
                // Find an available device of this type
                foreach (InputDevice device in _m_allDevices)
                {
                    if (_currentOccupancy.ContainsKey(device))
                        continue;
                    
                    // Special handling for KeyboardMouse type (assign both keyboard and mouse)
                    if (preferredType == PreferInputDeviceType.KeyboardMouse)
                    {
                        bool isKeyboard;
                        if (device is Keyboard)
                            isKeyboard = true;
                        else if (device is Mouse)
                            isKeyboard = false;
                        else
                            continue;

                        // Try to find the other device (mouse if current is keyboard, keyboard if current is mouse)
                        foreach (InputDevice otherDevice in _m_allDevices)
                        {
                            if (otherDevice == device)
                                continue;
                            if (_currentOccupancy.ContainsKey(otherDevice))
                                continue;

                            if (isKeyboard && otherDevice is Mouse ||
                                !isKeyboard && otherDevice is Keyboard)
                            {
                                // Mark both devices as occupied
                                _currentOccupancy.Add(device, _user);
                                _currentOccupancy.Add(otherDevice, _user);
                                // Found an available keyboard and mouse pair!
                                return new[] { device, otherDevice };
                            }
                        }
                    }
                    else
                    {
                        // Check if device type matches
                        if (GetDeviceType(device) != preferredType)
                            continue;

                        // Mark device as occupied
                        _currentOccupancy.Add(device, _user);
                        // Found an available device of the preferred type!
                        return new[] { device };   
                    }
                }
            }

            // No device found matching any preferred type
            return null;
        }
        // Get the device type from an InputDevice
        private PreferInputDeviceType GetDeviceType(InputDevice _device)
        {
            // Check device type
            if (_device is Gamepad)
                return PreferInputDeviceType.Gamepad;
            if (_device is Keyboard || _device is Mouse)
                return PreferInputDeviceType.KeyboardMouse;
            if (_device is Touchscreen)
                return PreferInputDeviceType.Touch;

            return PreferInputDeviceType.Other;
        }
        // Handle device connection and disconnection events
        private void OnDeviceChange(InputDevice _device, InputDeviceChange _change)
        {
            if (_device == null)
                return;

            switch (_change)
            {
                case InputDeviceChange.Added:
                    // Add to all devices list
                    _m_allDevices.Add(_device);
                    // Initialize device entry
                    _m_deviceOccupancyForAllUserDic.Add(_device, new HashSet<_IInputDeviceUser>());
                    // Add device to all device users
                    foreach (_IInputDeviceUser user in _m_allDeviceUsers)
                        AddDeviceTo(user, _device);
                    // Try to assign the new device to preferred users
                    TryAssignPreferredDevices();
                    // Fire event
                    onDeviceConnected?.Invoke(_device);
                    break;
                case InputDeviceChange.Removed:
                    // Remove from all devices list
                    _m_allDevices.Remove(_device);
                    // Remove from preferred occupancy dictionary
                    _m_deviceOccupancyForPreferredUsersDic.Remove(_device);
                    // Remove device from all users
                    if (!_m_deviceOccupancyForAllUserDic.Remove(_device, out HashSet<_IInputDeviceUser> users))
                        break;

                    foreach (_IInputDeviceUser user in users)
                        user.RemoveDevice(_device);
                    // Try to reassign devices to preferred users who lost their device
                    TryAssignPreferredDevices();
                    // Fire event
                    onDeviceDisconnected?.Invoke(_device);
                    break;
            }
        }
    }
}