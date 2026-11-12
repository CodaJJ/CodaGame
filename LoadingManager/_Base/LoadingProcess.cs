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
        [NotNull] private readonly Queue<AsyncFunction> _m_loadFunctions = new Queue<AsyncFunction>();
        private Action _m_loadingShowEndDelegate;
        [NotNull] private readonly StateMachine<ELoadingProcessStep, LoadingProcess> _m_stateMachine;
        private readonly string _m_name;
        private int _m_runningFunctionCount;
        private bool _m_forceHide;


        public LoadingProcess(_ILoadingShow _loadingShow)
            : this(_loadingShow, _loadingShow == null ? "Empty show loading process" : $"{_loadingShow.name} loading process")
        {
        }
        public LoadingProcess(_ILoadingShow _loadingShow, string _name)
        {
            _m_loadingShow = _loadingShow;
            _m_name = _name;
            _m_stateMachine = new StateMachine<ELoadingProcessStep, LoadingProcess>(this, $"{_m_name}'s state machine");
            _m_stateMachine.ChangeState(new IdleState());
        }


        /// <summary>The number of functions waiting to start, excluding running functions.</summary>
        public int loadFunctionCount { get { return _m_loadFunctions.Count; } }
        /// <summary>
        /// The public phase, including Show and Hide transitions. Existing enum values remain unchanged.
        /// </summary>
        public ELoadingProcessStep curState
        {
            get
            {
                if (_m_stateMachine.curTransition is ShowTransition)
                    return ELoadingProcessStep.LoadingShowStart;
                if (_m_stateMachine.curTransition is HideTransition)
                    return ELoadingProcessStep.LoadingShowEnd;
                return _m_stateMachine.curState.type;
            }
        }
        public string name { get { return _m_name; } }


        /// <summary>
        /// Add work to this loading round. Completion waits for all work and for the loading show to hide.
        /// </summary>
        public void AddLoadFunction(AsyncFunction _loadFunction, Action _loadingShowEndCallback)
        {
            if (_loadFunction != null)
                _m_loadFunctions.Enqueue(_loadFunction);
            _m_loadingShowEndDelegate += _loadingShowEndCallback;
            _m_stateMachine.Update();
        }
        /// <summary>
        /// Start hiding without waiting for Show or running load functions.
        /// Queued and running work continues; completion callbacks still wait for work and Hide to finish.
        /// </summary>
        public void ForceToHide()
        {
            Console.LogVerbose(SystemNames.Loading, _m_name, "Forced to hide.");
            _m_forceHide = true;
            _m_stateMachine.Update();
        }


        private void RunPendingFunctions()
        {
            // Only run this batch. Work submitted from a function is handled by its queued Update.
            int count = _m_loadFunctions.Count;
            for (int i = 0; i < count; i++)
            {
                AsyncFunction function = _m_loadFunctions.Dequeue();
                _m_runningFunctionCount++;
                bool completed = false;
                Action complete = () =>
                {
                    if (completed)
                        return;

                    completed = true;
                    _m_runningFunctionCount--;
                    _m_stateMachine.Update();
                };
                function.Invoke(complete);
            }
        }


        #region States
        private class IdleState : _AState<ELoadingProcessStep, LoadingProcess>
        {
            public override ELoadingProcessStep type { get { return ELoadingProcessStep.Idle; } }
            public override string name { get { return "IdleState"; } }


            protected override void OnEnter() { }
            protected override void OnExit() { }
            protected override void OnUpdate()
            {
                if (target._m_loadFunctions.Count > 0)
                {
                    if (target._m_forceHide)
                    {
                        target.RunPendingFunctions();
                        target._m_stateMachine.Update();
                    }
                    else
                    {
                        ChangeState(new LoadingState(), new ShowTransition());
                    }
                    return;
                }

                if (target._m_runningFunctionCount > 0)
                    return;

                target._m_forceHide = false;
                ActionUtility.InvokeAndClear(ref target._m_loadingShowEndDelegate);
            }
        }
        private class LoadingState : _AState<ELoadingProcessStep, LoadingProcess>
        {
            public override ELoadingProcessStep type { get { return ELoadingProcessStep.Loading; } }
            public override string name { get { return "LoadingState"; } }


            protected override void OnEnter() { }
            protected override void OnExit() { }
            protected override void OnUpdate()
            {
                if (target._m_loadFunctions.Count > 0)
                {
                    target.RunPendingFunctions();
                    target._m_stateMachine.Update();
                    return;
                }

                if (target._m_forceHide || target._m_runningFunctionCount == 0)
                    ChangeState(new IdleState(), new HideTransition());
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
            protected override void OnUpdate()
            {
                if (!target._m_forceHide)
                    return;

                target.RunPendingFunctions();
                stateMachine.ChangeState(new IdleState(), new HideTransition(), _interruptTransition: true);
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
            protected override void OnUpdate()
            {
                if (target._m_forceHide && target._m_loadFunctions.Count > 0)
                    target.RunPendingFunctions();
            }
        }
        #endregion
    }
}
