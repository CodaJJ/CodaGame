// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine.InputSystem;

namespace CodaGame
{
    public enum InputButtonType
    {
        Unknown,
    
        // Keyboard
        Escape, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
        Backquote, Digit1, Digit2, Digit3, Digit4, Digit5, Digit6, Digit7, Digit8, Digit9, Digit0, Minus, Equals, Backspace,
        Tab, Q, W, E, R, T, Y, U, I, O, P, LeftBracket, RightBracket, Backslash,
        CapsLock, A, S, D, F, G, H, J, K, L, Semicolon, Quote, Enter,
        LeftShift, Z, X, C, V, B, N, M, Comma, Period, Slash, RightShift,
        LeftCtrl, LeftMeta, LeftAlt, Space, RightAlt, RightMeta, ContextMenu, RightCtrl,
    
        Insert, Home, PageUp,
        Delete, End, PageDown,
    
        UpArrow, DownArrow, LeftArrow, RightArrow,
    
        NumLock, NumpadDivide, NumpadMultiply, NumpadMinus,
        Numpad7, Numpad8, Numpad9, NumpadPlus,
        Numpad4, Numpad5, Numpad6,
        Numpad1, Numpad2, Numpad3, NumpadEnter,
        Numpad0, NumpadPeriod,
    
        // Mouse
        LeftButton, RightButton, MiddleButton,
        ForwardButton, BackButton,
    
        // Gamepad
        ButtonSouth, ButtonEast, ButtonWest, ButtonNorth,
    
        LeftShoulder, RightShoulder,
        LeftTrigger, RightTrigger,
    
        LeftStickPress, RightStickPress,
    
        DpadUp, DpadDown, DpadLeft, DpadRight,
    
        // Gamepad
        Select,          // Back(Xbox) / Share(PS) / Minus(Switch)
        Start,           // Start(Xbox) / Options(PS) / Plus(Switch)
        TouchpadButton,  // PS specific
    }
    public static class InputButtonTypeExtensions
    {
        public static InputButtonType ToButtonType(this InputControl _control)
        {
            if (_control == null)
                return InputButtonType.Unknown;
            
            string name = _control.name;
            string parentName = _control.parent?.name;
            if (parentName == "dpad")
                name = parentName + name;
        
            if (Enum.TryParse(name, ignoreCase: true, out InputButtonType type))
                return type;
        
            return InputButtonType.Unknown;
        }
    }
}