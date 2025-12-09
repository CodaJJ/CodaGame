// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// Configuration item that maps a player input action enum to its corresponding action path.
    /// </summary>
    [Serializable]
    public class InputActionPathConfigItem<T_ACTION_ENUM> 
        where T_ACTION_ENUM : Enum
    {
        /// <summary>
        /// The action enum value.
        /// </summary>
        public T_ACTION_ENUM actionEnum;
        /// <summary>
        /// The path of the action.
        /// </summary>
        public string actionPath;
    }
    /// <summary>
    /// Configuration for mapping player input actions to their paths.
    /// </summary>
    public abstract class _AInputActionPathConfig<T_ACTION_ENUM> : _ATableConfig<InputActionPathConfigItem<T_ACTION_ENUM>> 
        where T_ACTION_ENUM : Enum
    {
    }
}