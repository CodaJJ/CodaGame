// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using CodaGame.StateMachine;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Abstract base class for all UI panels managed by the UI framework.
    /// </summary>
    /// <remarks>
    /// <para>Attach a derived class to the root GameObject of a panel prefab.</para>
    /// <para>Create panel instances via <see cref="UI.Create"/>.</para>
    /// <para>Override lifecycle hooks to implement custom behavior.</para>
    /// <para>
    /// For show/hide animations, override <see cref="OnShow"/> and <see cref="OnHide"/>,
    /// and invoke the '_complete' callback when the animation finishes.
    /// </para>
    /// </remarks>
    public abstract class _AUIPanel : _AUIComponent
    {
        [SerializeField] private int _m_layer;

        private AssetIndex _m_panelAssetIndex;
        private bool _m_isInitialized;
        // Requests are separate from the terminal Destroyed state.
        private bool _m_destroyRequested;
        private bool _m_destroyImmediateRequested;

        // Active reference count
        private int _m_activeRef;

        // Callbacks
        private Action _m_openComplete;
        private Action _m_hideComplete;
        private Action _m_destroyComplete;

        // State machine
        [NotNull] private readonly StateMachine<EUIPanelState, _AUIPanel> _m_stateMachine;


        protected _AUIPanel()
        {
            _m_stateMachine = new StateMachine<EUIPanelState, _AUIPanel>(this, GetType().Name);
            _m_stateMachine.ChangeState(new EmptyState<EUIPanelState, _AUIPanel>(EUIPanelState.None));
        }


        /// <summary>
        /// The layer index that determines which root transform this panel will be parented under.
        /// </summary>
        /// <remarks>
        /// <para>Set this value on the prefab in the Inspector.</para>
        /// </remarks>
        public int layer { get { return _m_layer; } }
        /// <summary>
        /// The asset index that identifies this panel.
        /// </summary>
        public AssetIndex panelAssetIndex { get { return _m_panelAssetIndex; } }
        /// <summary>
        /// The current lifecycle state of this panel.
        /// </summary>
        public EUIPanelState panelState
        {
            get
            {
                if (_m_stateMachine.curTransition is ShowingTransition)
                    return EUIPanelState.Showing;
                if (_m_stateMachine.curTransition is HidingTransition)
                    return EUIPanelState.Hiding;
                return _m_stateMachine.curState.type;
            }
        }
        /// <summary>
        /// Whether this panel has entered the terminal Destroyed state.
        /// A pending destroy request that is still waiting for an animation does not count.
        /// </summary>
        public bool isDestroyed { get { return panelState == EUIPanelState.Destroyed; } }
        /// <summary>
        /// Whether this widget has been initialized.
        /// </summary>
        public bool isInitialized { get { return _m_isInitialized; } }
        /// <summary>
        /// The concrete class name of this panel, used for logging and debugging.
        /// </summary>
        public string panelName { get { return _m_stateMachine.name; } }


        /// <summary>
        /// Open (show) this panel. Increments the active reference count.
        /// </summary>
        /// <remarks>
        /// <para>When active ref goes from 0 to positive, the show animation plays.</para>
        /// </remarks>
        /// <param name="_complete">Callback invoked when the operation completes.</param>
        public void Open(Action _complete = null)
        {
            if (CheckInvalid())
            {
                _complete?.Invoke();
                return;
            }

            _m_activeRef++;
            Console.LogVerbose(SystemNames.UI, panelName, $"Open: active ref={_m_activeRef}.");

            if (_complete != null)
                _m_openComplete += _complete;

            _m_stateMachine.Update();
        }
        /// <summary>
        /// Hide this panel. Decrements the active reference count.
        /// </summary>
        /// <remarks>
        /// <para>When active ref reaches 0, the hide animation plays.</para>
        /// </remarks>
        /// <param name="_complete">Callback invoked when the operation completes.</param>
        public void Hide(Action _complete = null)
        {
            if (CheckInvalid())
            {
                _complete?.Invoke();
                return;
            }

            _m_activeRef--;
            if (_m_activeRef < 0)
            {
                Console.LogWarning(SystemNames.UI, panelName, "Hide called more times than Open. Clamping active ref to 0.");
                _m_activeRef = 0;
            }

            Console.LogVerbose(SystemNames.UI, panelName, $"Hide: active ref={_m_activeRef}.");
            if (_complete != null)
                _m_hideComplete += _complete;

            _m_stateMachine.Update();
        }
        /// <summary>
        /// Destroy this panel and release assets.
        /// </summary>
        /// <remarks>
        /// <para>If the panel is active, it will be auto-hidden first.</para>
        /// <para>After destroy, the panel instance should not be used.</para>
        /// </remarks>
        /// <param name="_complete">Callback invoked when the operation completes.</param>
        public void Destroy(Action _complete = null)
        {
            if (CheckInvalid())
            {
                _complete?.Invoke();
                return;
            }

            _m_destroyRequested = true;
            _m_activeRef = 0;
            if (_complete != null)
                _m_destroyComplete += _complete;

            Console.LogVerbose(SystemNames.UI, panelName, "Destroy requested.");
            _m_stateMachine.Update();
        }


        /// <summary>
        /// Trigger a presentation refresh of this panel.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Calls <see cref="OnRefresh"/>. Calls made before the panel is initialized are silently ignored —
        /// the framework invokes this automatically once initialization completes, so subclasses do not need
        /// a first paint of their own.
        /// </para>
        /// <para>
        /// For panels that refresh from caller-supplied data, add a typed overload (e.g. <c>Refresh(data)</c>)
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
        /// Called once after the panel is instantiated, before the first show.
        /// </summary>
        /// <remarks>
        /// <para>Use this for one-time initialization such as caching component references.</para>
        /// </remarks>
        protected virtual void OnInit() { }
        /// <summary>
        /// Called to refresh the panel's visual presentation.
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
        /// Called when the panel should become visible. Implement show animation here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// You MUST invoke '_complete' when the show process is finished.
        /// The default implementation invokes '_complete' immediately (no animation).
        /// </para>
        /// </remarks>
        /// <param name="_complete">Callback that must be invoked when the show process is complete.</param>
        protected virtual void OnShow([NotNull] Action _complete) { _complete.Invoke(); }
        /// <summary>
        /// Called when the panel has fully entered the Active state.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The panel is now fully visible and interactive.
        /// Register input handlers or begin gameplay logic here.
        /// </para>
        /// </remarks>
        protected virtual void OnActiveEnter() { }
        /// <summary>
        /// Called when the panel is about to leave the Active state (before hide animation).
        /// </summary>
        /// <remarks>
        /// <para>Unregister input handlers or pause gameplay logic here.</para>
        /// </remarks>
        protected virtual void OnActiveExit() { }
        /// <summary>
        /// Called when the panel should become hidden. Implement hide animation here.
        /// </summary>
        /// <remarks>
        /// <para>
        /// You MUST invoke '_complete' when the hide process is finished.
        /// The default implementation invokes '_complete' immediately (no animation).
        /// </para>
        /// </remarks>
        /// <param name="_complete">Callback that must be invoked when the hide process is complete.</param>
        protected virtual void OnHide([NotNull] Action _complete) { _complete.Invoke(); }
        /// <summary>
        /// Called before the panel is destroyed.
        /// </summary>
        /// <remarks>
        /// <para>Use this for final cleanup. The GameObject will be destroyed immediately after this call.</para>
        /// </remarks>
        protected virtual void OnDiscard() { }


        /// <summary>
        /// Initialize the panel's internal state. Called by UIManager after instantiation.
        /// </summary>
        internal void Init(AssetIndex _assetIndex)
        {
            if (panelState != EUIPanelState.None)
                return;

            _m_panelAssetIndex = _assetIndex;

            _m_stateMachine.ChangeState(new HiddenState());

            OnInit();
            InitChildWidgets();

            _m_isInitialized = true;
            Refresh();
        }
        /// <summary>
        /// Destroy this panel immediately, skipping any hide animation.
        /// Open callbacks are cleared without invoking. Hide and destroy callbacks are invoked normally.
        /// </summary>
        internal void DestroyImmediate()
        {
            if (this == null || panelState == EUIPanelState.None || isDestroyed || _m_destroyImmediateRequested)
                return;

            Console.LogVerbose(SystemNames.UI, panelName, "Destroying panel immediately.");

            _m_destroyRequested = true;
            _m_destroyImmediateRequested = true;
            _m_activeRef = 0;

            _m_stateMachine.Update();
        }


        /// <summary>
        /// Check if this panel is in a valid state. Returns true if the panel should not be used.
        /// </summary>
        /// <remarks>
        /// <para>Rejects invalid Unity objects, panels not yet initialized by <see cref="UI.Create"/>, and destroy requests.</para>
        /// </remarks>
        /// <returns>True if the panel is invalid and should not be used.</returns>
        [HideInCallstack]
        protected bool CheckInvalid()
        {
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (this == null)
            {
                Console.LogError(SystemNames.UI, panelName, "Panel was not created through UI.Create. Do not use 'new' to create panels.");
                return true;
            }

            if (panelState == EUIPanelState.None)
            {
                Console.LogError(SystemNames.UI, panelName, "Panel has not been initialized through UI.Create.");
                return true;
            }

            if (_m_destroyRequested)
            {
                Console.LogError(SystemNames.UI, panelName, "Panel destruction has been requested and it should not be used.");
                return true;
            }

            return false;
        }


        #region States
        // Hidden, Active and Destroyed are stable states. Animation belongs to transitions.
        private class HiddenState : _AState<EUIPanelState, _AUIPanel>
        {
            public override EUIPanelState type { get { return EUIPanelState.Hidden; } }
            public override string name { get { return "HiddenState"; } }


            protected override void OnEnter()
            {
                target.gameObject.SetActive(false);
                Console.LogVerbose(SystemNames.UI, target.panelName, "Panel is now hidden.");
                ActionUtility.InvokeAndClear(ref target._m_hideComplete);
            }
            protected override void OnExit() { }
            protected override void OnUpdate()
            {
                if (target._m_destroyRequested)
                {
                    ChangeState(new DestroyedState());
                    return;
                }

                if (target._m_activeRef > 0)
                {
                    ChangeState(new ActiveState(), new ShowingTransition());
                    return;
                }

                ActionUtility.InvokeAndClear(ref target._m_hideComplete);
            }
        }
        private class ActiveState : _AState<EUIPanelState, _AUIPanel>
        {
            public override EUIPanelState type { get { return EUIPanelState.Active; } }
            public override string name { get { return "ActiveState"; } }


            protected override void OnEnter()
            {
                Console.LogVerbose(SystemNames.UI, target.panelName, "Panel is now active.");
                target.OnActiveEnter();
                target.PropagateShowToChildren();
            }
            protected override void OnExit()
            {
                target.OnActiveExit();
                target.PropagateHideToChildren();
            }
            protected override void OnUpdate()
            {
                if (target._m_destroyImmediateRequested)
                {
                    ChangeState(new DestroyedState());
                    return;
                }

                if (target._m_destroyRequested || target._m_activeRef <= 0)
                {
                    ChangeState(new HiddenState(), new HidingTransition());
                    return;
                }

                ActionUtility.InvokeAndClear(ref target._m_openComplete);
            }
        }
        private class DestroyedState : _AState<EUIPanelState, _AUIPanel>
        {
            public override EUIPanelState type { get { return EUIPanelState.Destroyed; } }
            public override string name { get { return "DestroyedState"; } }


            protected override void OnEnter()
            {
                target._m_openComplete = null;
                target.PropagateDestroyToChildren();
                target.OnDiscard();
                UIManager.instance.DestroyPanel(target);

                Console.LogVerbose(SystemNames.UI, target.panelName, "Panel destroyed.");
                ActionUtility.InvokeAndClear(ref target._m_hideComplete);
                ActionUtility.InvokeAndClear(ref target._m_destroyComplete);
            }
            protected override void OnExit() { }
            protected override void OnUpdate() { }
        }
        #endregion


        #region Transitions
        private class ShowingTransition : _ATransition<EUIPanelState, _AUIPanel>
        {
            protected override void OnEnter(Action _complete)
            {
                target.gameObject.SetActive(true);
                Console.LogVerbose(SystemNames.UI, target.panelName, "Showing panel...");
                if (!target._m_destroyImmediateRequested)
                    target.OnShow(_complete);
            }
            protected override void OnUpdate()
            {
                if (target._m_destroyImmediateRequested)
                    stateMachine.ChangeState(new DestroyedState(), _interruptTransition: true);
            }
        }
        private class HidingTransition : _ATransition<EUIPanelState, _AUIPanel>
        {
            protected override void OnEnter(Action _complete)
            {
                Console.LogVerbose(SystemNames.UI, target.panelName, "Hiding panel...");
                if (!target._m_destroyImmediateRequested)
                    target.OnHide(_complete);
            }
            protected override void OnUpdate()
            {
                if (target._m_destroyImmediateRequested)
                    stateMachine.ChangeState(new DestroyedState(), _interruptTransition: true);
            }
        }
        #endregion
    }
}
