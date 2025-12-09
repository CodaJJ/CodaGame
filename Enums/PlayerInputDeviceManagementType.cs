// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// How to manage player input devices.
    /// </summary>
    [Flags]
    public enum PlayerInputDeviceManagementType
    {
        /// <summary>
        /// No device management type.
        /// </summary>
        None = 0,
        /// <summary>
        /// Only manually added devices are used.
        /// </summary>
        Manual = 1 << 0,
        /// <summary>
        /// All connected input devices are used.
        /// </summary>
        AllDevices = 1 << 1,
        /// <summary>
        /// Only the preferred device is used.
        /// </summary>
        Preferred = 1 << 2,
    }
}