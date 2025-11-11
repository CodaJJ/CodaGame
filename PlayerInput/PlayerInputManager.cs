// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.InputSystem;

namespace CodaGame
{
    /// <summary>
    /// Player input manager
    /// </summary>
    /// <remarks>
    /// Has the same name as InputSystem's PlayerInputManager, but different functionality
    /// </remarks>
    public sealed class PlayerInputManager
    {
        [NotNull] public static PlayerInputManager instance { get { return _g_instance ??= new PlayerInputManager(); } }
        private static PlayerInputManager _g_instance;


        [NotNull] private Dictionary<int, PlayerInput> _m_playerInputs;
        

        private PlayerInputManager()
        {
            _m_playerInputs = new Dictionary<int, PlayerInput>();
        }


        public void AddPlayer()
        {
            
        }
        public void RemovePlayer()
        {
            
        }
    }
}