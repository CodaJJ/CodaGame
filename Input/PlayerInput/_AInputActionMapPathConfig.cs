// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// Configuration item for mapping a player input action map to its path.
    /// </summary>
    [Serializable]
    public class InputActionMapPathConfigItem<T_ACTION_MAP_ENUM>
    {
        /// <summary>
        /// The action map enum value.
        /// </summary>
        public T_ACTION_MAP_ENUM actionMapEnum;
        /// <summary>
        /// The path of the action map.
        /// </summary>
        public string actionMapPath;
    }
    /// <summary>
    /// Configuration for mapping player input action maps to their paths.
    /// </summary>
    public abstract class _AInputActionMapPathConfig<T_ACTION_MAP_ENUM> : _ATableConfig<InputActionMapPathConfigItem<T_ACTION_MAP_ENUM>>
    {
    }
}