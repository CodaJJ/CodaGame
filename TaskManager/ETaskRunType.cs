// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// Task running type.
    /// </summary>
    public enum ETaskRunType
    {
        /// <summary>
        /// Will run in Unity's Update.
        /// </summary>
        Update,
        /// <summary>
        /// Will run in Unity's FixedUpdate.
        /// </summary>
        FixedUpdate,
        /// <summary>
        /// Will run in Unity's LateUpdate.
        /// </summary>
        LateUpdate,
    }
}