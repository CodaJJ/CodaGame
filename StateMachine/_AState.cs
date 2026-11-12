// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;

namespace CodaGame.StateMachine
{
    /// <summary>
    /// A state bound to a strongly typed owner for the duration of its entry.
    /// </summary>
    /// <remarks>
    /// Supply entry data through the constructor of a fresh instance for each change.
    /// Constructors prepare data only: an interrupted transition may never enter its destination.
    /// Async business callbacks should capture enterSerialize and check it before using target.
    /// </remarks>
    public abstract class _AState<T_STATE_TYPE, T_TARGET>
        where T_STATE_TYPE : struct, Enum
        where T_TARGET : class
    {
        private uint _m_enterSerialize = Serialize.Next();
        private StateMachine<T_STATE_TYPE, T_TARGET> _m_stateMachine;


        public uint enterSerialize { get { return _m_enterSerialize; } }
        /// <summary>
        /// Identifies this state. Return the same value throughout the instance's lifetime, including before Enter and after Exit.
        /// </summary>
        public abstract T_STATE_TYPE type { get; }
        public abstract string name { get; }


        protected StateMachine<T_STATE_TYPE, T_TARGET> stateMachine { get { return _m_stateMachine; } }
        protected T_TARGET target { get { return _m_stateMachine?.target; } }


        /// <summary>
        /// Describe this state by name and type for logs and diagnostics.
        /// </summary>
        [NotNull]
        public string Describe()
        {
            return $"{name} [{type}]";
        }


        /// <summary>
        /// Whether this state allows a change to the given type. Null requests clearing the state.
        /// </summary>
        /// <remarks>Checked at request time, possibly before Enter. Keep this a pure check independent of the active binding.</remarks>
        protected internal virtual bool CanChangeTo(T_STATE_TYPE? _nextStateType) { return true; }

        protected void ChangeState(_AState<T_STATE_TYPE, T_TARGET> _state, _ATransition<T_STATE_TYPE, T_TARGET> _transition = null)
        {
            if (_m_stateMachine == null)
            {
                Console.LogError(SystemNames.StateMachine, "Cannot change state from an inactive state.");
                return;
            }

            _m_stateMachine.ChangeState(_state, _transition);
        }
        protected abstract void OnEnter();
        protected abstract void OnExit();
        /// <summary>
        /// Update business behavior; obtain any timing data from the owner or business APIs.
        /// </summary>
        protected abstract void OnUpdate();


        internal void Enter([NotNull] StateMachine<T_STATE_TYPE, T_TARGET> _machine)
        {
            _m_stateMachine = _machine;
            OnEnter();
        }
        internal void Update()
        {
            OnUpdate();
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
    }
}
