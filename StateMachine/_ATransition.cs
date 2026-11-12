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
    public abstract class _ATransition<T_STATE_TYPE, T_TARGET> where T_STATE_TYPE : Enum where T_TARGET : class
    {
        private uint _m_enterSerialize = Serialize.Next();
        private StateMachine<T_STATE_TYPE, T_TARGET> _m_stateMachine;
        private _AState<T_STATE_TYPE, T_TARGET> _m_fromState;
        private _AState<T_STATE_TYPE, T_TARGET> _m_toState;


        public virtual string name { get { return GetType().Name; } }
        public uint enterSerialize { get { return _m_enterSerialize; } }
        public _AState<T_STATE_TYPE, T_TARGET> fromState { get { return _m_fromState; } }
        public _AState<T_STATE_TYPE, T_TARGET> toState { get { return _m_toState; } }


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
        /// Use this hook to cancel business work such as an animation when the implementation supports it.
        /// </summary>
        protected virtual void OnExit(bool _completed) { }


        internal void Bind([NotNull] StateMachine<T_STATE_TYPE, T_TARGET> _machine,
            _AState<T_STATE_TYPE, T_TARGET> _from, _AState<T_STATE_TYPE, T_TARGET> _to)
        {
            ValidateEntry();
            _m_stateMachine = _machine;
            _m_fromState = _from;
            _m_toState = _to;
        }
        internal void ValidateEntry()
        {
            if (_m_stateMachine != null)
                throw new InvalidOperationException("A transition instance cannot be active in two machines.");
        }
        internal void Enter([NotNull] Action _complete) { OnEnter(_complete); }
        internal void Update() { OnUpdate(); }
        internal void Exit(bool _completed)
        {
            try
            {
                OnExit(_completed);
            }
            finally
            {
                Unbind();
            }
        }
        internal void Unbind()
        {
            _m_enterSerialize = Serialize.Next();
            _m_stateMachine = null;
            _m_fromState = null;
            _m_toState = null;
        }
    }
}
