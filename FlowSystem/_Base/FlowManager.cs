// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame.Base
{
    /// <summary>
    /// Internal manager for the Flow system. Manages active stack, suspended stacks, and operation queue.
    /// </summary>
    internal partial class FlowManager
    {
        [NotNull] public static FlowManager instance { get { return _g_instance ??= new FlowManager(); } }
        private static FlowManager _g_instance;


        // The currently active flow stack. Last element is the topmost flow.
        [ItemNotNull, NotNull] private readonly List<_AFlow> _m_activeStack;
        // Stacks suspended by Exclusive flows. Each entry is a snapshot of the active stack at the time of suspension.
        [ItemNotNull, NotNull] private readonly Stack<List<_AFlow>> _m_suspendedStacks;
        // Queued operations waiting for the current transition to complete.
        [ItemNotNull, NotNull] private readonly Queue<_AFlowOperation> _m_operationQueue;
        // Whether a transition is currently in progress (OnEnter async or loadingShow transition).
        private bool _m_isProcessing;


        private FlowManager()
        {
            _m_activeStack = new List<_AFlow>();
            _m_suspendedStacks = new Stack<List<_AFlow>>();
            _m_operationQueue = new Queue<_AFlowOperation>();
            _m_isProcessing = false;
        }


        /// <summary>
        /// The topmost active flow, or null if the active stack is empty.
        /// </summary>
        public _AFlow current
        {
            get { return _m_activeStack.Count > 0 ? _m_activeStack[^1] : null; }
        }


        /// <summary>
        /// Enter a new flow with the specified mode and optional loading show.
        /// </summary>
        public void Enter([NotNull] _AFlow _flow, EFlowMode _mode, _ILoadingShow _loadingShow)
        {
            if (_flow.flowState != EFlowState.None)
            {
                Console.LogError(SystemNames.Flow, _flow.flowName, "Flow has already been entered. Cannot enter again.");
                return;
            }

            if (_m_isProcessing)
            {
                Console.LogVerbose(SystemNames.Flow, _flow.flowName, "Transition in progress. Queuing Enter operation.");
                _m_operationQueue.Enqueue(new EnterOperation(_flow, _mode, _loadingShow));
                return;
            }

            ProcessEnter(_flow, _mode, _loadingShow);
        }
        /// <summary>
        /// Exit the topmost active flow without validation.
        /// </summary>
        public void Exit()
        {
            if (_m_isProcessing)
            {
                Console.LogVerbose(SystemNames.Flow, "Transition in progress. Queuing Exit operation.");
                _m_operationQueue.Enqueue(new ExitOperation());
                return;
            }

            if (!ValidateExit(null, null))
                return;

            ProcessExit();
        }
        /// <summary>
        /// Exit the topmost active flow with name validation.
        /// </summary>
        public void Exit([NotNull] string _name)
        {
            if (_m_isProcessing)
            {
                Console.LogVerbose(SystemNames.Flow, "Transition in progress. Queuing Exit operation.");
                _m_operationQueue.Enqueue(new ExitWithNameOperation(_name));
                return;
            }

            if (!ValidateExit(_name, null))
                return;

            ProcessExit();
        }
        /// <summary>
        /// Exit the topmost active flow with reference validation.
        /// </summary>
        public void Exit([NotNull] _AFlow _flow)
        {
            if (_m_isProcessing)
            {
                Console.LogVerbose(SystemNames.Flow, "Transition in progress. Queuing Exit operation.");
                _m_operationQueue.Enqueue(new ExitWithFlowOperation(_flow));
                return;
            }

            if (!ValidateExit(null, _flow))
                return;

            ProcessExit();
        }


        /// <summary>
        /// Validate that an exit operation is valid. Returns true if exit can proceed.
        /// </summary>
        private bool ValidateExit(string _name, _AFlow _flow)
        {
            if (_m_activeStack.Count == 0)
            {
                Console.LogError(SystemNames.Flow, "Cannot exit: active stack is empty.");
                return false;
            }

            _AFlow top = current;

            if (_name != null && top.flowName != _name)
            {
                Console.LogError(SystemNames.Flow, $"Cannot exit '{_name}': topmost flow is '{top.flowName}'.");
                return false;
            }

            if (_flow != null && top != _flow)
            {
                Console.LogError(SystemNames.Flow, _flow.flowName, $"Cannot exit: flow is not the topmost. Topmost is '{top.flowName}'.");
                return false;
            }

            return true;
        }
        private void ProcessEnter([NotNull] _AFlow _flow, EFlowMode _mode, _ILoadingShow _loadingShow)
        {
            _m_isProcessing = true;
            bool isCovered = _loadingShow != null;

            // The core transition: exit/pause old flows (sync), then enter new flow (async).
            AsyncFunction enterFunction = _complete =>
            {
                ExecuteModeEffect(_mode, isCovered);

                _m_activeStack.Add(_flow);
                _flow.InvokeOnEnter(() =>
                {
                    _flow.SetState(EFlowState.Active);
                    Console.LogVerbose(SystemNames.Flow, _flow.flowName, "Enter complete. Flow is now Active.");
                    _complete();
                });
            };

            Action transitionComplete = () =>
            {
                _m_isProcessing = false;
                ProcessNextOperation();
            };

            if (_loadingShow != null)
                Loading.Load(_loadingShow, enterFunction, transitionComplete);
            else
                enterFunction(transitionComplete);
        }
        private void ExecuteModeEffect(EFlowMode _mode, bool _isCovered)
        {
            switch (_mode)
            {
                case EFlowMode.Overlay:
                    // No effect on existing flows.
                    break;

                case EFlowMode.Exclusive:
                    // Pause all active flows (top to bottom) and suspend the stack.
                    if (_m_activeStack.Count > 0)
                    {
                        for (int i = _m_activeStack.Count - 1; i >= 0; i--)
                            _m_activeStack[i].InvokeOnPause();

                        _m_suspendedStacks.Push(new List<_AFlow>(_m_activeStack));
                        _m_activeStack.Clear();
                    }
                    break;

                case EFlowMode.Replace:
                    // Exit all active flows (top to bottom).
                    for (int i = _m_activeStack.Count - 1; i >= 0; i--)
                        _m_activeStack[i].InvokeOnExit(_isCovered);
                    _m_activeStack.Clear();

                    // Exit all suspended flows.
                    while (_m_suspendedStacks.Count > 0)
                    {
                        List<_AFlow> suspended = _m_suspendedStacks.Pop();
                        for (int i = suspended.Count - 1; i >= 0; i--)
                            suspended[i].InvokeOnExit(_isCovered);
                    }
                    break;
            }
        }
        private void ProcessExit()
        {
            _m_isProcessing = true;

            _AFlow top = _m_activeStack.GetLastAndRemove();
            top.InvokeOnExit(false);

            // If active stack is now empty and there are suspended stacks, restore the most recent one.
            if (_m_activeStack.Count == 0 && _m_suspendedStacks.Count > 0)
            {
                List<_AFlow> restored = _m_suspendedStacks.Pop();
                _m_activeStack.AddRange(restored);

                // Resume all restored flows (bottom to top).
                for (int i = 0; i < _m_activeStack.Count; i++)
                    _m_activeStack[i].InvokeOnResume();
            }

            _m_isProcessing = false;
            ProcessNextOperation();
        }
        private void ProcessNextOperation()
        {
            if (_m_operationQueue.Count == 0)
                return;

            _m_operationQueue.Dequeue().Execute(this);
        }
    }
}
