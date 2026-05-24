// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
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
        public static void SafeSetActive(this GameObject _obj, bool _active)
        {
            if (_obj == null)
            {
                Console.LogError(SystemNames.Utility, "SafeSetActive: _obj is null.");
                return;
            }

            if (_obj.activeSelf != _active)
                _obj.SetActive(_active);
        }
        public static void SafeSetActive(this List<GameObject> _objList, bool _active)
        {
            if (_objList == null)
            {
                Console.LogError(SystemNames.Utility, "SafeSetActive: _objList is null.");
                return;
            }

            for (int i = 0; i < _objList.Count; i++)
            {
                GameObject obj = _objList[i];
                if (obj == null)
                    continue;

                obj.SafeSetActive(_active);
            }
        }
        /// <summary>
        /// Play the named clip on this <see cref="Animation"/> component and invoke <paramref name="_complete"/>
        /// after the clip finishes.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Completion is scheduled via <see cref="Task.RunDelayActionTask"/> using the clip's length divided by its
        /// playback speed; the callback fires on the main thread. Looping clips will still trigger the callback
        /// after one nominal pass — callers are responsible for not mixing this with <see cref="WrapMode.Loop"/>.
        /// </para>
        /// <para>
        /// If the clip is missing on the component, an error is logged and <paramref name="_complete"/> is not invoked.
        /// </para>
        /// </remarks>
        /// <param name="_animation">The animation component.</param>
        /// <param name="_clipName">The clip name registered on the component.</param>
        /// <param name="_complete">Optional callback fired after the clip's nominal duration elapses.</param>
        public static void Play(this Animation _animation, string _clipName, Action _complete)
        {
            if (_animation == null)
            {
                Console.LogError(SystemNames.Utility, "Play: _animation is null.");
                return;
            }
            if (string.IsNullOrEmpty(_clipName))
            {
                Console.LogError(SystemNames.Utility, "Play: _clipName is null or empty.");
                return;
            }

            AnimationState state = _animation[_clipName];
            if (state == null)
            {
                Console.LogError(SystemNames.Utility, $"Play: clip '{_clipName}' is not registered on the Animation component.");
                return;
            }

            _animation.Play(_clipName);

            if (_complete == null)
                return;

            float speed = Mathf.Abs(state.speed);
            if (speed <= Mathf.Epsilon)
            {
                Console.LogError(SystemNames.Utility, $"Play: clip '{_clipName}' has zero speed; completion callback will not fire.");
                return;
            }
            Task.RunDelayActionTask(_complete, state.length / speed);
        }
        /// <summary>
        /// Sample the named clip at a normalized time without playing it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <paramref name="_normalizedTime"/> is clamped to [0, 1] and mapped to the clip's length. The clip's
        /// <see cref="AnimationState"/> is temporarily enabled with weight 1 so that <see cref="Animation.Sample"/>
        /// evaluates it, then restored to disabled.
        /// </para>
        /// </remarks>
        /// <param name="_animation">The animation component.</param>
        /// <param name="_clipName">The clip name registered on the component.</param>
        /// <param name="_normalizedTime">Normalized time in [0, 1].</param>
        public static void Sample(this Animation _animation, string _clipName, float _normalizedTime)
        {
            if (_animation == null)
            {
                Console.LogError(SystemNames.Utility, "Sample: _animation is null.");
                return;
            }
            if (string.IsNullOrEmpty(_clipName))
            {
                Console.LogError(SystemNames.Utility, "Sample: _clipName is null or empty.");
                return;
            }

            AnimationState state = _animation[_clipName];
            if (state == null)
            {
                Console.LogError(SystemNames.Utility, $"Sample: clip '{_clipName}' is not registered on the Animation component.");
                return;
            }

            bool prevEnabled = state.enabled;
            float prevWeight = state.weight;
            float prevTime = state.time;

            state.enabled = true;
            state.weight = 1f;
            state.time = Mathf.Clamp01(_normalizedTime) * state.length;
            _animation.Sample();

            state.time = prevTime;
            state.weight = prevWeight;
            state.enabled = prevEnabled;
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