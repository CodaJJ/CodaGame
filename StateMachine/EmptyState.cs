// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame.StateMachine
{
    /// <summary>
    /// A state whose lifecycle hooks do nothing, with a caller-supplied state type.
    /// </summary>
    /// <remarks>
    /// Normal state machine transition bookkeeping and logging still apply.
    /// </remarks>
    public sealed class EmptyState<T_STATE_TYPE, T_TARGET> : _AState<T_STATE_TYPE, T_TARGET> where T_STATE_TYPE : Enum where T_TARGET : class
    {
        private readonly T_STATE_TYPE _m_type;


        /// <param name="_type">The caller-selected type for this empty state.</param>
        public EmptyState(T_STATE_TYPE _type)
        {
            _m_type = _type;
        }


        public override T_STATE_TYPE type { get { return _m_type; } }
        public override string name { get { return "EmptyState"; } }


        protected override void OnEnter() { }
        protected override void OnExit() { }
        protected override void OnUpdate() { }
    }
}
