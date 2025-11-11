// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using RebindingOperation = UnityEngine.InputSystem.InputActionRebindingExtensions.RebindingOperation;

namespace CodaGame
{
    /// <summary>
    /// Input management class for a single player
    /// </summary>
    /// <remarks>
    /// Note: This class is not InputSystem's PlayerInput component, but they have similar functionality. You can find all methods related to player input here.
    /// </remarks>
    public partial class PlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>
        where T_ACTION_MAP_ENUM : Enum
        where T_ACTION_ENUM : Enum
    {
        // Name prefix for this class
        private const string _k_prefixKey = "PlayerInput_";
        // How long actions will be buffered
        private const float _k_actionBufferTime = 2f;


        // Current player's input action asset
        [NotNull] private readonly InputActionAsset _m_actionAsset;
        // Current player's index
        private readonly int _m_playerIndex;
        // Devices used by this player
        [ItemNotNull, NotNull] private readonly List<InputDevice> _m_devices;
        // Dictionary mapping enum values to input action maps
        [NotNull] private readonly Dictionary<T_ACTION_MAP_ENUM, InputActionMapInternal> _m_enum2ActionMapDict;
        // Dictionary mapping enum values to input actions
        [NotNull] private readonly Dictionary<T_ACTION_ENUM, InputActionInternal> _m_enum2ActionDict;
        // Rebinding operation control handle
        private RebindingHandle _m_rebindingHandle;
        // Whether this class is still enabled
        private bool _m_isEnable;
        // Current control scheme
        private ControlSchemeType _m_currentScheme;
        
        
        /// <summary>
        /// Construct a player input manager
        /// </summary>
        /// <param name="_actionAsset">Action asset resource</param>
        /// <param name="_playerIndex">Player index number</param>
        /// <param name="_devices">Devices used by the player</param>
        /// <param name="_actionPathMapping">Mapping from action enum to action path</param>
        /// <param name="_actionMapPathMapping">Mapping from action map enum to action map path</param>
        internal PlayerInput([NotNull] InputActionAsset _actionAsset, int _playerIndex, [NotNull] List<InputDevice> _devices, 
            [NotNull] Dictionary<T_ACTION_ENUM, string> _actionPathMapping,
            [NotNull] Dictionary<T_ACTION_MAP_ENUM, string> _actionMapPathMapping)
        {
            _m_enum2ActionDict = new Dictionary<T_ACTION_ENUM, InputActionInternal>();
            _m_enum2ActionMapDict = new Dictionary<T_ACTION_MAP_ENUM, InputActionMapInternal>();
            
            _m_playerIndex = _playerIndex;
            _m_actionAsset = _actionAsset;
            _m_isEnable = true;
            _m_currentScheme = ControlSchemeType.Unknown;

            _m_devices = new List<InputDevice>(_devices.Count);
            foreach (InputDevice device in _devices)
            {
                if (device == null)
                {
                    Console.LogWarning(SystemNames.Input, name, "One of the devices is null, it will be ignored.");
                    continue;
                }
                _m_devices.Add(device);
            }
            
            foreach (KeyValuePair<T_ACTION_ENUM, string> pair in _actionPathMapping)
            {
                if (pair.Value == null)
                {
                    Console.LogWarning(SystemNames.Input, name, $"The action path for enum {pair.Key} is null, it will be ignored.");
                    continue;
                }

                InputAction action = _m_actionAsset.FindAction(pair.Value);
                if (action == null)
                {
                    Console.LogWarning(SystemNames.Input, name, $"The action for enum {pair.Key} with path {pair.Value} is not found, it will be ignored.");
                    continue;
                }

                if (_m_enum2ActionDict.ContainsKey(pair.Key))
                {
                    Console.LogWarning(SystemNames.Input, name, $"The action for enum {pair.Key} with path {pair.Value} is already mapped, it will be ignored.");
                    continue;
                }

                _m_enum2ActionDict.Add(pair.Key, new InputActionInternal(this, action));
            }
            
            foreach (KeyValuePair<T_ACTION_MAP_ENUM, string> pair in _actionMapPathMapping)
            {
                if (pair.Value == null)
                {
                    Console.LogWarning(SystemNames.Input, name, $"The action map path for enum {pair.Key} is null, it will be ignored.");
                    continue;
                }

                InputActionMap actionMap = _m_actionAsset.FindActionMap(pair.Value);
                if (actionMap == null)
                {
                    Console.LogWarning(SystemNames.Input, name, $"The action map for enum {pair.Key} with path {pair.Value} is not found, it will be ignored.");
                    continue;
                }

                if (_m_enum2ActionMapDict.ContainsKey(pair.Key))
                {
                    Console.LogWarning(SystemNames.Input, name, $"The action map for enum {pair.Key} with path {pair.Value} is already mapped, it will be ignored.");
                    continue;
                }

                _m_enum2ActionMapDict.Add(pair.Key, new InputActionMapInternal(actionMap));
            }
            
            Initialize();
        }


        /// <summary>
        /// Triggered when a new device is added
        /// </summary>
        public event NotNullAction<InputDevice> onDeviceAdded;
        /// <summary>
        /// Triggered when a device disconnects
        /// </summary>
        public event NotNullAction<InputDevice> onDeviceLost;
        /// <summary>
        /// Triggered when the player loses all devices
        /// </summary>
        public event Action onLostAllDevices;
        /// <summary>
        /// Triggered when the player's control scheme changes
        /// </summary>
        /// <remarks>
        /// For example, when switching between keyboard/mouse and gamepad
        /// </remarks>
        public event Action<ControlSchemeType> onSchemeChanged;
        
        /// <summary>
        /// Name of this player input
        /// </summary>
        public string name { get { return _k_prefixKey + _m_playerIndex; } }
        /// <summary>
        /// Whether this class is still enabled
        /// </summary>
        /// <remarks>
        /// Once disabled, this class can no longer be used
        /// </remarks>
        public bool isEnabled { get { return _m_isEnable; } }
        /// <summary>
        /// Currently active control scheme
        /// </summary>
        public ControlSchemeType currentScheme { get { LogIfInvalid(); return _m_currentScheme; } }
        /// <summary>
        /// Currently active input devices for this player
        /// </summary>
        /// <remarks>
        /// Automatically removed when devices disconnect, until they are added again
        /// </remarks>
        public IReadOnlyList<InputDevice> devices { get { LogIfInvalid(); return _m_devices; } }
        
        
        /// <summary>
        /// Add a new input device
        /// </summary>
        public void AddDevice(InputDevice _device)
        {
            if (LogIfInvalid())
                return;
            
            if (_device == null)
            {
                Console.LogWarning(SystemNames.Input, name, "Add device failed, device is null.");
                return;
            }
            if (_m_devices.Contains(_device))
            {
                Console.LogWarning(SystemNames.Input, name, $"Add device failed, device {_device.displayName} is already added.");
                return;
            }

            _m_devices.Add(_device);
            _m_actionAsset.devices = _m_devices.ToArray();
            onDeviceAdded?.Invoke(_device);
        }
        /// <summary>
        /// Remove an input device
        /// </summary>
        public void RemoveDevice(InputDevice _device)
        {
            if (LogIfInvalid())
                return;
            
            if (_device == null)
            {
                Console.LogWarning(SystemNames.Input, name, "Remove device failed, device is null.");
                return;
            }
            if (!_m_devices.Contains(_device))
            {
                Console.LogWarning(SystemNames.Input, name, $"Remove device failed, device {_device.displayName} is not found.");
                return;
            }

            _m_devices.Remove(_device);
            _m_actionAsset.devices = _m_devices.ToArray();
            onDeviceLost?.Invoke(_device);
            if (_m_devices.Count == 0)
                onLostAllDevices?.Invoke();
        }
        /// <summary>
        /// Enable an action map
        /// </summary>
        /// <remarks>
        /// By default, action maps are enabled with a count of 1. Calling this method increments the enable count. The action map is only active when the count is greater than 0.
        /// </remarks>
        public void EnableActionMap(T_ACTION_MAP_ENUM _actionMap)
        {
            if (LogIfInvalid())
                return;
            
            InputActionMapInternal actionMap = _m_enum2ActionMapDict.GetValueOrDefault(_actionMap);
            if (actionMap == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Enable action map failed, action map {_actionMap} not found.");
                return;
            }

            actionMap.Enable();
        }
        /// <summary>
        /// Disable an action map
        /// </summary>
        /// <remarks>
        /// By default, action maps are enabled with a count of 1. Calling this method decrements the enable count. The action map is disabled when the count is less than 1.
        /// </remarks>
        public void DisableActionMap(T_ACTION_MAP_ENUM _actionMap)
        {
            if (LogIfInvalid())
                return;
            
            InputActionMapInternal actionMap = _m_enum2ActionMapDict.GetValueOrDefault(_actionMap);
            if (actionMap == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Disable action map failed, action map {_actionMap} not found.");
                return;
            }

            actionMap.Disable();
        }
        /// <summary>
        /// Enable an action
        /// </summary>
        /// <remarks>
        /// By default, actions are enabled with a count of 1. Calling this method increments the enable count. The action is only active when the count is greater than 0.
        /// </remarks>
        public void EnableAction(T_ACTION_ENUM _action)
        {
            if (LogIfInvalid())
                return;
            
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Enable action failed, action {_action} not found.");
                return;
            }

            action.Enable();
        }
        /// <summary>
        /// Disable an action
        /// </summary>
        /// <remarks>
        /// By default, actions are enabled with a count of 1. Calling this method decrements the enable count. The action is disabled when the count is less than 1.
        /// </remarks>
        public void DisableAction(T_ACTION_ENUM _action)
        {
            if (LogIfInvalid())
                return;
            
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Disable action failed, action {_action} not found.");
                return;
            }

            action.Disable();
        }
        /// <summary>
        /// Register an action event callback
        /// </summary>
        /// <param name="_action">Action type</param>
        /// <param name="_callback">Callback function</param>
        /// <param name="_callbackType">Callback trigger type</param>
        public void RegisterActionCallback(T_ACTION_ENUM _action, Action<InputAction.CallbackContext> _callback, InputCallbackType _callbackType = InputCallbackType.Performed)
        {
            if (LogIfInvalid())
                return;
            
            if (_callback == null)
            {
                Console.LogWarning(SystemNames.Input, name, "Action callback register failed, callback is null.");
                return;
            }
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Action callback register failed, action {_action} not found.");
                return;
            }

            switch (_callbackType)
            {
                case InputCallbackType.Started:
                    action.started += _callback;
                    break;
                case InputCallbackType.Performed:
                    action.performed += _callback;
                    break;
                case InputCallbackType.Canceled:
                    action.canceled += _callback;
                    break;
            }
        }
        /// <summary>
        /// Register an action event callback
        /// </summary>
        /// <param name="_action">Action type</param>
        /// <param name="_callback">Callback function</param>
        /// <param name="_callbackType">Callback trigger type</param>
        /// <typeparam name="T_VALUE">Return value type of the action</typeparam>
        public void RegisterActionCallback<T_VALUE>(T_ACTION_ENUM _action, Action<T_VALUE> _callback, InputCallbackType _callbackType = InputCallbackType.Performed)
            where T_VALUE : struct
        {
            if (LogIfInvalid())
                return;
            
            if (_callback == null)
            {
                Console.LogWarning(SystemNames.Input, name, "Action callback register failed, callback is null.");
                return;
            }
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Action callback register failed, action {_action} not found.");
                return;
            }
            
            action.AddCallback<T_VALUE>(_callbackType, _callback);
        }
        /// <summary>
        /// Register an action event callback
        /// </summary>
        /// <param name="_action">Action type</param>
        /// <param name="_callback">Callback function</param>
        /// <param name="_callbackType">Callback trigger type</param>
        public void RegisterActionCallbacks(T_ACTION_ENUM _action, Action _callback, InputCallbackType _callbackType = InputCallbackType.Performed)
        {
            if (LogIfInvalid())
                return;
            
            if (_callback == null)
            {
                Console.LogWarning(SystemNames.Input, name, "Action callback register failed, callback is null.");
                return;
            }
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Action callback register failed, action {_action} not found.");
                return;
            }
            
            action.AddCallback(_callbackType, _callback);
        }
        /// <summary>
        /// Unregister an action event callback
        /// </summary>
        /// <param name="_action">Action type</param>
        /// <param name="_callback">Callback function</param>
        /// <param name="_callbackType">Trigger type set during registration</param>
        public void UnregisterActionCallback(T_ACTION_ENUM _action, Action<InputAction.CallbackContext> _callback, InputCallbackType _callbackType = InputCallbackType.Performed)
        {
            if (LogIfInvalid())
                return;
            
            if (_callback == null)
            {
                Console.LogWarning(SystemNames.Input, name, "Action callback unregister failed, callback is null.");
                return;
            }
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Action callback unregister failed, action {_action} not found.");
                return;
            }

            switch (_callbackType)
            {
                case InputCallbackType.Started:
                    action.started -= _callback;
                    break;
                case InputCallbackType.Performed:
                    action.performed -= _callback;
                    break;
                case InputCallbackType.Canceled:
                    action.canceled -= _callback;
                    break;
            }
        }
        /// <summary>
        /// Unregister an action event callback
        /// </summary>
        /// <param name="_action">Action type</param>
        /// <param name="_callback">Callback function</param>
        /// <param name="_callbackType">Trigger type set during registration</param>
        /// <typeparam name="T_VALUE">Return value type of the action</typeparam>
        public void UnregisterActionCallback<T_VALUE>(T_ACTION_ENUM _action, Action<T_VALUE> _callback, InputCallbackType _callbackType = InputCallbackType.Performed)
            where T_VALUE : struct
        {
            if (LogIfInvalid())
                return;
            
            if (_callback == null)
            {
                Console.LogWarning(SystemNames.Input, name, "Action callback unregister failed, callback is null.");
                return;
            }
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Action callback unregister failed, action {_action} not found.");
                return;
            }

            action.RemoveCallback<T_VALUE>(_callbackType, _callback);
        }
        /// <summary>
        /// Unregister an action event callback
        /// </summary>
        /// <param name="_action">Action type</param>
        /// <param name="_callback">Callback function</param>
        /// <param name="_callbackType">Trigger type set during registration</param>
        public void UnregisterActionCallbacks(T_ACTION_ENUM _action, Action _callback, InputCallbackType _callbackType = InputCallbackType.Performed)
        {
            if (LogIfInvalid())
                return;

            if (_callback == null)
            {
                Console.LogWarning(SystemNames.Input, name, "Action callback unregister failed, callback is null.");
                return;
            }
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"Action callback unregister failed, action {_action} not found.");
                return;
            }
            
            action.RemoveCallback(_callbackType, _callback);
        }
        /// <summary>
        /// Check if the action was started in the specified frame
        /// </summary>
        public bool WasActionStarted(T_ACTION_ENUM _action, int _logicFrame)
        {
            if (LogIfInvalid())
                return false;
            
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"WasActionStarted check failed, action {_action} not found.");
                return false;
            }
            
            return action.WasActionStarted(_logicFrame);
        }
        /// <summary>
        /// Check if the action was performed in the specified frame
        /// </summary>
        public bool WasActionPerformed(T_ACTION_ENUM _action, int _logicFrame)
        {
            if (LogIfInvalid())
                return false;
            
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"WasActionPerformed check failed, action {_action} not found.");
                return false;
            }
            
            return action.WasActionPerformed(_logicFrame);
        }
        /// <summary>
        /// Check if the action was canceled in the specified frame
        /// </summary>
        public bool WasActionCanceled(T_ACTION_ENUM _action, int _logicFrame)
        {
            if (LogIfInvalid())
                return false;
            
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"WasActionCanceled check failed, action {_action} not found.");
                return false;
            }
            
            return action.WasActionCanceled(_logicFrame);
        }
        /// <summary>
        /// Read the action value at the specified frame
        /// </summary>
        public T_VALUE ReadActionValue<T_VALUE>(T_ACTION_ENUM _action, int _logicFrame)
            where T_VALUE : struct
        {
            if (LogIfInvalid())
                return default;
            
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"ReadActionValue failed, action {_action} not found.");
                return default;
            }
            
            return action.ReadValue<T_VALUE>(_logicFrame);
        }
        /// <summary>
        /// Start rebinding an action's key binding
        /// </summary>
        /// <param name="_action">Action type</param>
        /// <param name="_bindingIndex">Binding index</param>
        /// <param name="_timeOut">Timeout duration</param>
        public RebindingHandle StartRebinding(T_ACTION_ENUM _action, int _bindingIndex, float _timeOut = 10f)
        {
            if (LogIfInvalid())
                return null;
            
            if (_m_rebindingHandle != null)
            {
                Console.LogWarning(SystemNames.Input, name, "StartRebinding failed, there is already a rebinding operation in progress.");
                return null;
            }
            
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"StartRebinding failed, action {_action} not found.");
                return null;
            }

            RebindingOperation operation = action.StartRebinding(_bindingIndex);
            if (operation == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"StartRebinding failed, binding index {_bindingIndex} not found for action {_action}.");
                return null;
            }
            
            _m_rebindingHandle = new RebindingHandle(operation, _m_devices, _timeOut, RebindingComplete);
            return _m_rebindingHandle;
        }
        /// <summary>
        /// Get the button type bound to the current action
        /// </summary>
        public InputButtonType GetBindingButtonType(T_ACTION_ENUM _action, int _bindingIndex)
        {
            if (LogIfInvalid())
                return InputButtonType.Unknown;
            
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"GetBindingButtonType failed, action {_action} not found.");
                return InputButtonType.Unknown;
            }

            InputControl control = action.GetBindingControl(_bindingIndex);
            if (control == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"GetBindingButtonType failed, binding index {_bindingIndex} not found for action {_action}.");
                return InputButtonType.Unknown;
            }

            return control.ToButtonType();
        }
        /// <summary>
        /// Get the input control bound to the current action
        /// </summary>
        public InputControl GetBindingControl(T_ACTION_ENUM _action, int _bindingIndex)
        {
            if (LogIfInvalid())
                return null;
            
            InputActionInternal action = _m_enum2ActionDict.GetValueOrDefault(_action);
            if (action == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"GetBindingName failed, action {_action} not found.");
                return null;
            }

            InputControl control = action.GetBindingControl(_bindingIndex);
            if (control == null)
            {
                Console.LogWarning(SystemNames.Input, name, $"GetBindingName failed, binding index {_bindingIndex} not found for action {_action}.");
                return null;
            }

            return control;
        }


        /// <summary>
        /// Dispose function called by PlayerInputManager
        /// </summary>
        /// <remarks>
        /// <para>Once disposed, this class can no longer be used.</para>
        /// </remarks>
        internal void Dispose()
        {
            foreach (InputActionInternal action in _m_enum2ActionDict.Values)
                action.Dispose();
            _m_enum2ActionDict.Clear();
            foreach (InputActionMapInternal actionMap in _m_enum2ActionMapDict.Values)
                actionMap.Dispose();
            _m_enum2ActionMapDict.Clear();
            
            _m_isEnable = false;
            InputSystem.onDeviceChange -= OnDeviceChange;
        }
        
        
        // Initialization, load saved bindings and set up device mask
        private void Initialize()
        {
            // Load saved key bindings
            string bindingString = PlayerPrefs.GetString(name + "_overrideBinding");
            if (!string.IsNullOrEmpty(bindingString))
                _m_actionAsset.LoadBindingOverridesFromJson(bindingString);
            // Set devices used by this player
            _m_actionAsset.devices = _m_devices.ToArray();
            // Listen for device disconnect events
            InputSystem.onDeviceChange += OnDeviceChange;
        }
        // Log warning if the class is already disposed
        private bool LogIfInvalid()
        {
            if (!_m_isEnable)
            {
                Console.LogWarning(SystemNames.Input, name, "The PlayerInput is already disposed. You can't use it anymore.");
                return true;
            }
            
            return false;
        }
        // Change the current control scheme
        private void ChangeControlScheme(ControlSchemeType _controlSchemeType)
        {
            if (!_m_isEnable)
                return;

            if (_controlSchemeType != _m_currentScheme)
            {
                _m_currentScheme = _controlSchemeType;
                onSchemeChanged?.Invoke(_m_currentScheme);
            }
        }
        // Handle device change events, remove disconnected devices
        private void OnDeviceChange(InputDevice _device, InputDeviceChange _change)
        {
            if (!_m_isEnable || _device == null)
                return;

            if (_change is InputDeviceChange.Disconnected or InputDeviceChange.Removed)
            {
                if (_m_devices.Contains(_device))
                    RemoveDevice(_device);
            }
        }
        // Callback when rebinding is complete
        private void RebindingComplete(bool _success)
        {
            if (_success)
            {
                string bindingString = _m_actionAsset.SaveBindingOverridesAsJson();
                PlayerPrefs.SetString(name + "_overrideBinding", bindingString);
            }
            
            _m_rebindingHandle = null;
        }
    }
}