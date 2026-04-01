// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Abstract base class for all flows managed by the Flow system.
    /// </summary>
    /// <remarks>
    /// <para>Derive from this class to create concrete flows.</para>
    /// <para>Enter flows via <see cref="Flow.Enter"/>.</para>
    /// <para>Override lifecycle callbacks to implement custom behavior.</para>
    /// <para>
    /// For resource loading, override <see cref="OnEnter"/> and invoke '_complete' when resources are ready.
    /// </para>
    /// </remarks>
    [PublicAPI]
    public abstract class _AFlow
    {
        private EFlowState _m_state;


        /// <summary>
        /// The display name of this flow, used for debug logging and Exit validation.
        /// Defaults to the class name, but can be overridden for more descriptive names.
        /// </summary>
        public virtual string flowName { get { return GetType().Name; } }
        /// <summary>
        /// The current lifecycle state of this flow.
        /// </summary>
        public EFlowState flowState { get { return _m_state; } }


        /// <summary>
        /// Exit this flow. Only valid when this flow is the topmost active flow.
        /// </summary>
        /// <remarks>
        /// <para>Equivalent to calling <see cref="Flow.Exit(_AFlow)"/> with this instance.</para>
        /// </remarks>
        protected void Exit()
        {
            Flow.Exit(this);
        }

        /// <summary>
        /// Called when the flow enters. Load resources and invoke '_complete' when ready.
        /// </summary>
        /// <remarks>
        /// <para>You MUST invoke '_complete' when the enter process is finished.</para>
        /// <para>The default implementation invokes '_complete' immediately (no loading).</para>
        /// </remarks>
        /// <param name="_complete">Callback that must be invoked when the enter process is complete.</param>
        protected virtual void OnEnter([NotNull] Action _complete) { _complete.Invoke(); }
        /// <summary>
        /// Called when the flow is suspended by an Exclusive flow.
        /// </summary>
        /// <remarks>
        /// <para>Pause game logic, input handling, etc.</para>
        /// </remarks>
        protected virtual void OnPause() { }
        /// <summary>
        /// Called when the flow is restored after an Exclusive flow exits.
        /// </summary>
        /// <remarks>
        /// <para>Resume game logic, input handling, etc.</para>
        /// </remarks>
        protected virtual void OnResume() { }
        /// <summary>
        /// Called when the flow exits. Clean up resources.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The '_isCovered' parameter indicates whether a loading screen is covering the screen.
        /// When true, skip exit animations and destroy resources immediately.
        /// When false, play exit animations and manage destruction gracefully.
        /// </para>
        /// <para>
        /// This can be called from either the Active or Paused state.
        /// During a Replace transition, paused flows are exited directly without being resumed first.
        /// </para>
        /// <para>After this call, the Flow system no longer manages this instance.</para>
        /// </remarks>
        /// <param name="_isCovered">Whether a loading screen is covering the screen.</param>
        protected virtual void OnExit(bool _isCovered) { }


        /// <summary>
        /// Set the flow state. Called by FlowManager.
        /// </summary>
        internal void SetState(EFlowState _state)
        {
            _m_state = _state;
        }
        /// <summary>
        /// Invoke OnEnter. Called by FlowManager.
        /// </summary>
        internal void InvokeOnEnter([NotNull] Action _complete)
        {
            _m_state = EFlowState.Entering;
            Console.LogVerbose(SystemNames.Flow, flowName, "Entering...");
            OnEnter(_complete);
        }
        /// <summary>
        /// Invoke OnPause. Called by FlowManager.
        /// </summary>
        internal void InvokeOnPause()
        {
            _m_state = EFlowState.Paused;
            Console.LogVerbose(SystemNames.Flow, flowName, "Paused.");
            OnPause();
        }
        /// <summary>
        /// Invoke OnResume. Called by FlowManager.
        /// </summary>
        internal void InvokeOnResume()
        {
            _m_state = EFlowState.Active;
            Console.LogVerbose(SystemNames.Flow, flowName, "Resumed.");
            OnResume();
        }
        /// <summary>
        /// Invoke OnExit. Called by FlowManager.
        /// </summary>
        internal void InvokeOnExit(bool _isCovered)
        {
            _m_state = EFlowState.None;
            Console.LogVerbose(SystemNames.Flow, flowName, $"Exited (isCovered={_isCovered}).");
            OnExit(_isCovered);
        }
    }
}
