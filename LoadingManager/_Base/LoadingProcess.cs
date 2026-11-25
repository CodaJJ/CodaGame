// Copyright (c) 2024 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using CodaGame.StateMachine;
using JetBrains.Annotations;

namespace CodaGame.Base
{
    /// <summary>
    /// Runs load functions while showing a loading screen. Show and Hide are explicit transitions.
    /// </summary>
    public class LoadingProcess
    {
        private readonly _ILoadingShow _m_loadingShow;
        [NotNull] private readonly Queue<AsyncFunction> _m_loadFunctions;
        private Action _m_loadingShowEndDelegate;
        [NotNull] private readonly StateMachine<ELoadingProcessStep, LoadingProcess> _m_stateMachine;
        private readonly string _m_name;


        public LoadingProcess(_ILoadingShow _loadingShow)
            : this(_loadingShow, _loadingShow == null ? "Empty show loading process" : $"{_loadingShow.name} loading process")
        {
        }
        public LoadingProcess(_ILoadingShow _loadingShow, string _name)
        {
            _m_loadingShow = _loadingShow;
            _m_loadFunctions = new Queue<AsyncFunction>();
            _m_name = _name;
            _m_stateMachine = new StateMachine<ELoadingProcessStep, LoadingProcess>(this, $"{_m_name}'s state machine");
            _m_stateMachine.ChangeState(new IdleState());
        }


        /// <summary>
        /// The number of functions waiting to start, excluding running functions.
        /// </summary>
        public int loadFunctionCount { get { return _m_loadFunctions.Count; } }
        /// <summary>
        /// The destination of the executing clip, including while its transition is running.
        /// </summary>
        public ELoadingProcessStep curState { get { return _m_stateMachine.curState.type; } }
        public string name { get { return _m_name; } }


        /// <summary>
        /// Submit work and optionally register a callback for the next stable Idle.
        /// </summary>
        public void AddLoadFunction(AsyncFunction _loadFunction, Action _loadingShowEndCallback = null)
        {
            if (_loadFunction != null)
                _m_loadFunctions.Enqueue(_loadFunction);
            _m_loadingShowEndDelegate += _loadingShowEndCallback;
            _m_stateMachine.Update();
        }
        /// <summary>
        /// Invoke the callback once when Idle has no queued work. It may run before this method returns.
        /// </summary>
        public void AddShowEndCallback(Action _loadingShowEndCallback)
        {
            _m_loadingShowEndDelegate += _loadingShowEndCallback;
            _m_stateMachine.Update();
        }


        #region States
        private class IdleState : _AState<ELoadingProcessStep, LoadingProcess>
        {
            public override ELoadingProcessStep type { get { return ELoadingProcessStep.Idle; } }
            public override string name { get { return "IdleState"; } }


            protected override void OnEnter() { stateMachine.Update(); }
            protected override void OnExit() { }
            protected override void OnUpdate()
            {
                if (target._m_loadFunctions.Count > 0)
                    ChangeState(new LoadingState(), new ShowTransition());
                else
                    ActionUtility.InvokeAndClear(ref target._m_loadingShowEndDelegate);
            }
        }
        private class LoadingState : _AState<ELoadingProcessStep, LoadingProcess>
        {
            private int _m_runningFunctionCount;


            public override ELoadingProcessStep type { get { return ELoadingProcessStep.Loading; } }
            public override string name { get { return "LoadingState"; } }


            protected override void OnEnter() { stateMachine.Update(); }
            protected override void OnExit() { }
            protected override void OnUpdate()
            {
                int count = target.loadFunctionCount;
                if (count == 0 && _m_runningFunctionCount == 0)
                {
                    ChangeState(new IdleState(), new HideTransition());
                    return;
                }

                // AddLoadFunction requests another update for work appended by this batch.
                for (int i = 0; i < count; i++)
                    StartFunction(target._m_loadFunctions.Dequeue());
            }


            private void StartFunction(AsyncFunction _function)
            {
                _m_runningFunctionCount++;
                bool completed = false;
                _function(() =>
                {
                    if (completed)
                        return;

                    completed = true;
                    _m_runningFunctionCount--;
                    stateMachine.Update();
                });
            }
        }
        #endregion


        #region Transitions
        private class ShowTransition : _ATransition<ELoadingProcessStep, LoadingProcess>
        {
            protected override void OnEnter(Action _complete)
            {
                if (target._m_loadingShow == null)
                    _complete.Invoke();
                else
                    target._m_loadingShow.Show(_complete);
            }
        }
        private class HideTransition : _ATransition<ELoadingProcessStep, LoadingProcess>
        {
            protected override void OnEnter(Action _complete)
            {
                if (target._m_loadingShow == null)
                    _complete.Invoke();
                else
                    target._m_loadingShow.Hide(_complete);
            }
        }
        #endregion
    }
}
