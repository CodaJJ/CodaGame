// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// Controls the traversal behavior when visiting a node in a tree traversal.
    /// </summary>
    public enum ETraverseOp
    {
        /// <summary>
        /// Continue traversing into this node's children.
        /// </summary>
        Continue,
        /// <summary>
        /// Skip this node's subtree and move on to the next sibling.
        /// </summary>
        Skip,
        /// <summary>
        /// Stop the entire traversal immediately.
        /// </summary>
        Stop,
    }
}
