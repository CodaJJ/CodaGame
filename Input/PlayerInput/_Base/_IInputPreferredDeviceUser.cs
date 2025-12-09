// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame.Base
{
    internal interface _IInputPreferredDeviceUser : _IInputDeviceUser
    {
        public ReadOnlyList<PreferInputDeviceType> preferredTypes { get; }
    }
}