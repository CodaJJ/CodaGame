// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using CodaGame.StateMachine.TargetedLite;
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
    /// For show/hide animations, override <see cref="OnPanelShow"/> and <see cref="OnPanelHide"/>,
    /// and invoke the '_complete' callback when the animation finishes.
    /// </para>
    /// </remarks>
    public abstract class _AUIPanel : MonoBehaviour
    {
        [SerializeField] private int _m_layer;

        private AssetIndex _m_panelAssetIndex;
        private bool _m_isDestroyed;

        // Active reference count
        private int _m_activeRef;

        // Callbacks
        private Action _m_openComplete;
        private Action _m_hideComplete;
        private Action _m_destroyComplete;

        // State machine
        private StateMachine<EUIPanelState, _AUIPanel> _m_stateMachine;


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
        public EUIPanelState panelState { get { return _m_stateMachine.curState.type; } }
        /// <summary>
        /// Whether this panel has been destroyed.
        /// </summary>
        public bool isDestroyed { get { return _m_isDestroyed; } }
        /// <summary>
        /// The name of this panel, used for logging and debugging. Defaults to the class name, but can be overridden for more descriptive names.
        /// </summary>
        public virtual string panelName { get { return GetType().Name; } }


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
            _m_stateMachine.Update(0);
            
            if (panelState == EUIPanelState.Active)
                _complete?.Invoke();
            else if (_complete != null)
                _m_openComplete += _complete;
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
            _m_stateMachine.Update(0);

            if (panelState == EUIPanelState.Hidden)
                _complete?.Invoke();
            else if (_complete != null)
                _m_hideComplete += _complete;
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

            _m_isDestroyed = true;
            _m_activeRef = 0;
            if (_complete != null)
                _m_destroyComplete += _complete;

            Console.LogVerbose(SystemNames.UI, panelName, "Destroy requested.");
            _m_stateMachine.Update(0);
        }


        /// <summary>
        /// Called once after the panel is instantiated, before the first show.
        /// </summary>
        /// <remarks>
        /// <para>Use this for one-time initialization such as caching component references.</para>
        /// </remarks>
        protected virtual void OnPanelInit() { }
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
        protected virtual void OnPanelShow([NotNull] Action _complete) { _complete.Invoke(); }
        /// <summary>
        /// Called when the panel has fully entered the Active state.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The panel is now fully visible and interactive.
        /// Register input handlers or begin gameplay logic here.
        /// </para>
        /// </remarks>
        protected virtual void OnPanelActiveEnter() { }
        /// <summary>
        /// Called when the panel is about to leave the Active state (before hide animation).
        /// </summary>
        /// <remarks>
        /// <para>Unregister input handlers or pause gameplay logic here.</para>
        /// </remarks>
        protected virtual void OnPanelActiveExit() { }
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
        protected virtual void OnPanelHide(Action _complete) { _complete?.Invoke(); }
        /// <summary>
        /// Called before the panel is destroyed.
        /// </summary>
        /// <remarks>
        /// <para>Use this for final cleanup. The GameObject will be destroyed immediately after this call.</para>
        /// </remarks>
        protected virtual void OnPanelDestroy() { }


        /// <summary>
        /// Initialize the panel's internal state. Called by UIManager after instantiation.
        /// </summary>
        internal void Init(AssetIndex _assetIndex)
        {
            _m_panelAssetIndex = _assetIndex;
            _m_stateMachine = new StateMachine<EUIPanelState, _AUIPanel>(this, panelName);
            _m_stateMachine.ChangeState(new HiddenState());

            OnPanelInit();
        }
        /// <summary>
        /// Destroy this panel immediately, skipping any hide animation.
        /// Open callbacks are cleared without invoking. Hide and destroy callbacks are invoked normally.
        /// </summary>
        internal void DestroyImmediate()
        {
            if (_m_isDestroyed)
                return;

            Console.LogVerbose(SystemNames.UI, panelName, "Destroying panel immediately.");

            _m_isDestroyed = true;
            _m_activeRef = 0;

            // Open callbacks are meaningless — the panel never finished opening
            _m_openComplete = null;

            // If active, notify exit
            if (panelState == EUIPanelState.Active)
                OnPanelActiveExit();

            DestroyInternal();

            ActionUtility.InvokeAndClear(ref _m_hideComplete);
            ActionUtility.InvokeAndClear(ref _m_destroyComplete);
        }


        /// <summary>
        /// Check if this panel is in a valid state. Returns true if the panel should not be used.
        /// </summary>
        /// <remarks>
        /// <para>Detects two invalid cases: panel created via 'new' instead of <see cref="UI.Create"/>, and panel already destroyed.</para>
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

            if (_m_isDestroyed)
            {
                Console.LogError(SystemNames.UI, panelName, "Panel has already been destroyed and should not be used.");
                return true;
            }

            return false;
        }


        private void DestroyInternal()
        {
            OnPanelDestroy();
            UIManager.instance.DestroyPanel(this);
        }


        #region States
        // Hidden: panel in memory but not visible.
        private class HiddenState : _AState<EUIPanelState, _AUIPanel>
        {
            public override EUIPanelState type { get { return EUIPanelState.Hidden; } }
            public override string name { get { return "HiddenState"; } }


            protected override void OnEnter()
            {
                target.gameObject.SetActive(false);
                Console.LogVerbose(SystemNames.UI, target.panelName, "Panel is now hidden.");

                ActionUtility.InvokeAndClear(ref target._m_hideComplete);
                
                // Re-evaluate in case state changed during callbacks
                target._m_stateMachine.Update(0);
            }
            protected override void OnExit()
            {
                target.gameObject.SetActive(true);
            }
            protected override void OnUpdate(float _)
            {
                // Destroy takes priority
                if (target._m_isDestroyed)
                {
                    target.DestroyInternal();
                    Console.LogVerbose(SystemNames.UI, target.panelName, "Panel destroyed.");
                    ActionUtility.InvokeAndClear(ref target._m_destroyComplete);
                    return;
                }

                // Check if should show
                if (target._m_activeRef > 0)
                    ChangeState(new ShowingState());
            }
        }
        // Showing: playing show animation.
        private class ShowingState : _AState<EUIPanelState, _AUIPanel>
        {
            public override EUIPanelState type { get { return EUIPanelState.Showing; } }
            public override string name { get { return "ShowingState"; } }


            protected override void OnEnter()
            {
                Console.LogVerbose(SystemNames.UI, target.panelName, "Showing panel...");

                uint serialize = enterSerialize;
                target.OnPanelShow(() =>
                {
                    if (serialize != enterSerialize)
                        return;

                    ChangeState(new ActiveState());
                });
            }
            protected override void OnExit() { }
            protected override void OnUpdate(float _) { }
        }

        // Active: panel fully visible.
        private class ActiveState : _AState<EUIPanelState, _AUIPanel>
        {
            public override EUIPanelState type { get { return EUIPanelState.Active; } }
            public override string name { get { return "ActiveState"; } }


            protected override void OnEnter()
            {
                target.OnPanelActiveEnter();
                Console.LogVerbose(SystemNames.UI, target.panelName, "Panel is now active.");

                ActionUtility.InvokeAndClear(ref target._m_openComplete);

                // Re-evaluate in case state changed during callbacks
                target._m_stateMachine.Update(0);
            }
            protected override void OnExit() { }
            protected override void OnUpdate(float _)
            {
                // Destroy or activeRef dropped to 0
                if (target._m_isDestroyed || target._m_activeRef <= 0)
                {
                    target.OnPanelActiveExit();
                    ChangeState(new HidingState());
                }
            }
        }
        // Hiding: playing hide animation.
        private class HidingState : _AState<EUIPanelState, _AUIPanel>
        {
            public override EUIPanelState type { get { return EUIPanelState.Hiding; } }
            public override string name { get { return "HidingState"; } }


            protected override void OnEnter()
            {
                Console.LogVerbose(SystemNames.UI, target.panelName, "Hiding panel...");

                uint serialize = enterSerialize;
                target.OnPanelHide(() =>
                {
                    if (serialize != enterSerialize)
                        return;

                    ChangeState(new HiddenState());
                });
            }
            protected override void OnExit() { }
            protected override void OnUpdate(float _) { }
        }
        #endregion
    }
}
