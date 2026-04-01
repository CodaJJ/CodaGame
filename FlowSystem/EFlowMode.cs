// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// The entry mode for a flow.
    /// </summary>
    public enum EFlowMode
    {
        /// <summary>
        /// Push the flow on top of the active stack. Existing flows are unaffected.
        /// </summary>
        Overlay,
        /// <summary>
        /// Suspend the entire active stack (OnPause). The new flow becomes the sole active flow.
        /// When the new flow exits, the suspended stack is restored (OnResume).
        /// </summary>
        Exclusive,
        /// <summary>
        /// Exit all active and suspended flows. The new flow becomes the sole active flow.
        /// </summary>
        Replace
    }
}
