// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// The lifecycle state of a UI panel.
    /// </summary>
    public enum EUIPanelState
    {
        /// <summary>
        /// Panel is playing its show animation.
        /// </summary>
        Showing,
        /// <summary>
        /// Panel is fully visible and interactive.
        /// </summary>
        Active,
        /// <summary>
        /// Panel is playing its hide animation.
        /// </summary>
        Hiding,
        /// <summary>
        /// Panel is hidden but still in memory. Can be re-shown without loading.
        /// </summary>
        Hidden
    }
}
