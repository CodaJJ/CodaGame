// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;

namespace CodaGame.StateMachine
{
    /// <summary>A state bound to a strongly typed owner for the duration of its entry.</summary>
    /// <remarks>
    /// Supply entry data through the constructor; use separate instances for different queued data.
    /// Constructors prepare data only: an interrupted transition may never enter its destination.
    /// Sequential reuse and same-instance reentry are supported. Async business callbacks should
    /// capture enterSerialize and check it before using target, which is cleared when Exit returns.
    /// </remarks>
    public abstract class _AState<T_STATE_TYPE, T_TARGET> where T_STATE_TYPE : Enum where T_TARGET : class
    {
        private uint _m_enterSerialize = Serialize.Next();
        private StateMachine<T_STATE_TYPE, T_TARGET> _m_stateMachine;


        public uint enterSerialize { get { return _m_enterSerialize; } }
        public abstract T_STATE_TYPE type { get; }
        public abstract string name { get; }


        protected StateMachine<T_STATE_TYPE, T_TARGET> stateMachine { get { return _m_stateMachine; } }
        protected T_TARGET target { get { return _m_stateMachine?.target; } }


        protected void ChangeState(_AState<T_STATE_TYPE, T_TARGET> _state,
            _ATransition<T_STATE_TYPE, T_TARGET> _transition = null, bool _interruptTransition = false)
        {
            if (_m_stateMachine == null)
                throw new InvalidOperationException("Cannot change state from an inactive state.");

            _m_stateMachine.ChangeState(_state, _transition, _interruptTransition);
        }
        protected abstract void OnEnter();
        protected abstract void OnExit();
        /// <summary>Update business behavior; obtain any timing data from the owner or business APIs.</summary>
        protected abstract void OnUpdate();


        internal void ValidateEntry([NotNull] StateMachine<T_STATE_TYPE, T_TARGET> _machine)
        {
            if (_m_stateMachine != null && !ReferenceEquals(_m_stateMachine, _machine))
                throw new InvalidOperationException("A state instance cannot be active in two machines.");
        }
        internal void Enter([NotNull] StateMachine<T_STATE_TYPE, T_TARGET> _machine)
        {
            ValidateEntry(_machine);
            _m_stateMachine = _machine;
            OnEnter();
        }
        internal void Exit()
        {
            try
            {
                OnExit();
            }
            finally
            {
                _m_enterSerialize = Serialize.Next();
                _m_stateMachine = null;
            }
        }
        internal void Update() { OnUpdate(); }
    }
}
