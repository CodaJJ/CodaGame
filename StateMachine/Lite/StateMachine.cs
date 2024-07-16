
using System;

namespace UnityGameFramework.StateMachine.Lite
{
    /// <summary>
    /// A state machine lite that can change state.
    /// </summary>
    /// <remarks>
    /// This machine only handles the state change, invoking the enter and exit of the state.
    /// </remarks>
    public class StateMachine<T> where T : Enum
    {
        // The current state.
        private _AStateBase<T> _m_curState;
        

        public StateMachine()
        {
            _m_curState = null;
        }
        
        
        /// <summary>
        /// When the state machine's state changes.
        /// </summary>
        public event Action<_AStateBase<T>, _AStateBase<T>> OnStateChg;
        

        /// <summary>
        /// The current state.
        /// </summary>
        public _AStateBase<T> curState { get { return _m_curState; } }

        
        /// <summary>
        /// Change state.
        /// </summary>
        public void ChangeState(_AState<T> _state)
        {
            _AStateBase<T> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter();
            
            OnStateChg?.Invoke(lastState, _state);
        }
        /// <summary>
        /// Change state with a parameter.
        /// </summary>
        public void ChangeState<PARAM_1>(_AState<T, PARAM_1> _state, PARAM_1 _param1)
        {
            _AStateBase<T> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter(_param1);
            
            OnStateChg?.Invoke(lastState, _state);
        }
        /// <summary>
        /// Change state with two parameters.
        /// </summary>
        public void ChangeState<PARAM_1, PARAM_2>(_AState<T, PARAM_1, PARAM_2> _state, PARAM_1 _param1, PARAM_2 _param2)
        {
            _AStateBase<T> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter(_param1, _param2);
            
            OnStateChg?.Invoke(lastState, _state);
        }
        /// <summary>
        /// Change state with three parameters.
        /// </summary>
        public void ChangeState<PARAM_1, PARAM_2, PARAM_3>(_AState<T, PARAM_1, PARAM_2, PARAM_3> _state, PARAM_1 _param1, PARAM_2 _param2, PARAM_3 _param3)
        {
            _AStateBase<T> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter(_param1, _param2, _param3);
            
            OnStateChg?.Invoke(lastState, _state);
        }
        /// <summary>
        /// Change state with four parameters.
        /// </summary>
        public void ChangeState<PARAM_1, PARAM_2, PARAM_3, PARAM_4>(_AState<T, PARAM_1, PARAM_2, PARAM_3, PARAM_4> _state, PARAM_1 _param1, PARAM_2 _param2, PARAM_3 _param3, PARAM_4 _param4)
        {
            _AStateBase<T> lastState = _m_curState;
            lastState?.Exit();
            
            _m_curState = _state;
            _state?.Enter(_param1, _param2, _param3, _param4);
            
            OnStateChg?.Invoke(lastState, _state);
        }
    }
}