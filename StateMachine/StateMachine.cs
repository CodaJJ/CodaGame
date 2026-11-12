// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace CodaGame.StateMachine
{
    /// <summary>A single-threaded state machine with a required owner and optional asynchronous transitions.</summary>
    /// <remarks>
    /// Hooks and notifications never interrupt each other. Nested requests are queued and executable
    /// operations drain before the outermost call returns, without waiting for asynchronous completion.
    /// Every change is retained, including same-instance reentry. Only adjacent pending normal updates merge.
    /// Exceptions propagate without rollback, queue clearing or recovery. Finally releases the processing
    /// guard, but a failed machine may be partially executed and must not be assumed safe to continue.
    /// </remarks>
    public class StateMachine<T_STATE_TYPE, T_TARGET> where T_STATE_TYPE : Enum where T_TARGET : class
    {
        private static string Describe(_AState<T_STATE_TYPE, T_TARGET> _state)
        {
            return _state == null ? "null" : $"{_state.name} [{_state.type}]";
        }


        [NotNull] private readonly T_TARGET _m_target;
        [NotNull] private readonly string _m_name;
        [NotNull] private readonly Queue<_AOperation> _m_pendingOperations = new Queue<_AOperation>();
        [NotNull] private readonly Queue<ChangeOperation> _m_waitingChanges = new Queue<ChangeOperation>();
        private _AState<T_STATE_TYPE, T_TARGET> _m_curState;
        private UpdateOperation _m_lastPendingUpdate;
        private TransitionRun _m_transitionRun;
        private bool _m_isProcessing;
        private long _m_nextOperationId;
        private long _m_lastOperationId;
        private _AOperation _m_lastOperation;
        private bool _m_lastUpdateForced;
        private bool _m_lastOperationCompleted;
        private ExecutionStage _m_lastStage;
        private _AState<T_STATE_TYPE, T_TARGET> _m_executingFrom;
        private _AState<T_STATE_TYPE, T_TARGET> _m_executingTo;


        public StateMachine([NotNull] T_TARGET _target, [NotNull] string _name)
        {
            _m_target = _target ?? throw new ArgumentNullException(nameof(_target));
            _m_name = _name ?? throw new ArgumentNullException(nameof(_name));
        }
        public StateMachine([NotNull] T_TARGET _target) : this(_target, $"StateMachine_{Serialize.NextStateMachine()}") { }


        /// <summary>
        /// A completed change: invoked after the destination's OnEnter returns, or after exiting to null.
        /// A → transition → B emits only (A, B). Interrupting into C emits (A, C); B never enters.
        /// Reentry emits (A, A). Listener requests wait until the entire notification returns.
        /// </summary>
        public event Action<_AState<T_STATE_TYPE, T_TARGET>, _AState<T_STATE_TYPE, T_TARGET>> OnStateChg;


        /// <summary>
        /// A during A.OnExit; null throughout a transition; B before B.OnEnter is invoked.
        /// Also null before initialization and after an explicit change to null.
        /// </summary>
        public _AState<T_STATE_TYPE, T_TARGET> curState { get { return _m_curState; } }
        public _ATransition<T_STATE_TYPE, T_TARGET> curTransition { get { return _m_transitionRun?.change.transition; } }
        /// <summary>The original exited source of the active transition, including after interruptions.</summary>
        public _AState<T_STATE_TYPE, T_TARGET> fromState { get { return _m_transitionRun?.from; } }
        public _AState<T_STATE_TYPE, T_TARGET> nextState { get { return _m_transitionRun?.change.state; } }
        public bool isTransitioning { get { return _m_transitionRun != null; } }
        [NotNull] public T_TARGET target { get { return _m_target; } }
        [NotNull] public string name { get { return _m_name; } }
        /// <summary>Opt in to editor logs for update requests, merges and execution; off by default.</summary>
        public bool traceUpdates { get; set; }


        /// <summary>Update the active state or transition. Business code owns timing and update behavior.</summary>
        /// <param name="_force">Keep this request separate from adjacent pending updates.</param>
        public void Update(bool _force = false)
        {
            if (!_m_isProcessing)
            {
                // The ordinary external update path does not allocate an operation or delegate.
                ProcessPendingOperations(true, _force);
                return;
            }

            if (!_force && _m_lastPendingUpdate != null)
            {
                if (traceUpdates)
                    Console.LogVerbose(SystemNames.StateMachine, name, $"Update merged into #{_m_lastPendingUpdate.id}.");
                return;
            }

            UpdateOperation operation = new UpdateOperation(++_m_nextOperationId, _force);
            _m_pendingOperations.Enqueue(operation);
            _m_lastPendingUpdate = _force ? null : operation;
            if (traceUpdates)
                Console.LogVerbose(SystemNames.StateMachine, name, $"Queued {operation}.");
        }
        /// <summary>
        /// Request a change. Ordinary requests wait behind an active transition. An interrupt skips that
        /// transition's destination, calls OnExit(false), and retains all other waiting changes.
        /// Changing to null clears the active state; it does not cancel queued work or shut down the machine.
        /// </summary>
        public void ChangeState(_AState<T_STATE_TYPE, T_TARGET> _state,
            _ATransition<T_STATE_TYPE, T_TARGET> _transition = null, bool _interruptTransition = false)
        {
            ChangeOperation operation = new ChangeOperation(++_m_nextOperationId, _state, _transition, _interruptTransition);
            // A completion already queued by this hook must not commit ahead of its interrupt request.
            if (_interruptTransition && _m_transitionRun != null)
                _m_transitionRun.interruptRequested = true;

            _m_pendingOperations.Enqueue(operation);
            _m_lastPendingUpdate = null;
            Console.LogVerbose(SystemNames.StateMachine, name, $"Queued {operation}.");
            ProcessPendingOperations();
        }
        /// <summary>
        /// Build an on-demand diagnostic snapshot, including every pending operation in execution order.
        /// After an exception the last operation/stage is left intact; completed=False is diagnostic only,
        /// not a recovery or fault-state mechanism. Call before retrying to preserve the failure location.
        /// </summary>
        [NotNull] public string GetDebugInfo()
        {
            StringBuilder text = new StringBuilder();
            text.AppendLine($"StateMachine: {name}");
            text.AppendLine($"Current: {Describe(_m_curState)}; Processing: {_m_isProcessing}");
            TransitionRun run = _m_transitionRun;
            text.AppendLine(run == null ? "Transition: null" :
                $"Transition: {run.change.transition.name} (#{run.change.id}); From: {Describe(run.from)}; To: {Describe(run.change.state)}; CompletionRequested: {run.completionRequested}; InterruptRequested: {run.interruptRequested}");
            string operation = _m_lastOperation?.ToString() ?? $"#{_m_lastOperationId} Update(force={_m_lastUpdateForced})";
            text.AppendLine($"Last operation: {(_m_lastOperationId == 0 ? "none" : operation)}; Stage: {_m_lastStage}; Completed: {_m_lastOperationCompleted}");
            text.AppendLine($"Executing change: {Describe(_m_executingFrom)} -> {Describe(_m_executingTo)}");
            text.AppendLine($"Pending operations ({_m_pendingOperations.Count}):");
            foreach (_AOperation pending in _m_pendingOperations)
                text.AppendLine($"  {pending}");
            text.AppendLine($"Waiting changes ({_m_waitingChanges.Count}):");
            foreach (ChangeOperation waiting in _m_waitingChanges)
                text.AppendLine($"  {waiting}");
            return text.ToString();
        }


        private void ProcessPendingOperations(bool _initialUpdate = false, bool _force = false)
        {
            if (_m_isProcessing)
                return;

            _m_isProcessing = true;
            try
            {
                if (_initialUpdate)
                {
                    BeginOperation(null, ++_m_nextOperationId, _force);
                    ExecuteUpdate();
                    _m_lastOperationCompleted = true;
                }

                while (true)
                {
                    if (_m_pendingOperations.Count > 0)
                    {
                        _AOperation operation = _m_pendingOperations.Dequeue();
                        if (ReferenceEquals(operation, _m_lastPendingUpdate))
                            _m_lastPendingUpdate = null;
                        BeginOperation(operation, operation.id);
                        if (operation is ChangeOperation change)
                            RequestChange(change);
                        else
                            ExecuteUpdate();
                    }
                    else if (_m_transitionRun == null && _m_waitingChanges.Count > 0)
                    {
                        ChangeOperation change = _m_waitingChanges.Dequeue();
                        BeginOperation(change, change.id);
                        BeginChange(change, _m_curState);
                    }
                    else
                        break;

                    _m_lastOperationCompleted = true;
                }
            }
            finally
            {
                _m_isProcessing = false;
            }
        }
        private void BeginOperation(_AOperation _operation, long _id, bool _force = false)
        {
            _m_lastOperation = _operation;
            _m_lastOperationId = _id;
            _m_lastUpdateForced = _force;
            _m_lastOperationCompleted = false;
            _m_lastStage = ExecutionStage.Dispatch;
            _m_executingFrom = null;
            _m_executingTo = null;
        }
        private void ExecuteUpdate()
        {
            if (traceUpdates)
                Console.LogVerbose(SystemNames.StateMachine, name, $"Executing Update #{_m_lastOperationId}.");
            if (_m_transitionRun != null)
            {
                TransitionRun run = _m_transitionRun;
                if (run.interruptRequested)
                    return;

                _m_lastStage = ExecutionStage.TransitionUpdate;
                run.change.transition.Update();
                if (run.completionRequested && !run.interruptRequested)
                    FinishTransition(run);
            }
            else
            {
                _m_lastStage = ExecutionStage.StateUpdate;
                _m_curState?.Update();
            }
        }
        private void RequestChange([NotNull] ChangeOperation _change)
        {
            if (_change.interruptTransition && _m_transitionRun != null)
            {
                TransitionRun run = _m_transitionRun;
                _change.state?.ValidateEntry(this);
                if (!ReferenceEquals(_change.transition, run.change.transition))
                    _change.transition?.ValidateEntry();
                _m_executingFrom = run.from;
                _m_executingTo = _change.state;
                _m_lastStage = ExecutionStage.TransitionExitInterrupted;
                Console.LogVerbose(SystemNames.StateMachine, name, $"Interrupting transition #{run.change.id} with #{_change.id}.");
                run.change.transition.Exit(false);
                _m_transitionRun = null;
                BeginChange(_change, run.from, true);
                return;
            }

            if (_m_transitionRun != null || _m_waitingChanges.Count > 0)
            {
                _m_waitingChanges.Enqueue(_change);
                Console.LogVerbose(SystemNames.StateMachine, name, $"Waiting {_change}.");
            }
            else
                BeginChange(_change, _m_curState);
        }
        private void BeginChange([NotNull] ChangeOperation _change, _AState<T_STATE_TYPE, T_TARGET> _from, bool _sourceExited = false)
        {
            _m_executingFrom = _from;
            _m_executingTo = _change.state;
            _change.state?.ValidateEntry(this);
            // Reserve the transition before source Exit, but expose it only after the source has exited.
            _change.transition?.Bind(this, _from, _change.state);
            bool exitReturned = false;
            try
            {
                if (!_sourceExited)
                {
                    _m_lastStage = ExecutionStage.StateExit;
                    Console.LogVerbose(SystemNames.StateMachine, name, $"#{_change.id} Exiting {Describe(_from)}.");
                    _from?.Exit();
                }
                exitReturned = true;
            }
            finally
            {
                if (!exitReturned)
                    _change.transition?.Unbind();
            }

            _m_curState = null;
            if (_change.transition == null)
            {
                EnterDestination(_from, _change);
                return;
            }

            TransitionRun run = new TransitionRun(_from, _change);
            _m_transitionRun = run;
            _m_lastStage = ExecutionStage.TransitionEnter;
            Console.LogVerbose(SystemNames.StateMachine, name, $"Starting transition #{_change.id}: {Describe(_from)} -> {_change.transition.name} -> {Describe(_change.state)}.");
            _change.transition.Enter(() => CompleteTransition(run));
            Update();
        }
        private void CompleteTransition([NotNull] TransitionRun _run)
        {
            if (!ReferenceEquals(_m_transitionRun, _run) || _run.completionRequested || _run.interruptRequested)
                return;

            _run.completionRequested = true;
            Console.LogVerbose(SystemNames.StateMachine, name, $"Completion requested for transition #{_run.change.id}.");
            Update();
        }
        private void FinishTransition([NotNull] TransitionRun _run)
        {
            _m_executingFrom = _run.from;
            _m_executingTo = _run.change.state;
            _m_lastStage = ExecutionStage.TransitionExitCompleted;
            _run.change.transition.Exit(true);
            _m_transitionRun = null;
            EnterDestination(_run.from, _run.change);
        }
        private void EnterDestination(_AState<T_STATE_TYPE, T_TARGET> _from, [NotNull] ChangeOperation _change)
        {
            _m_lastStage = ExecutionStage.StateEnter;
            _change.state?.ValidateEntry(this);
            _m_curState = _change.state;
            Console.LogVerbose(SystemNames.StateMachine, name, $"#{_change.id} Entering {Describe(_change.state)}.");
            _change.state?.Enter(this);
            _m_lastStage = ExecutionStage.StateChangedEvent;
            Console.LogVerbose(SystemNames.StateMachine, name, $"#{_change.id} Completed: {Describe(_from)} -> {Describe(_change.state)}.");
            OnStateChg?.Invoke(_from, _change.state);
            _m_executingFrom = null;
            _m_executingTo = null;
            if (_m_curState != null)
                Update();
        }


        private enum ExecutionStage
        {
            None, Dispatch, StateExit, TransitionEnter, TransitionUpdate,
            TransitionExitCompleted, TransitionExitInterrupted, StateEnter, StateChangedEvent, StateUpdate
        }
        private sealed class TransitionRun
        {
            public readonly _AState<T_STATE_TYPE, T_TARGET> from;
            [NotNull] public readonly ChangeOperation change;
            public bool completionRequested;
            public bool interruptRequested;


            public TransitionRun(_AState<T_STATE_TYPE, T_TARGET> _from, [NotNull] ChangeOperation _change)
            {
                from = _from;
                change = _change;
            }
        }
        private abstract class _AOperation
        {
            public readonly long id;


            protected _AOperation(long _id) { id = _id; }
        }
        private sealed class ChangeOperation : _AOperation
        {
            public readonly _AState<T_STATE_TYPE, T_TARGET> state;
            public readonly _ATransition<T_STATE_TYPE, T_TARGET> transition;
            public readonly bool interruptTransition;


            public ChangeOperation(long _id, _AState<T_STATE_TYPE, T_TARGET> _state,
                _ATransition<T_STATE_TYPE, T_TARGET> _transition, bool _interruptTransition) : base(_id)
            {
                state = _state;
                transition = _transition;
                interruptTransition = _interruptTransition;
            }


            public override string ToString()
            {
                return $"#{id} ChangeState({Describe(state)}, transition={transition?.name ?? "null"}, interrupt={interruptTransition})";
            }
        }
        private sealed class UpdateOperation : _AOperation
        {
            private readonly bool _m_force;


            public UpdateOperation(long _id, bool _force) : base(_id) { _m_force = _force; }


            public override string ToString() { return $"#{id} Update(force={_m_force})"; }
        }
    }
}
