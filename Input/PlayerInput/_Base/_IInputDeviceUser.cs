// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine.InputSystem;

namespace CodaGame.Base
{
    internal interface _IInputDeviceUser
    {
        public bool AddDevice(InputDevice _device);
        public bool RemoveDevice(InputDevice _device);
        public ReadOnlyList<InputDevice> devices { get; }
        public PlayerInputDeviceManagementType deviceManagementType { get; }
    }
}