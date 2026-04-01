// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// The lifecycle state of a flow.
    /// </summary>
    public enum EFlowState
    {
        /// <summary>
        /// Flow has not entered or has already exited.
        /// </summary>
        None,
        /// <summary>
        /// Flow is loading resources (OnEnter async in progress).
        /// </summary>
        Entering,
        /// <summary>
        /// Flow is fully active and running.
        /// </summary>
        Active,
        /// <summary>
        /// Flow is suspended by an Exclusive flow.
        /// </summary>
        Paused
    }
}
