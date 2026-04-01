// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Abstract base class for static UI widgets — reusable sub-components within panels or other widgets.
    /// </summary>
    /// <remarks>
    /// <para>Static widgets are placed in the prefab hierarchy and auto-collected on panel/widget init.</para>
    /// <para>Lifecycle is fully managed by the owner — no public Destroy.</para>
    /// <para>For dynamically loaded widgets, use <see cref="_AUIPrefab"/>.</para>
    /// </remarks>
    [PublicAPI]
    public abstract class _AUIWidget : _AUIComponent
    {
        private bool _m_isInitialized;
        private bool _m_isDestroyed;


        /// <summary>
        /// The name of this widget, used for logging and debugging. Defaults to the class name.
        /// </summary>
        public virtual string widgetName { get { return GetType().Name; } }
        /// <summary>
        /// Whether this widget has been destroyed.
        /// </summary>
        public bool isDestroyed { get { return _m_isDestroyed; } }


        /// <summary>
        /// Called once when the widget is initialized. Use for one-time setup.
        /// </summary>
        protected virtual void OnInit() { }
        /// <summary>
        /// Called when the owning panel enters the Active state.
        /// </summary>
        protected virtual void OnActiveEnter() { }
        /// <summary>
        /// Called when the owning panel exits the Active state.
        /// </summary>
        protected virtual void OnActiveExit() { }
        /// <summary>
        /// Called before the widget is destroyed. Use for final cleanup.
        /// </summary>
        protected virtual void OnDiscard() { }


        /// <summary>
        /// Initialize this widget and collect child widgets.
        /// </summary>
        internal void Init()
        {
            if (_m_isInitialized)
                return;

            _m_isInitialized = true;

            InitChildWidgets();

            OnInit();
            Console.LogVerbose(SystemNames.UI, widgetName, "Widget initialized.");
        }
        /// <summary>
        /// Propagate show event to this widget and all child widgets.
        /// </summary>
        internal void TriggerShow()
        {
            if (_m_isDestroyed)
                return;

            OnActiveEnter();
            PropagateShowToChildren();
        }
        /// <summary>
        /// Propagate hide event to this widget and all child widgets.
        /// </summary>
        internal void TriggerHide()
        {
            if (_m_isDestroyed)
                return;

            PropagateHideToChildren();
            OnActiveExit();
        }
        /// <summary>
        /// Propagate destroy event to this widget and all child widgets.
        /// </summary>
        internal virtual void TriggerDestroy()
        {
            if (_m_isDestroyed)
                return;

            _m_isDestroyed = true;

            PropagateDestroyToChildren();

            OnDiscard();
            Console.LogVerbose(SystemNames.UI, widgetName, "Widget destroyed.");
        }
    }
}
