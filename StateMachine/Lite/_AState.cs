// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;

namespace CodaGame.StateMachine.Lite
{
    /// <summary>
    /// A base class for state machine's state.
    /// </summary>
    public abstract class _AStateBase<T_STATE_TYPE> where T_STATE_TYPE : Enum
    {
        // The serialize number of enter.
        private uint _m_enterSerialize;
        // The state machine that this state belongs to.
        private StateMachine<T_STATE_TYPE> _m_stateMachine;


        protected _AStateBase()
        {
            _m_enterSerialize = Serialize.Next();
        }
        
        
        /// <summary>
        /// The serialize number of enter, if the number is the same, it means that it is still in the same enter.
        /// </summary>
        public uint enterSerialize { get { return _m_enterSerialize; } }
        /// <summary>
        /// What state is this state.
        /// </summary>
        public abstract T_STATE_TYPE type { get; }
        /// <summary>
        /// The name of this state.
        /// </summary>
        /// <remarks>
        /// <para>Usually used for debugging tho.</para>
        /// </remarks>
        public abstract string name { get; }
        
        
        /// <summary>
        /// Change state.
        /// </summary>
        protected void ChangeState(_AState<T_STATE_TYPE> _state)
        {
            if (_m_stateMachine == null)
            {
                Console.LogWarning(SystemNames.StateMachine, name, "You are trying to change state in a disabled state.");
                return;
            }
            
            _m_stateMachine.ChangeState(_state);
        }
        /// <summary>
        /// Change state with a parameter.
        /// </summary>
        protected void ChangeState<T_PARAM_1>(_AState<T_STATE_TYPE, T_PARAM_1> _state, T_PARAM_1 _param1)
        {
            if (_m_stateMachine == null)
            {
                Console.LogWarning(SystemNames.StateMachine, name, "You are trying to change state in a disabled state.");
                return;
            }
            
            _m_stateMachine.ChangeState(_state, _param1);
        }
        /// <summary>
        /// Change state with two parameters.
        /// </summary>
        protected void ChangeState<T_PARAM_1, T_PARAM_2>(_AState<T_STATE_TYPE, T_PARAM_1, T_PARAM_2> _state, T_PARAM_1 _param1, T_PARAM_2 _param2)
        {
            if (_m_stateMachine == null)
            {
                Console.LogWarning(SystemNames.StateMachine, name, "You are trying to change state in a disabled state.");
                return;
            }
            
            _m_stateMachine.ChangeState(_state, _param1, _param2);
        }
        /// <summary>
        /// Change state with three parameters.
        /// </summary>
        protected void ChangeState<T_PARAM_1, T_PARAM_2, T_PARAM_3>(_AState<T_STATE_TYPE, T_PARAM_1, T_PARAM_2, T_PARAM_3> _state, T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3)
        {
            if (_m_stateMachine == null)
            {
                Console.LogWarning(SystemNames.StateMachine, name, "You are trying to change state in a disabled state.");
                return;
            }
            
            _m_stateMachine.ChangeState(_state, _param1, _param2, _param3);
        }
        /// <summary>
        /// Change state with four parameters.
        /// </summary>
        protected void ChangeState<T_PARAM_1, T_PARAM_2, T_PARAM_3, T_PARAM_4>(_AState<T_STATE_TYPE, T_PARAM_1, T_PARAM_2, T_PARAM_3, T_PARAM_4> _state, T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3, T_PARAM_4 _param4)
        {
            if (_m_stateMachine == null)
            {
                Console.LogWarning(SystemNames.StateMachine, name, "You are trying to change state in a disabled state.");
                return;
            }
            
            _m_stateMachine.ChangeState(_state, _param1, _param2, _param3, _param4);
        }


        /// <summary>
        /// Update the runtime data.
        /// </summary>
        private protected void UpdateRuntimeData(StateMachine<T_STATE_TYPE> _stateMachine)
        {
            _m_stateMachine = _stateMachine;
        }
        internal void Exit()
        {
            OnExit();
            // Increase the serialize number, for the next enter.
            _m_enterSerialize = Serialize.Next();
            
            // Output log.
            Console.LogVerbose(SystemNames.StateMachine, _m_stateMachine.name, $"state-{name} -- : exits.");
            _m_stateMachine = null;
        }
        internal void Update(float _deltaTime)
        {
            OnUpdate(_deltaTime);
        }

        
        /// <summary>
        /// When exiting this state.
        /// </summary>
        protected abstract void OnExit();
        /// <summary>
        /// When updating this state.
        /// </summary>
        protected abstract void OnUpdate(float _deltaTime);
    }
    /// <summary>
    /// A state for state machine.
    /// </summary>
    public abstract class _AState<T_STATE_TYPE> : _AStateBase<T_STATE_TYPE> where T_STATE_TYPE : Enum
    {
        internal void Enter([NotNull] StateMachine<T_STATE_TYPE> _stateMachine)
        {
            // Output log.
            Console.LogVerbose(SystemNames.StateMachine, _stateMachine.name, $"state-{name} -- : enters.");
            
            UpdateRuntimeData(_stateMachine);
            OnEnter();
        }

        
        /// <summary>
        /// When entering this state.
        /// </summary>
        protected abstract void OnEnter();
    }
    /// <summary>
    /// A state for state machine.
    /// </summary>
    /// <remarks>
    /// <para>The target state's enter has a parameter.</para>
    /// </remarks>
    public abstract class _AState<T_STATE_TYPE, T_PARAM> : _AStateBase<T_STATE_TYPE> where T_STATE_TYPE : Enum
    {
        internal void Enter([NotNull] StateMachine<T_STATE_TYPE> _stateMachine, T_PARAM _param)
        {
            UpdateRuntimeData(_stateMachine);
            
            // Output log.
            Console.LogVerbose(SystemNames.StateMachine, _stateMachine.name, $"state-{name} -- : enters with parameter {_param}.");
            
            OnEnter(_param);
        }

        
        /// <summary>
        /// When entering this state.
        /// </summary>
        protected abstract void OnEnter(T_PARAM _param);
    }
    /// <summary>
    /// A state for state machine.
    /// </summary>
    /// <remarks>
    /// <para>The target state's enter has two parameters.</para>
    /// </remarks>
    public abstract class _AState<T_STATE_TYPE, T_PARAM_1, T_PARAM_2> : _AStateBase<T_STATE_TYPE> where T_STATE_TYPE : Enum
    {
        internal void Enter([NotNull] StateMachine<T_STATE_TYPE> _stateMachine, T_PARAM_1 _param1, T_PARAM_2 _param2)
        {
            UpdateRuntimeData(_stateMachine);
            
            // Output log.
            Console.LogVerbose(SystemNames.StateMachine, _stateMachine.name, $"state-{name} -- : enters with parameters {_param1}, {_param2}.");
            
            OnEnter(_param1, _param2);
        }

        
        /// <summary>
        /// When entering this state.
        /// </summary>
        protected abstract void OnEnter(T_PARAM_1 _param1, T_PARAM_2 _param2);
    }
    /// <summary>
    /// A state for state machine.
    /// </summary>
    /// <remarks>
    /// <para>The target state's enter has three parameters.</para>
    /// </remarks>
    public abstract class _AState<T_STATE_TYPE, T_PARAM_1, T_PARAM_2, T_PARAM_3> : _AStateBase<T_STATE_TYPE> where T_STATE_TYPE : Enum
    {
        internal void Enter([NotNull] StateMachine<T_STATE_TYPE> _stateMachine, T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3)
        {
            UpdateRuntimeData(_stateMachine);
            
            // Output log.
            Console.LogVerbose(SystemNames.StateMachine, _stateMachine.name, $"state-{name} -- : enters with parameters {_param1}, {_param2}, {_param3}.");
            
            OnEnter(_param1, _param2, _param3);
        }

        
        /// <summary>
        /// When entering this state.
        /// </summary>
        protected abstract void OnEnter(T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3);
    }
    /// <summary>
    /// A state for state machine.
    /// </summary>
    /// <remarks>
    /// <para>The target state's enter has four parameters.</para>
    /// </remarks>
    public abstract class _AState<T_STATE_TYPE, T_PARAM_1, T_PARAM_2, T_PARAM_3, T_PARAM_4> : _AStateBase<T_STATE_TYPE> where T_STATE_TYPE : Enum
    {
        internal void Enter([NotNull] StateMachine<T_STATE_TYPE> _stateMachine, T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3, T_PARAM_4 _param4)
        {
            UpdateRuntimeData(_stateMachine);
            
            // Output log.
            Console.LogVerbose(SystemNames.StateMachine, _stateMachine.name, $"state-{name} -- : enters with parameters {_param1}, {_param2}, {_param3}, {_param4}.");

            OnEnter(_param1, _param2, _param3, _param4);
        }

        
        /// <summary>
        /// When entering this state.
        /// </summary>
        protected abstract void OnEnter(T_PARAM_1 _param1, T_PARAM_2 _param2, T_PARAM_3 _param3, T_PARAM_4 _param4);
    }
}