// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

namespace CodaGame
{
    public enum ControlSchemeType
    {
        KeyboardAndMouse,
        PlayStation,
        Xbox,
        SwitchPro,
        MobileTouch,
        Unknown
    }
    public static class ControlSchemeTypeExtensions
    {
        public static ControlSchemeType ToControlSchemeType(this InputDevice _device)
        {
            if (_device is Keyboard or Mouse)
                return ControlSchemeType.KeyboardAndMouse;

            if (_device is Gamepad)
            {
                if (_device is DualShockGamepad)
                    return ControlSchemeType.PlayStation;
                if (_device is XInputController)
                    return ControlSchemeType.Xbox;
                if (_device is SwitchProControllerHID)
                    return ControlSchemeType.SwitchPro;
                
                return ControlSchemeType.Xbox;
            }

            if (_device is Touchscreen)
                return ControlSchemeType.MobileTouch;
            
            return ControlSchemeType.Unknown;
        }
    }
}