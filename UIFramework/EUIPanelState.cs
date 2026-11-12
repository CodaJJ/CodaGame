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
        /// Panel has been constructed but has not been initialized by UI.Create yet.
        /// </summary>
        None,
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
        Hidden,
        /// <summary>
        /// Terminal state. The panel is being disposed and cannot be opened again.
        /// </summary>
        Destroyed
    }
}
