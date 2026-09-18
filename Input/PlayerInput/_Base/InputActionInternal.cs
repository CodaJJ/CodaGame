// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace CodaGame.Base
{
    public partial class _APlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>
        where T_ACTION_MAP_ENUM : Enum
        where T_ACTION_ENUM : Enum
    {
        /// <summary>
        /// Internal action management class
        /// </summary>
        private class InputActionInternal
        {
            [NotNull] private readonly _APlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _m_playerInput;
            // Cached per-frame action records (circular buffer). Same-frame phases are OR-merged so that
            // started/performed/canceled emitted on the same frame are all queryable.
            [NotNull] private readonly FrameRecord[] _m_actionCtxBuffer;
            // Specific type event management dictionary
            [NotNull] private readonly Dictionary<Type, _ASpecificTypeEvent> _m_specificTypeEvents;
            // Action object
            [NotNull] private readonly InputAction _m_action;
            // Enable count
            private int _m_enabledCount;
            // Carry-forward state, updated on every event and persisted beyond the buffer window so holds
            // longer than _k_actionBufferTime still reconstruct. _m_lastValue is the latest event's value
            // (boxed once per event); _m_phase is the action's current resting phase (Waiting / Started /
            // Performed), driven by the event stream. _m_lastEventFrame is the logic frame of the most recent
            // event; -1 means no event has occurred yet.
            private object _m_lastValue;
            private InputActionPhase _m_phase = InputActionPhase.Waiting;
            private int _m_lastEventFrame = -1;

            private event Action _m_started;
            private event Action _m_performed;
            private event Action _m_canceled;

            private event Action<InputAction.CallbackContext> _m_startedWithCtx;
            private event Action<InputAction.CallbackContext> _m_performedWithCtx;
            private event Action<InputAction.CallbackContext> _m_canceledWithCtx;


            public InputActionInternal([NotNull] _APlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _playerInput, [NotNull] InputAction _action)
            {
                _m_playerInput = _playerInput;
                _m_specificTypeEvents = new Dictionary<Type, _ASpecificTypeEvent>();
                _m_actionCtxBuffer = new FrameRecord[Mathf.CeilToInt(_k_actionBufferTime * _AGameMain.instance.logicFps)];
                for (int i = 0; i < _m_actionCtxBuffer.Length; i++)
                    _m_actionCtxBuffer[i].frameIndex = -1;

                _m_action = _action;
                _m_action.Enable();
                _m_enabledCount = 1;
                _m_action.started += OnStarted;
                _m_action.performed += OnPerformed;
                _m_action.canceled += OnCanceled;
            }


            public event Action<InputAction.CallbackContext> started { add { _m_startedWithCtx += value; } remove { _m_startedWithCtx -= value; } }
            public event Action<InputAction.CallbackContext> performed { add { _m_performedWithCtx += value; } remove { _m_performedWithCtx -= value; } }
            public event Action<InputAction.CallbackContext> canceled { add { _m_canceledWithCtx += value; } remove { _m_canceledWithCtx -= value; } }


            public void Dispose()
            {
                _m_action.started -= OnStarted;
                _m_action.performed -= OnPerformed;
                _m_action.canceled -= OnCanceled;
            }

            // Net count, may be negative; the action is active only while the count is positive.
            // See InputActionMapInternal.Enable for the semantics.
            public void Enable()
            {
                _m_enabledCount++;
                if (_m_enabledCount == 1)
                    _m_action.Enable();
            }
            public void Disable()
            {
                _m_enabledCount--;
                if (_m_enabledCount == 0)
                    _m_action.Disable();
            }

            public void OnStarted(InputAction.CallbackContext _ctx)
            {
                _m_startedWithCtx?.Invoke(_ctx);
                _m_started?.Invoke();
                foreach (_ASpecificTypeEvent typeEvent in _m_specificTypeEvents.Values)
                    typeEvent.OnStarted(_ctx);

                InsertContextToBuffer(_ctx);
                _m_playerInput.ChangeControlScheme(_ctx.control.device.ToControlSchemeType());
            }
            public void OnPerformed(InputAction.CallbackContext _ctx)
            {
                _m_performedWithCtx?.Invoke(_ctx);
                _m_performed?.Invoke();
                foreach (_ASpecificTypeEvent typeEvent in _m_specificTypeEvents.Values)
                    typeEvent.OnPerformed(_ctx);

                InsertContextToBuffer(_ctx);
            }
            public void OnCanceled(InputAction.CallbackContext _ctx)
            {
                _m_canceledWithCtx?.Invoke(_ctx);
                _m_canceled?.Invoke();
                foreach (_ASpecificTypeEvent typeEvent in _m_specificTypeEvents.Values)
                    typeEvent.OnCanceled(_ctx);

                InsertContextToBuffer(_ctx);
            }

            public void AddCallback(InputCallbackType _callbackType, Action _callback)
            {
                switch (_callbackType)
                {
                    case InputCallbackType.Started:
                        _m_started += _callback;
                        break;
                    case InputCallbackType.Performed:
                        _m_performed += _callback;
                        break;
                    case InputCallbackType.Canceled:
                        _m_canceled += _callback;
                        break;
                }
            }
            public void AddCallback<T_VALUE>(InputCallbackType _callbackType, Action<T_VALUE> _callback)
                where T_VALUE : struct
            {
                Type type = typeof(T_VALUE);
                if (!_m_specificTypeEvents.TryGetValue(type, out _ASpecificTypeEvent typeEvent))
                {
                    typeEvent = new SpecificTypeEvent<T_VALUE>();
                    _m_specificTypeEvents[type] = typeEvent;
                }

                ((SpecificTypeEvent<T_VALUE>)typeEvent).AddCallback(_callbackType, _callback);
            }
            public void RemoveCallback(InputCallbackType _callbackType, Action _callback)
            {
                switch (_callbackType)
                {
                    case InputCallbackType.Started:
                        _m_started -= _callback;
                        break;
                    case InputCallbackType.Performed:
                        _m_performed -= _callback;
                        break;
                    case InputCallbackType.Canceled:
                        _m_canceled -= _callback;
                        break;
                }
            }
            public void RemoveCallback<T_VALUE>(InputCallbackType _callbackType, Action<T_VALUE> _callback)
                where T_VALUE : struct
            {
                Type type = typeof(T_VALUE);
                if (_m_specificTypeEvents.TryGetValue(type, out _ASpecificTypeEvent typeEvent))
                {
                    ((SpecificTypeEvent<T_VALUE>)typeEvent).RemoveCallback(_callbackType, _callback);
                    if (!typeEvent.hasCallback)
                        _m_specificTypeEvents.Remove(type);
                }
            }
            // Reconstructs the action's resting phase (Waiting / Started / Performed) on the given logic frame
            // by carrying the phase forward from the most recent event — the same scheme ReadValue uses for
            // values, so it holds across frames where no callback fired and across holds that outlive the
            // buffer. Canceled is not a resting phase (Unity resolves it to Waiting immediately); the release
            // edge is exposed by WasActionCanceled instead.
            private InputActionPhase ReconstructPhase(int _logicFrame)
            {
                if (_logicFrame >= _m_lastEventFrame)
                    return _m_phase;
                for (int frame = _logicFrame; frame > _logicFrame - _m_actionCtxBuffer.Length && frame >= 0; frame--)
                {
                    FrameRecord record = _m_actionCtxBuffer[frame % _m_actionCtxBuffer.Length];
                    if (record.frameIndex != frame)
                        continue;
                    if (record.canceled)
                        return InputActionPhase.Waiting;
                    if (record.performed)
                        return InputActionPhase.Performed;
                    if (record.started)
                        return InputActionPhase.Started;
                }
                return InputActionPhase.Waiting;
            }
            // STATE queries: "is the action in phase X on this frame". Mutually exclusive — exactly one of
            // Waiting / Started / Performed is true per frame. A held action reports Performed on every frame
            // between its performed and canceled events. Note Started only has duration under interactions like
            // Hold (charging); for a plain action started→performed is instantaneous, so WasActionStarted is
            // essentially never true. Callers wanting an edge ("pressed this frame") derive it from a phase
            // transition or a callback.
            public bool WasActionWaiting(int _logicFrame)
            {
                return ReconstructPhase(_logicFrame) == InputActionPhase.Waiting;
            }
            public bool WasActionStarted(int _logicFrame)
            {
                return ReconstructPhase(_logicFrame) == InputActionPhase.Started;
            }
            public bool WasActionPerformed(int _logicFrame)
            {
                return ReconstructPhase(_logicFrame) == InputActionPhase.Performed;
            }
            // EDGE, not a phase: the release transition on this frame. Canceled has no resting phase (Unity
            // resolves it to Waiting immediately), so it stays a per-frame event query.
            public bool WasActionCanceled(int _logicFrame)
            {
                FrameRecord record = _m_actionCtxBuffer[_logicFrame % _m_actionCtxBuffer.Length];
                return record.frameIndex == _logicFrame && record.canceled;
            }
            public T_VALUE ReadValue<T_VALUE>(int _logicFrame)
                where T_VALUE : struct
            {
                // A continuous value is not an event: between events it stays constant, so the value at
                // _logicFrame is the value of the most recent event at or before it. Held input therefore
                // reconstructs by carrying forward the last event's value, rather than reading the buffer slot
                // directly (which is empty on any frame where no callback fired).

                // Fast path — reading the current (or a later) frame: the latest event's value still holds.
                // This also covers holds longer than the buffer window, since _m_lastValue persists after the
                // event ages out of the ring.
                if (_logicFrame >= _m_lastEventFrame)
                    return _m_lastValue is T_VALUE latest ? latest : default;

                // Catch-up path — reading a past frame while a newer event already exists (the logic loop is
                // replaying frames behind wall-clock): walk back to the most recent recorded event at or before
                // _logicFrame, which skips the future events sitting at frames > _logicFrame.
                for (int frame = _logicFrame; frame > _logicFrame - _m_actionCtxBuffer.Length && frame >= 0; frame--)
                {
                    FrameRecord record = _m_actionCtxBuffer[frame % _m_actionCtxBuffer.Length];
                    if (record.frameIndex == frame && record.value != null)
                        return record.value is T_VALUE past ? past : default;
                }
                return default;
            }
            public InputActionRebindingExtensions.RebindingOperation StartRebinding(int _bindingIndex)
            {
                return _m_action.PerformInteractiveRebinding(_bindingIndex);
            }
            public InputControl GetBindingControl(int _bindingIndex)
            {
                ReadOnlyArray<InputBinding> bindings = _m_action.bindings;
                if (_bindingIndex < 0 || _bindingIndex >= bindings.Count)
                    return null;

                InputBinding binding = bindings[_bindingIndex];
                if (binding.isComposite)
                {
                    Console.LogError(SystemNames.Input, $"Binding index {_bindingIndex} of action {_m_action.name} refers to a composite root, which has no single control. Pass the index of a composite part instead.");
                    return null;
                }

                string path = binding.effectivePath;
                if (string.IsNullOrEmpty(path))
                    return null;

                foreach (InputControl control in _m_action.controls)
                {
                    if (InputControlPath.Matches(path, control))
                        return control;
                }
                return null;
            }


            private void InsertContextToBuffer(InputAction.CallbackContext _ctx)
            {
                int frameIndex = _AGameMain.instance.CalculateLogicFrameIndex(_ctx.time);
                int frameCount = _AGameMain.instance.CalculateLogicFrameIndex(Time.realtimeSinceStartupAsDouble);

                if (frameIndex > frameCount)
                {
                    Console.LogWarning(SystemNames.Input, "Future frame context detected, adjusting to current frame.");
                    frameIndex = frameCount;
                }

                if (frameIndex < frameCount - _m_actionCtxBuffer.Length)
                    return;

                int bufferIndex = frameIndex % _m_actionCtxBuffer.Length;
                ref FrameRecord record = ref _m_actionCtxBuffer[bufferIndex];
                // Different frame in this slot (either stale wrap-around or first write) — reset before merging.
                if (record.frameIndex != frameIndex)
                {
                    record.frameIndex = frameIndex;
                    record.started = false;
                    record.performed = false;
                    record.canceled = false;
                }
                if (_ctx.started)
                    record.started = true;
                if (_ctx.performed)
                    record.performed = true;
                if (_ctx.canceled)
                    record.canceled = true;

                // Capture the value at event time (boxed once per event — events are infrequent) and carry it
                // forward, together with the performed-phase state, so ReadValue / WasActionPerformed can
                // reconstruct held state between events — including holds that outlive the buffer window.
                object value = _ctx.ReadValueAsObject();
                record.value = value;
                _m_lastValue = value;
                if (_ctx.started)
                    _m_phase = InputActionPhase.Started;
                if (_ctx.performed)
                    _m_phase = InputActionPhase.Performed;
                if (_ctx.canceled)
                    _m_phase = InputActionPhase.Waiting;
                _m_lastEventFrame = frameIndex;
            }


            // Per-frame slot in the action callback circular buffer.
            private struct FrameRecord
            {
                public int frameIndex;
                public bool started;
                public bool performed;
                public bool canceled;
                // Boxed action value captured at event time (null if this slot only ever recorded edge flags,
                // which never happens in practice — every event carries a value).
                public object value;
            }


            // Event handling for specific types
            private abstract class _ASpecificTypeEvent
            {
                public abstract bool hasCallback { get; }


                public abstract void OnStarted(InputAction.CallbackContext _ctx);
                public abstract void OnPerformed(InputAction.CallbackContext _ctx);
                public abstract void OnCanceled(InputAction.CallbackContext _ctx);
            }


            // Event handling for concrete types
            private class SpecificTypeEvent<T_VALUE> : _ASpecificTypeEvent
                where T_VALUE : struct
            {
                private event Action<T_VALUE> _m_started;
                private event Action<T_VALUE> _m_performed;
                private event Action<T_VALUE> _m_canceled;


                public override bool hasCallback { get { return _m_started != null || _m_performed != null || _m_canceled != null; } }


                public override void OnStarted(InputAction.CallbackContext _ctx)
                {
                    if (_m_started == null)
                        return;

                    T_VALUE value = _ctx.ReadValue<T_VALUE>();
                    _m_started.Invoke(value);
                }
                public override void OnPerformed(InputAction.CallbackContext _ctx)
                {
                    if (_m_performed == null)
                        return;

                    T_VALUE value = _ctx.ReadValue<T_VALUE>();
                    _m_performed.Invoke(value);
                }
                public override void OnCanceled(InputAction.CallbackContext _ctx)
                {
                    if (_m_canceled == null)
                        return;

                    T_VALUE value = _ctx.ReadValue<T_VALUE>();
                    _m_canceled.Invoke(value);
                }


                public void AddCallback(InputCallbackType _callbackType, Action<T_VALUE> _callback)
                {
                    switch (_callbackType)
                    {
                        case InputCallbackType.Started:
                            _m_started += _callback;
                            break;
                        case InputCallbackType.Performed:
                            _m_performed += _callback;
                            break;
                        case InputCallbackType.Canceled:
                            _m_canceled += _callback;
                            break;
                    }
                }
                public void RemoveCallback(InputCallbackType _callbackType, Action<T_VALUE> _callback)
                {
                    switch (_callbackType)
                    {
                        case InputCallbackType.Started:
                            _m_started -= _callback;
                            break;
                        case InputCallbackType.Performed:
                            _m_performed -= _callback;
                            break;
                        case InputCallbackType.Canceled:
                            _m_canceled -= _callback;
                            break;
                    }
                }
            }
        }
    }
}