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
        public void Update()
        {
            _m_curState?.Update();
        }
        /// <summary>
        /// Change state.
        /// </summary>
        public void ChangeState(_AState<T_STATE_TYPE, T_TARGET> _state)
        {
            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter(_m_target, this);
            
            OnStateChg?.Invoke(lastState, _state);
        }
        /// <summary>
        /// Change state with a parameter.
        /// </summary>
        public void ChangeState<T_PARAM_1>(_AState<T_STATE_TYPE, T_TARGET, T_PARAM_1> _state, T_PARAM_1 _param1)
        {
            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter(_m_target, this, _param1);
            
            OnStateChg?.Invoke(lastState, _state);
        }
        /// <summary>
        /// Change state with two parameters.
        /// </summary>
        public void ChangeState<T_PARAM_1, T_PARAM_2>(_AState<T_STATE_TYPE, T_TARGET, T_PARAM_1, T_PARAM_2> _state, T_PARAM_1 _param1, T_PARAM_2 _param2)
        {
            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter(_m_target, this, _param1, _param2);
            
            OnStateChg?.Invoke(lastState, _state);
        }
        /// <summary>
        /// Change state with three parameters.
        /// </summary>
        public void ChangeState<T_PARAM_1, T_PARAM_2, T_PARAM_3>(_AState<T_STATE_TYPE, T_TARGET, T_PARAM_1, T_PARAM_2, T_PARAM_3> _state, T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3)
        {
            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter(_m_target, this, _param1, _param2, _param3);
            
            OnStateChg?.Invoke(lastState, _state);
        }
        /// <summary>
        /// Change state with four parameters.
        /// </summary>
        public void ChangeState<T_PARAM_1, T_PARAM_2, T_PARAM_3, T_PARAM_4>(_AState<T_STATE_TYPE, T_TARGET, T_PARAM_1, T_PARAM_2, T_PARAM_3, T_PARAM_4> _state, T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3, T_PARAM_4 _param4)
        {
            _AStateBase<T_STATE_TYPE, T_TARGET> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter(_m_target, this, _param1, _param2, _param3, _param4);
            
            OnStateChg?.Invoke(lastState, _state);
        }
    }
}