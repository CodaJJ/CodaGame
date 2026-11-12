// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;

namespace CodaGame.StateMachine
{
    /// <summary>
    /// An asynchronous operation between exiting one state and entering another.
    /// </summary>
    /// <remarks>
    /// Update is dispatched here while the transition is active. Timing belongs to the implementation.
    /// Pass the supplied completion callback to asynchronous work: the machine rejects duplicate and
    /// stale completions, including callbacks from a previous use of this transition instance.
    /// Other asynchronous business callbacks can capture enterSerialize and compare it before touching target.
    /// </remarks>
    public abstract class _ATransition<T_STATE_TYPE, T_TARGET>
        where T_STATE_TYPE : struct, Enum
        where T_TARGET : class
    {
        private uint _m_enterSerialize = Serialize.Next();
        private StateMachine<T_STATE_TYPE, T_TARGET> _m_stateMachine;
        private _AState<T_STATE_TYPE, T_TARGET> _m_fromState;
        private _AState<T_STATE_TYPE, T_TARGET> _m_toState;


        public virtual string name { get { return GetType().Name; } }
        public uint enterSerialize { get { return _m_enterSerialize; } }
        /// <summary>
        /// The source type while bound; null if unbound or entering from no state.
        /// </summary>
        public T_STATE_TYPE? fromStateType { get { return _m_fromState?.type; } }
        /// <summary>
        /// The destination type while bound; null if unbound or transitioning to no state.
        /// </summary>
        public T_STATE_TYPE? toStateType { get { return _m_toState?.type; } }


        protected StateMachine<T_STATE_TYPE, T_TARGET> stateMachine { get { return _m_stateMachine; } }
        protected T_TARGET target { get { return _m_stateMachine?.target; } }


        /// <summary>
        /// Start the transition. Call complete when the destination state may be entered.
        /// Calling it synchronously still allows this hook to finish first.
        /// </summary>
        protected abstract void OnEnter([NotNull] Action _complete);
        /// <summary>
        /// Update this transition. Obtain any time or other business data from the owning context.
        /// </summary>
        protected virtual void OnUpdate() { }
        /// <summary>
        /// Called before leaving the transition. False means it was interrupted by another change.
        /// Release resources owned by this transition on both completion and interruption.
        /// On interruption, cancel any outstanding work such as an animation when supported.
        /// </summary>
        protected virtual void OnExit(bool _completed) { }


        internal void Enter([NotNull] StateMachine<T_STATE_TYPE, T_TARGET> _machine, _AState<T_STATE_TYPE, T_TARGET> _from, _AState<T_STATE_TYPE, T_TARGET> _to, [NotNull] Action _complete)
        {
            _m_stateMachine = _machine;
            _m_fromState = _from;
            _m_toState = _to;
            OnEnter(_complete);
        }
        internal void Update()
        {
            OnUpdate();
        }
        internal void Exit(bool _completed)
        {
            try
            {
                OnExit(_completed);
            }
            finally
            {
                _m_enterSerialize = Serialize.Next();
                _m_stateMachine = null;
                _m_fromState = null;
                _m_toState = null;
            }
        }
    }
}
