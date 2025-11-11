// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine.InputSystem;

namespace CodaGame
{
    public class MainPlayerInput : PlayerInput
    {
        public static MainPlayerInput instance { get { return _g_instance; } }
        private static MainPlayerInput _g_instance;


        internal MainPlayerInput(InputActionAsset _asset)
        {
            _g_instance = this;
        }
    }
}