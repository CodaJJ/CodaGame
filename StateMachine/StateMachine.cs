// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace CodaGame.StateMachine
{
    /// <summary>
    /// A single-threaded state machine with a typed owner and optional asynchronous transitions.
    /// </summary>
    /// <remarks>
    /// The caller is responsible for supplying a non-null target and name; construction does not validate them.
    /// Each change creates a clip containing its transition, destination state and pending updates.
    /// Callers create fresh state and transition instances for each change.
    /// A completion accepted before the next change enters its destination after the current hook returns.
    /// An allowed change requested first interrupts the unfinished transition.
    /// Only an explicit Update call queues a business update. Completing a transition resumes the change.
    /// After a hook returns, a valid completion enters the destination before any pending update runs.
    /// Updates belong to the latest requested clip, which may not have entered yet. Forced requests always
    /// enqueue; normal requests merge into the last pending update of that clip, including forced updates.
    /// Exceptions propagate without rollback, queue clearing or recovery. Finally releases the processing
    /// guard, but a failed machine may be partially executed and must not be assumed safe to continue.
    /// </remarks>
    public class StateMachine<T_STATE_TYPE, T_TARGET>
        where T_STATE_TYPE : struct, Enum
        where T_TARGET : class
    {
        private readonly T_TARGET _m_target;
        private readonly string _m_name;
        [NotNull] private readonly Queue<StateClip> _m_pendingClips;
        [NotNull] private StateClip _m_currentClip;
        [NotNull] private StateClip _m_latestClip;
        private bool _m_isProcessing;


        public StateMachine(T_TARGET _target, string _name)
        {
            _m_target = _target;
            _m_name = _name;
            _m_pendingClips = new Queue<StateClip>();
            _m_currentClip = new StateClip(this, null, null);
            _m_latestClip = _m_currentClip;
        }
        public StateMachine(T_TARGET _target)
            : this(_target, $"StateMachine_{Serialize.NextStateMachine()}")
        {
        }


        /// <summary>
        /// A completed change: invoked after the destination's OnEnter returns, or after exiting to null.
        /// Reports state types, not instances. Null means no state, distinct from any enum value.
        /// A -> transition -> B emits only (A, B). Interrupting into C emits (A, C); B never enters.
        /// Reentry emits (A, A). Listener requests wait until the entire notification returns.
        /// </summary>
        public event Action<T_STATE_TYPE?, T_STATE_TYPE?> OnStateChg;


        /// <summary>
        /// Destination of the currently executing clip. A remains visible during A.OnExit, then B is
        /// visible throughout the transition to B, even before B.OnEnter. Null for a clip with no destination.
        /// </summary>
        public _AState<T_STATE_TYPE, T_TARGET> curState { get { return _m_currentClip.curState; } }
        public _ATransition<T_STATE_TYPE, T_TARGET> curTransition { get { return _m_currentClip.curTransition; } }
        public bool isTransitioning { get { return _m_currentClip.isTransitioning; } }
        public T_TARGET target { get { return _m_target; } }
        public string name { get { return _m_name; } }


        /// <summary>
        /// Queue an update in the latest requested clip. Business code owns timing and update behavior.
        /// </summary>
        /// <param name="_force">Always enqueue this request; later normal updates in the same clip may merge into it.</param>
        public void Update(bool _force = false)
        {
            _m_latestClip.EnqueueUpdate(_force);
            TryProcessClips();
        }
        /// <summary>
        /// The latest accepted clip's destination decides whether to accept a new clip, even before its transition starts.
        /// A rejected request leaves the queue and current transition untouched.
        /// An unfinished transition exits with OnExit(false), skipping its destination.
        /// Requests start in order after the current hook returns. Updates belong to the latest requested clip.
        /// Changing to null clears the active state; it does not cancel later clips or shut down the machine.
        /// </summary>
        public void ChangeState(_AState<T_STATE_TYPE, T_TARGET> _state, _ATransition<T_STATE_TYPE, T_TARGET> _transition = null)
        {
            _AState<T_STATE_TYPE, T_TARGET> source = _m_latestClip.curState;
            if (source != null && !source.CanChangeTo(_state?.type))
            {
                LogVerbose($"Rejected change: {source.Describe()} -> {_state?.Describe() ?? "null"}.");
                return;
            }

            StateClip clip = new StateClip(this, _state, _transition);
            _m_latestClip = clip;
            _m_pendingClips.Enqueue(clip);
            LogVerbose($"Queued {clip}.");
            TryProcessClips();
        }


        [Conditional("UNITY_EDITOR")]
        private void LogVerbose(string _message)
        {
            Console.LogVerbose(SystemNames.StateMachine, name, _message);
        }
        private void TryProcessClips()
        {
            if (_m_isProcessing)
                return;

            _m_isProcessing = true;
            try
            {
                while (true)
                {
                    _m_currentClip.TryCompleteTransition();
                    bool consumedUpdate = _m_currentClip.TryUpdate();
                    if (consumedUpdate)
                        continue;

                    if (_m_pendingClips.Count == 0)
                        break;

                    StateClip nextClip = _m_pendingClips.Dequeue();
                    _AState<T_STATE_TYPE, T_TARGET> from = _m_currentClip.Exit();
                    _m_currentClip = nextClip;
                    _m_currentClip.Enter(from);
                }
            }
            finally
            {
                _m_isProcessing = false;
            }
        }


        private sealed class StateClip
        {
            private readonly uint _m_id;
            [NotNull] private readonly StateMachine<T_STATE_TYPE, T_TARGET> _m_machine;
            private readonly _AState<T_STATE_TYPE, T_TARGET> _m_state;
            private readonly _ATransition<T_STATE_TYPE, T_TARGET> _m_transition;
            private int _m_pendingUpdateCount;
            private _AState<T_STATE_TYPE, T_TARGET> _m_from;
            private bool _m_isTransitioning;
            private bool _m_completionRequested;


            public StateClip([NotNull] StateMachine<T_STATE_TYPE, T_TARGET> _machine, _AState<T_STATE_TYPE, T_TARGET> _state, _ATransition<T_STATE_TYPE, T_TARGET> _transition)
            {
                _m_id = Serialize.Next();
                _m_machine = _machine;
                _m_state = _state;
                _m_transition = _transition;
                _m_isTransitioning = _transition != null;
            }


            public _AState<T_STATE_TYPE, T_TARGET> curState { get { return _m_state; } }
            public _ATransition<T_STATE_TYPE, T_TARGET> curTransition { get { return _m_isTransitioning ? _m_transition : null; } }
            public bool isTransitioning { get { return _m_isTransitioning; } }


            public _AState<T_STATE_TYPE, T_TARGET> Exit()
            {
                if (_m_isTransitioning)
                {
                    _m_machine.LogVerbose($"Interrupting clip #{_m_id}.");
                    _m_transition.Exit(false);
                    return _m_from;
                }

                _m_machine.LogVerbose($"#{_m_id} Exiting {_m_state?.Describe() ?? "null"}.");
                _m_state?.Exit();
                return _m_state;
            }
            public void Enter(_AState<T_STATE_TYPE, T_TARGET> _from)
            {
                _m_from = _from;
                if (_m_isTransitioning)
                {
                    _m_machine.LogVerbose($"Starting clip #{_m_id}: {_m_from?.Describe() ?? "null"} -> {_m_transition.name} -> {_m_state?.Describe() ?? "null"}.");
                    _m_transition.Enter(_m_machine, _from, _m_state, CompleteTransition);
                }
                else
                    EnterState();
            }
            public void EnqueueUpdate(bool _force)
            {
                if (_force || _m_pendingUpdateCount == 0)
                    _m_pendingUpdateCount++;
            }
            public bool TryUpdate()
            {
                if (_m_pendingUpdateCount == 0)
                    return false;

                _m_pendingUpdateCount--;
                if (_m_isTransitioning)
                    _m_transition.Update();
                else
                    _m_state?.Update();
                return true;
            }
            public void TryCompleteTransition()
            {
                if (!_m_completionRequested)
                    return;

                _m_transition.Exit(true);
                _m_isTransitioning = false;
                _m_completionRequested = false;
                EnterState();
            }
            public override string ToString()
            {
                return $"#{_m_id} StateClip({_m_state?.Describe() ?? "null"}, transition={_m_transition?.name ?? "null"})";
            }


            private void CompleteTransition()
            {
                if (!_m_isTransitioning || _m_completionRequested)
                    return;

                bool isInterrupted = this != _m_machine._m_latestClip;
                if (isInterrupted)
                    return;

                _m_completionRequested = true;
                _m_machine.LogVerbose($"Completion requested for clip #{_m_id}.");
                _m_machine.TryProcessClips();
            }
            private void EnterState()
            {
                _m_machine.LogVerbose($"#{_m_id} Entering {_m_state?.Describe() ?? "null"}.");
                _m_state?.Enter(_m_machine);
                _m_machine.LogVerbose($"#{_m_id} Completed: {_m_from?.Describe() ?? "null"} -> {_m_state?.Describe() ?? "null"}.");
                _m_machine.OnStateChg?.Invoke(_m_from?.type, _m_state?.type);
                _m_from = null;
            }
        }
    }
}
