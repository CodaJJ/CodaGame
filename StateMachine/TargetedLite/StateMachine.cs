// Copyright (c) 2024 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame.StateMachine.TargetedLite
{
    /// <summary>
    /// A state machine lite with a target that can change state.
    /// </summary>
    /// <remarks>
    /// <para>This machine handles the state change, invoking the enter, exit and update of the state for the target.</para>
    /// </remarks>
    public class StateMachine<T_STATE_TYPE, T_TARGET> where T_STATE_TYPE : Enum
    {
        // The current state.
        private _AStateBase<T_STATE_TYPE, T_TARGET> _m_curState;
        // Whether the state machine is currently in the middle of a state exit.
        private bool _m_isExiting;
        // Whether the state machine is currently in the middle of a state enter.
        private bool _m_isEntering;
        // The target of this state machine.
        private readonly T_TARGET _m_target;
        // The name of this state machine.
        private readonly string _m_name;


        public StateMachine(T_TARGET _target, string _name)
        {
            _m_target = _target;
            _m_name = _name;
            _m_curState = null;
        }
        public StateMachine(T_TARGET _target) : this(_target, $"StateMachine_{Serialize.NextStateMachine()}")
        {
        }


        /// <summary>
        /// When the state machine's state changes.
        /// </summary>
        public event Action<_AStateBase<T_STATE_TYPE, T_TARGET>, _AStateBase<T_STATE_TYPE, T_TARGET>> OnStateChg;


        /// <summary>
        /// The current state.
        /// </summary>
        public _AStateBase<T_STATE_TYPE, T_TARGET> curState { get { return _m_curState; } }
        /// <summary>
        /// The target of this state machine.
        /// </summary>
        public T_TARGET target { get { return _m_target; } }
        /// <summary>
        /// The name of this state machine.
        /// </summary>
        public string name { get { return _m_name; } }


        /// <summary>
        /// Update the current state.
        /// </summary>
        public void Update(float _deltaTime)
        {
            _m_curState?.Update(_deltaTime);
        }
        /// <summary>
        /// Change state.
        /// </summary>
        public void ChangeState(_AState<T_STATE_TYPE, T_TARGET> _state)
        {
            if (!CanChangeState())
                return;

            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            _m_isExiting = true;
            lastState?.Exit();
            _m_isExiting = false;

            _m_curState = _state;
            _m_isEntering = true;
            _state?.Enter(_m_target, this);
            _m_isEntering = false;

            OnStateChg?.Invoke(lastState, _state);

            // The post-entry hook runs only if this state is still current — an OnStateChg
            // handler may have already transitioned away (and fired its own EnterDone).
            if (ReferenceEquals(_m_curState, _state))
                _state?.EnterDone();
        }
        /// <summary>
        /// Change state with a parameter.
        /// </summary>
        public void ChangeState<T_PARAM_1>(_AState<T_STATE_TYPE, T_TARGET, T_PARAM_1> _state, T_PARAM_1 _param1)
        {
            if (!CanChangeState())
                return;

            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            _m_isExiting = true;
            lastState?.Exit();
            _m_isExiting = false;

            _m_curState = _state;
            _m_isEntering = true;
            _state?.Enter(_m_target, this, _param1);
            _m_isEntering = false;

            OnStateChg?.Invoke(lastState, _state);

            if (ReferenceEquals(_m_curState, _state))
                _state?.EnterDone();
        }
        /// <summary>
        /// Change state with two parameters.
        /// </summary>
        public void ChangeState<T_PARAM_1, T_PARAM_2>(_AState<T_STATE_TYPE, T_TARGET, T_PARAM_1, T_PARAM_2> _state, T_PARAM_1 _param1, T_PARAM_2 _param2)
        {
            if (!CanChangeState())
                return;

            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            _m_isExiting = true;
            lastState?.Exit();
            _m_isExiting = false;

            _m_curState = _state;
            _m_isEntering = true;
            _state?.Enter(_m_target, this, _param1, _param2);
            _m_isEntering = false;

            OnStateChg?.Invoke(lastState, _state);

            if (ReferenceEquals(_m_curState, _state))
                _state?.EnterDone();
        }
        /// <summary>
        /// Change state with three parameters.
        /// </summary>
        public void ChangeState<T_PARAM_1, T_PARAM_2, T_PARAM_3>(_AState<T_STATE_TYPE, T_TARGET, T_PARAM_1, T_PARAM_2, T_PARAM_3> _state, T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3)
        {
            if (!CanChangeState())
                return;

            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            _m_isExiting = true;
            lastState?.Exit();
            _m_isExiting = false;

            _m_curState = _state;
            _m_isEntering = true;
            _state?.Enter(_m_target, this, _param1, _param2, _param3);
            _m_isEntering = false;

            OnStateChg?.Invoke(lastState, _state);

            if (ReferenceEquals(_m_curState, _state))
                _state?.EnterDone();
        }
        /// <summary>
        /// Change state with four parameters.
        /// </summary>
        public void ChangeState<T_PARAM_1, T_PARAM_2, T_PARAM_3, T_PARAM_4>(_AState<T_STATE_TYPE, T_TARGET, T_PARAM_1, T_PARAM_2, T_PARAM_3, T_PARAM_4> _state, T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3, T_PARAM_4 _param4)
        {
            if (!CanChangeState())
                return;

            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            _m_isExiting = true;
            lastState?.Exit();
            _m_isExiting = false;

            _m_curState = _state;
            _m_isEntering = true;
            _state?.Enter(_m_target, this, _param1, _param2, _param3, _param4);
            _m_isEntering = false;

            OnStateChg?.Invoke(lastState, _state);

            if (ReferenceEquals(_m_curState, _state))
                _state?.EnterDone();
        }


        /// <summary>
        /// Whether a state change is allowed right now. Logs an error and returns false when called
        /// during a state's OnExit or OnEnter.
        /// </summary>
        /// <remarks>
        /// <para>Transitions decided as a consequence of entering a state must be made in OnEntered, not OnEnter.</para>
        /// </remarks>
        private bool CanChangeState()
        {
            if (_m_isExiting)
            {
                Console.LogError("StateMachine", _m_name, "Cannot change state during OnExit.");
                return false;
            }
            if (_m_isEntering)
            {
                Console.LogError("StateMachine", _m_name, "Cannot change state during OnEnter. Move the transition to OnEntered.");
                return false;
            }

            return true;
        }
    }
}
