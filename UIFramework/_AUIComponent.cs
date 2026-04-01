// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Abstract base class for all UI components (panels and widgets).
    /// </summary>
    /// <remarks>
    /// <para>Manages child widget collection, registration, and lifecycle propagation.</para>
    /// <para>Only one <see cref="_AUIComponent"/> is allowed per GameObject.</para>
    /// </remarks>
    [DisallowMultipleComponent]
    public abstract class _AUIComponent : MonoBehaviour
    {
        private bool _m_isActive;

        // Child widgets owned by this component
        [ItemNotNull, NotNull] private readonly List<_AUIWidget> _m_childWidgets = new List<_AUIWidget>();


        /// <summary>
        /// Collect child widgets from the hierarchy and initialize them.
        /// </summary>
        /// <remarks>
        /// <para>Depth-first traversal stops at ownership boundaries (<see cref="_AUIComponent"/> nodes).</para>
        /// <para>Only <see cref="_AUIWidget"/> children are collected; <see cref="_AUIPanel"/> children are skipped.</para>
        /// </remarks>
        protected void InitChildWidgets()
        {
            // Collect static child widgets (stop at ownership boundaries)
            transform.TraverseChildren(_child =>
            {
                _AUIComponent component = _child.GetComponent<_AUIComponent>();
                if (component == null)
                    return ETraverseOp.Continue;

                if (component is _AUIWidget widget)
                    _m_childWidgets.Add(widget);

                return ETraverseOp.Skip;
            });

            // Init all child widgets
            for (int i = 0; i < _m_childWidgets.Count; i++)
                _m_childWidgets[i].Init();
        }

        /// <summary>
        /// Propagate show event to all child widgets.
        /// </summary>
        protected void PropagateShowToChildren()
        {
            _m_isActive = true;

            for (int i = 0; i < _m_childWidgets.Count; i++)
                _m_childWidgets[i].TriggerShow();
        }
        /// <summary>
        /// Propagate hide event to all child widgets in reverse order.
        /// </summary>
        protected void PropagateHideToChildren()
        {
            _m_isActive = false;

            for (int i = _m_childWidgets.Count - 1; i >= 0; i--)
                _m_childWidgets[i].TriggerHide();
        }
        /// <summary>
        /// Propagate destroy event to all child widgets in reverse order, then clear the list.
        /// </summary>
        protected void PropagateDestroyToChildren()
        {
            for (int i = _m_childWidgets.Count - 1; i >= 0; i--)
                _m_childWidgets[i].TriggerDestroy();

            _m_childWidgets.Clear();
        }

        /// <summary>
        /// Register a dynamically created prefab widget as a child.
        /// </summary>
        internal void RegisterChildWidget([NotNull] _AUIPrefab _widget)
        {
            _m_childWidgets.Add(_widget);
            _widget.SetOwner(this);

            if (_m_isActive)
                _widget.TriggerShow();
        }
        /// <summary>
        /// Unregister a child prefab widget from this component's ownership.
        /// </summary>
        internal void UnregisterChildWidget([NotNull] _AUIPrefab _widget)
        {
            if (_m_isActive)
                _widget.TriggerHide();

            _m_childWidgets.Remove(_widget);
        }
    }
}
