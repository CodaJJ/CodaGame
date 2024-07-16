
using System;

namespace UnityGameFramework.StateMachine.Lite
{
    /// <summary>
    /// A base class for state machine's state.
    /// </summary>
    public abstract class _AStateBase<T> where T : Enum
    {
        // The serialize number of enter.
        private uint _m_enterSerialize;
        
        
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
        public abstract T state { get; }

        
        internal void Exit()
        {
            OnExit();
            // Increase the serialize number, for the next enter.
            _m_enterSerialize = Serialize.Next();
        }

        
        /// <summary>
        /// When exiting this state.
        /// </summary>
        protected abstract void OnExit();
    }
    /// <summary>
    /// A state for state machine.
    /// </summary>
    public abstract class _AState<T> : _AStateBase<T> where T : Enum
    {
        internal void Enter()
        {
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
    /// The target state's enter has a parameter.
    /// </remarks>
    public abstract class _AState<T, PARAM> : _AStateBase<T> where T : Enum
    {
        internal void Enter(PARAM _param)
        {
            OnEnter(_param);
        }

        
        /// <summary>
        /// When entering this state.
        /// </summary>
        protected abstract void OnEnter(PARAM _param);
    }
    /// <summary>
    /// A state for state machine.
    /// </summary>
    /// <remarks>
    /// The target state's enter has two parameters.
    /// </remarks>
    public abstract class _AState<T, PARAM_1, PARAM_2> : _AStateBase<T> where T : Enum
    {
        internal void Enter(PARAM_1 _param1, PARAM_2 _param2)
        {
            OnEnter(_param1, _param2);
        }

        
        /// <summary>
        /// When entering this state.
        /// </summary>
        protected abstract void OnEnter(PARAM_1 _param1, PARAM_2 _param2);
    }
    /// <summary>
    /// A state for state machine.
    /// </summary>
    /// <remarks>
    /// The target state's enter has three parameters.
    /// </remarks>
    public abstract class _AState<T, PARAM_1, PARAM_2, PARAM_3> : _AStateBase<T> where T : Enum
    {
        internal void Enter(PARAM_1 _param1, PARAM_2 _param2, PARAM_3 _param3)
        {
            OnEnter(_param1, _param2, _param3);
        }

        
        /// <summary>
        /// When entering this state.
        /// </summary>
        protected abstract void OnEnter(PARAM_1 _param1, PARAM_2 _param2, PARAM_3 _param3);
    }
    /// <summary>
    /// A state for state machine.
    /// </summary>
    /// <remarks>
    /// The target state's enter has four parameters.
    /// </remarks>
    public abstract class _AState<T, PARAM_1, PARAM_2, PARAM_3, PARAM_4> : _AStateBase<T> where T : Enum
    {
        internal void Enter(PARAM_1 _param1, PARAM_2 _param2, PARAM_3 _param3, PARAM_4 _param4)
        {
            OnEnter(_param1, _param2, _param3, _param4);
        }

        
        /// <summary>
        /// When entering this state.
        /// </summary>
        protected abstract void OnEnter(PARAM_1 _param1, PARAM_2 _param2, PARAM_3 _param3, PARAM_4 _param4);
    }
}