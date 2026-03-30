// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// Utility methods for <see cref="Action"/> delegates.
    /// </summary>
    public static class ActionUtility
    {
        /// <summary>
        /// Invoke the action and clear the reference.
        /// </summary>
        /// <remarks>
        /// <para>The reference is cleared before invoking, so new callbacks added during invocation are not lost.</para>
        /// </remarks>
        public static void InvokeAndClear(ref Action _action)
        {
            if (_action == null) return;
            Action cb = _action;
            _action = null;
            cb.Invoke();
        }
    }
}
