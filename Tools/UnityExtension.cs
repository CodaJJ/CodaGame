// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    public static class UnityExtension
    {
        /// <summary>
        /// Depth-first traverse all descendants of this transform.
        /// </summary>
        /// <remarks>
        /// <para>The root transform itself is not visited — only its descendants.</para>
        /// <para>
        /// The visitor is called for each descendant node. Return <see cref="ETraverseOp.Continue"/> to
        /// descend into the node's children, <see cref="ETraverseOp.Skip"/> to skip the subtree,
        /// or <see cref="ETraverseOp.Stop"/> to terminate the entire traversal.
        /// </para>
        /// </remarks>
        /// <param name="_root">The root transform whose descendants will be traversed.</param>
        /// <param name="_visitor">
        /// A function invoked for each descendant. Returns an <see cref="ETraverseOp"/> to control traversal.
        /// </param>
        public static void TraverseChildren(this Transform _root, NotNullFunc<Transform, ETraverseOp> _visitor)
        {
            if (_root == null)
            {
                Console.LogError(SystemNames.Utility, "TraverseChildren: _root is null.");
                return;
            }
            if (_visitor == null)
            {
                Console.LogError(SystemNames.Utility, "TraverseChildren: _visitor is null.");
                return;
            }

            for (int i = 0; i < _root.childCount; i++)
            {
                if (TraverseChildrenRecursive(_root.GetChild(i), _visitor))
                    return;
            }
        }
        /// <summary>
        /// Traverse the parent chain of this transform, starting from its direct parent.
        /// </summary>
        /// <remarks>
        /// <para>The transform itself is not visited — only its ancestors.</para>
        /// <para>
        /// The visitor is called for each ancestor. Return <see cref="ETraverseOp.Continue"/> to
        /// move to the next parent, or <see cref="ETraverseOp.Skip"/>/<see cref="ETraverseOp.Stop"/>
        /// to terminate the traversal (both have the same effect in a linear chain).
        /// </para>
        /// </remarks>
        /// <param name="_root">The transform whose ancestors will be traversed.</param>
        /// <param name="_visitor">
        /// A function invoked for each ancestor. Returns an <see cref="ETraverseOp"/> to control traversal.
        /// </param>
        public static void TraverseParents(this Transform _root, Func<Transform, ETraverseOp> _visitor)
        {
            if (_root == null)
            {
                Console.LogError(SystemNames.Utility, "TraverseParents: _root is null.");
                return;
            }
            if (_visitor == null)
            {
                Console.LogError(SystemNames.Utility, "TraverseParents: _visitor is null.");
                return;
            }

            Transform current = _root.parent;
            while (current != null)
            {
                if (_visitor(current) != ETraverseOp.Continue)
                    return;

                current = current.parent;
            }
        }
        
        
        /// <returns>True if the entire traversal should stop.</returns>
        private static bool TraverseChildrenRecursive([NotNull] Transform _current, [NotNull] NotNullFunc<Transform, ETraverseOp> _visitor)
        {
            ETraverseOp op = _visitor(_current);

            if (op == ETraverseOp.Stop)
                return true;
            if (op == ETraverseOp.Skip)
                return false;

            // Continue: descend into children
            for (int i = 0; i < _current.childCount; i++)
            {
                if (TraverseChildrenRecursive(_current.GetChild(i), _visitor))
                    return true;
            }

            return false;
        }
    }
}