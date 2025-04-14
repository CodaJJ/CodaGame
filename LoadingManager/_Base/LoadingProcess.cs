// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using CodaGame.StateMachine.TargetedLite;
using JetBrains.Annotations;

namespace CodaGame.Base
{
    /// <summary>
    /// A class that manages the loading process.
    /// </summary>
    public class LoadingProcess
    {
        // The loading show interface.
        private readonly _ILoadingShow _m_loadingShow;
        // The list of load functions, they must be done to complete the loading process.
        [ItemNotNull, NotNull] private readonly List<AsyncFunction> _m_loadFunctions;
        // The delegate when loading show ends.
        private Action _m_loadingShowEndDelegate;
        // A state machine to manage the loading process.
        [NotNull] private readonly StateMachine<ELoadingProcessStep, LoadingProcess> _m_stateMachine;
        // The name of this process.
        private readonly string _m_name;
        // Is the loading process forced to hide.
        private bool _m_forceHide;


        public LoadingProcess(_ILoadingShow _loadingShow) 
            : this(_loadingShow, _loadingShow == null ? "Empty show loading process" : $"{_loadingShow.name} loading process")
        {
        }
        public LoadingProcess(_ILoadingShow _loadingShow, string _name)
        {
            _m_loadingShow = _loadingShow;
            _m_name = _name;
            
            _m_loadFunctions = new List<AsyncFunction>();
            _m_loadingShowEndDelegate = null;
            _m_stateMachine = new StateMachine<ELoadingProcessStep, LoadingProcess>(this, $"{_m_name}'s state machine");
            _m_stateMachine.ChangeState(new IdleState());
            _m_forceHide = false;
        }
        
            
        /// <summary>
        /// How many load functions are there in this loading process.
        /// </summary>
        public int loadFunctionCount { get { return _m_loadFunctions.Count; } }
        /// <summary>
        /// The current state of the loading process.
        /// </summary>
        public ELoadingProcessStep curState { get { return _m_stateMachine.curState.type; } }
        /// <summary>
        /// The name of the loading process.
        /// </summary>
        public string name { get { return _m_name; } }


        /// <summary>
        /// Add a load function to the loading process.
        /// </summary>
        /// <remarks>
        /// <para>The load function will run immediately after loading show shows.</para>
        /// </remarks>
        /// <param name="_loadFunction">The load function you want to run.</param>
        /// <param name="_loadingShowEndCallback">The callback when loading show ends.</param>
        public void AddLoadFunction(AsyncFunction _loadFunction, Action _loadingShowEndCallback)
        {
            if (_loadFunction != null)
                _m_loadFunctions.Add(_loadFunction);
            _m_loadingShowEndDelegate += _loadingShowEndCallback;
            
            _m_stateMachine.Update();
        }
        /// <summary>
        /// Force to hide the loading show.
        /// </summary>
        /// <remarks>
        /// <para>Make the loading show hide immediately, no matter what the loading process is.</para>
        /// <para>The remaining load functions will be invoked, and loadingShowEndCallback will be invoked after all load functions done.</para>
        /// </remarks>
        public void ForceToHide()
        {
            Console.LogVerbose(SystemNames.Loading, _m_name, "Forced to hide.");
            
            _m_forceHide = true;
            _m_stateMachine.Update();
        }


        #region State
        // Idle state of the loading process. Waiting for load functions.
        private class IdleState : _AState<ELoadingProcessStep, LoadingProcess>
        {
            public override ELoadingProcessStep type { get { return ELoadingProcessStep.Idle; } }
            public override string name { get { return "IdleState"; } }


            protected override void OnEnter()
            {
            }
            protected override void OnExit()
            {
            }
            protected override void OnUpdate()
            {
                if (target == null)
                {
                    Console.LogError(SystemNames.Loading, "The state machine is broken. Make sure to invoke the Loading module on the main thread.");
                    return;
                }

                if (target._m_forceHide)
                {
                    target._m_forceHide = false;
                    List<AsyncFunction> loadFunctions = new List<AsyncFunction>(target._m_loadFunctions);
                    target._m_loadFunctions.Clear();
                    Action showEndDelegate = target._m_loadingShowEndDelegate;
                    target._m_loadingShowEndDelegate = null;
                
                    Async.Parallel(loadFunctions, showEndDelegate, "LoadingProcess");
                }
                
                if (target._m_loadFunctions.Count > 0)
                    ChangeState(new LoadingShowStartState());
                else
                {
                    Action showEndDelegate = target._m_loadingShowEndDelegate;
                    target._m_loadingShowEndDelegate = null;
                    showEndDelegate?.Invoke();
                }
            }
        }
        // Loading show start state of the loading process. The load functions will be started after the loading show done.
        private class LoadingShowStartState : _AState<ELoadingProcessStep, LoadingProcess>
        {
            public override ELoadingProcessStep type { get { return ELoadingProcessStep.LoadingShowStart; } }
            public override string name { get { return "LoadingShowStartState"; } }


            protected override void OnEnter()
            {
                if (target == null)
                {
                    Console.LogError(SystemNames.Loading, "The state machine is broken. Make sure to invoke the Loading module on the main thread.");    
                    return;
                }
                
                if (target._m_loadingShow == null)
                {
                    ChangeState(new LoadingState());
                    return;
                }

                uint serialize = enterSerialize;
                target._m_loadingShow.Show(() =>
                {
                    if (serialize != enterSerialize)
                        return;
                    
                    ChangeState(new LoadingState());
                });
            }
            protected override void OnExit()
            {
            }
            protected override void OnUpdate()
            {
            }
        }
        // Loading state of the loading process. Running load functions, if there are new load functions, they will be added to the loading process.
        private class LoadingState : _AState<ELoadingProcessStep, LoadingProcess>
        {
            private Parallel _m_parallelOperation;
            
            
            public override ELoadingProcessStep type { get { return ELoadingProcessStep.Loading; } }
            public override string name { get { return "LoadingState"; } }
            

            protected override void OnEnter()
            {
                if (target == null)
                {
                    Console.LogError(SystemNames.Loading, "The state machine is broken. Make sure to invoke the Loading module on the main thread.");
                    return;
                }
                
                if (target._m_loadFunctions.Count == 0)
                {
                    ChangeState(new LoadingShowEndState());
                    return;
                }
                
                List<AsyncFunction> loadFunctions = new List<AsyncFunction>(target._m_loadFunctions);
                target._m_loadFunctions.Clear();
                
                _m_parallelOperation = new Parallel("LoadingProcess");
                foreach (AsyncFunction loadFunction in loadFunctions)
                    _m_parallelOperation.RunFunction(loadFunction);
                
                uint serialize = enterSerialize;
                _m_parallelOperation.AddCompleteCallback(() =>
                {
                    if (serialize != enterSerialize)
                        return;
                    
                    ChangeState(new LoadingShowEndState());
                });
            }
            protected override void OnExit()
            {
                _m_parallelOperation = null;
            }
            protected override void OnUpdate()
            {
                if (target == null || _m_parallelOperation == null)
                {
                    Console.LogError(SystemNames.Loading, "The state machine is broken. Make sure to invoke the Loading module on the main thread.");
                    return;
                }
                
                if (target._m_loadFunctions.Count == 0)
                    return;
                
                List<AsyncFunction> loadFunctions = new List<AsyncFunction>(target._m_loadFunctions);
                target._m_loadFunctions.Clear();
                foreach (AsyncFunction loadFunction in loadFunctions)
                    _m_parallelOperation.RunFunction(loadFunction);

                if (target._m_forceHide)
                {
                    target._m_forceHide = false;
                    ChangeState(new LoadingShowEndState());
                }
            }
        }
        // Loading show end state of the loading process. The loading show will be hidden after the load functions done.
        private class LoadingShowEndState : _AState<ELoadingProcessStep, LoadingProcess>
        {
            public override ELoadingProcessStep type { get { return ELoadingProcessStep.LoadingShowEnd; } }
            public override string name { get { return "LoadingShowEndState"; } }
            

            protected override void OnEnter()
            {
                if (target == null)
                {
                    Console.LogError(SystemNames.Loading, "The state machine is broken. Make sure to invoke the Loading module on the main thread.");
                    return;
                }
                
                if (target._m_loadingShow == null)
                {
                    ChangeState(new IdleState());
                    return;
                }
                
                uint serialize = enterSerialize;
                target._m_loadingShow.Hide(() =>
                {
                    if (serialize != enterSerialize)
                        return;
                    
                    ChangeState(new IdleState());
                });
            }
            protected override void OnExit()
            {
            }
            protected override void OnUpdate()
            {
            }
        }
        #endregion
    }
}