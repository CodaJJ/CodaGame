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
        /// Whether this widget has been initialized.
        /// </summary>
        public bool isInitialized { get { return _m_isInitialized; } }


        /// <summary>
        /// Trigger a presentation refresh of this widget.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Calls <see cref="OnRefresh"/>. Calls made before the widget is initialized are silently ignored —
        /// the framework invokes this automatically once initialization completes, so subclasses do not need
        /// a first paint of their own.
        /// </para>
        /// <para>
        /// For widgets that refresh from caller-supplied data, add a typed overload (e.g. <c>Refresh(data)</c>)
        /// that stores the data into fields and then calls this method.
        /// </para>
        /// </remarks>
        public void Refresh()
        {
            if (!_m_isInitialized)
                return;

            OnRefresh();
        }


        /// <summary>
        /// Called once when the widget is initialized. Use for one-time setup.
        /// </summary>
        protected virtual void OnInit() { }
        /// <summary>
        /// Called to refresh the widget's visual presentation.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Invoked automatically once after initialization, and again on every <see cref="Refresh"/> call.
        /// At this point initialization is complete, so child widgets and cached data are ready to read.
        /// </para>
        /// <para>Write only presentation updates here — one-time setup belongs in <see cref="OnInit"/>.</para>
        /// </remarks>
        protected virtual void OnRefresh() { }
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

            OnInit();
            InitChildWidgets();

            _m_isInitialized = true;
            Refresh();

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
