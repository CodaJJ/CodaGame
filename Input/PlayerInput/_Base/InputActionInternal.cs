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
            // Complete snapshots for every logic frame, including frames with no input events.
            [NotNull] private readonly FrameRecord[] _m_actionCtxBuffer;
            // Specific type event management dictionary
            [NotNull] private readonly Dictionary<Type, _ASpecificTypeEvent> _m_specificTypeEvents;
            // Action object
            [NotNull] private readonly InputAction _m_action;
            [NotNull] private readonly InputActionMapInternal _m_actionMap;
            // Enable count
            private int _m_enabledCount;
            private int _m_snapshotFrame;
            private bool _m_isRebinding;


            public InputActionInternal([NotNull] _APlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _playerInput,
                [NotNull] InputActionMapInternal _actionMap, [NotNull] InputAction _action)
            {
                _m_playerInput = _playerInput;
                _m_actionMap = _actionMap;
                _m_specificTypeEvents = new Dictionary<Type, _ASpecificTypeEvent>();
                _m_actionCtxBuffer = new FrameRecord[_AGameMain.instance.inputHistoryFrameCount];
                for (int i = 0; i < _m_actionCtxBuffer.Length; i++)
                    _m_actionCtxBuffer[i].frameIndex = -1;
                _m_snapshotFrame = _AGameMain.instance.logicFrameCount;
                _m_actionCtxBuffer[_m_snapshotFrame % _m_actionCtxBuffer.Length].frameIndex = _m_snapshotFrame;

                _m_action = _action;
                _m_enabledCount = 1;
                _m_action.started += OnStarted;
                _m_action.performed += OnPerformed;
                _m_action.canceled += OnCanceled;
                RefreshEnabled();
            }


            public event Action<InputAction.CallbackContext> started { add { _m_startedWithCtx += value; } remove { _m_startedWithCtx -= value; } }
            public event Action<InputAction.CallbackContext> performed { add { _m_performedWithCtx += value; } remove { _m_performedWithCtx -= value; } }
            public event Action<InputAction.CallbackContext> canceled { add { _m_canceledWithCtx += value; } remove { _m_canceledWithCtx -= value; } }


            private event Action _m_started;
            private event Action _m_performed;
            private event Action _m_canceled;

            private event Action<InputAction.CallbackContext> _m_startedWithCtx;
            private event Action<InputAction.CallbackContext> _m_performedWithCtx;
            private event Action<InputAction.CallbackContext> _m_canceledWithCtx;


            public void Dispose()
            {
                _m_action.started -= OnStarted;
                _m_action.performed -= OnPerformed;
                _m_action.canceled -= OnCanceled;
            }

            // Net count, may be negative; both action and map counts must be positive to receive input.
            // See InputActionMapInternal.Enable for the semantics.
            public void Enable()
            {
                _m_enabledCount++;
                if (_m_enabledCount == 1)
                    RefreshEnabled();
            }
            public void Disable()
            {
                _m_enabledCount--;
                if (_m_enabledCount == 0)
                    RefreshEnabled();
            }
            public void RefreshEnabled()
            {
                // Contract: do not synchronously re-enable this action or its map from the canceled
                // callback triggered by Disable(). Unity finishes disabling after that callback returns,
                // which can leave the native state out of sync with our counts. Defer re-enabling instead.
                bool enabled = _m_enabledCount > 0 && _m_actionMap.isEnabled
                    && !_m_isRebinding && _m_playerInput.isEnabled;
                if (_m_action.enabled == enabled)
                    return;

                if (enabled)
                    _m_action.Enable();
                else
                    _m_action.Disable();
            }

            public void OnStarted(InputAction.CallbackContext _ctx)
            {
                InsertContextToBuffer(_ctx);
                _m_startedWithCtx?.Invoke(_ctx);
                _m_started?.Invoke();
                foreach (_ASpecificTypeEvent typeEvent in _m_specificTypeEvents.Values)
                    typeEvent.OnStarted(_ctx);

                _m_playerInput.ChangeControlScheme(_ctx.control.device.ToControlSchemeType());
            }
            public void OnPerformed(InputAction.CallbackContext _ctx)
            {
                InsertContextToBuffer(_ctx);
                // A different device can take over an active Value action without another started event.
                _m_playerInput.ChangeControlScheme(_ctx.control.device.ToControlSchemeType());
                _m_performedWithCtx?.Invoke(_ctx);
                _m_performed?.Invoke();
                foreach (_ASpecificTypeEvent typeEvent in _m_specificTypeEvents.Values)
                    typeEvent.OnPerformed(_ctx);
            }
            public void OnCanceled(InputAction.CallbackContext _ctx)
            {
                InsertContextToBuffer(_ctx);
                _m_canceledWithCtx?.Invoke(_ctx);
                _m_canceled?.Invoke();
                foreach (_ASpecificTypeEvent typeEvent in _m_specificTypeEvents.Values)
                    typeEvent.OnCanceled(_ctx);
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

            // Called whenever the logic loop enters a new frame. Copy the state, never the previous edges.
            public void AdvanceLogicFrame(int _logicFrame)
            {
                if (_logicFrame <= _m_snapshotFrame)
                    return;

                FrameRecord previous = _m_actionCtxBuffer[_m_snapshotFrame % _m_actionCtxBuffer.Length];
                int firstFrame = Math.Max(_m_snapshotFrame + 1, _logicFrame - _m_actionCtxBuffer.Length + 1);
                for (int frame = firstFrame; frame <= _logicFrame; frame++)
                {
                    _m_actionCtxBuffer[frame % _m_actionCtxBuffer.Length] = new FrameRecord
                    {
                        frameIndex = frame,
                        performed = previous.performed,
                        value = previous.value
                    };
                }
                _m_snapshotFrame = _logicFrame;
            }

            // Waiting/Performed are complementary states; Started/Canceled are independent per-frame events.
            public bool WasActionWaiting(int _logicFrame)
            {
                return !GetSnapshot(_logicFrame).performed;
            }
            public bool WasActionStarted(int _logicFrame)
            {
                return GetSnapshot(_logicFrame).started;
            }
            public bool WasActionPerformed(int _logicFrame)
            {
                return GetSnapshot(_logicFrame).performed;
            }
            // The release event remains recorded even if input starts again on the same frame.
            public bool WasActionCanceled(int _logicFrame)
            {
                return GetSnapshot(_logicFrame).canceled;
            }
            public T_VALUE ReadValue<T_VALUE>(int _logicFrame)
                where T_VALUE : struct
            {
                return GetSnapshot(_logicFrame).value is T_VALUE value ? value : default;
            }

            public InputActionRebindingExtensions.RebindingOperation StartRebinding(int _bindingIndex)
            {
                ReadOnlyArray<InputBinding> bindings = _m_action.bindings;
                if (_bindingIndex < 0 || _bindingIndex >= bindings.Count || bindings[_bindingIndex].isComposite)
                {
                    Console.LogError(SystemNames.Input, "StartRebinding requires a valid non-composite binding index.");
                    return null;
                }

                _m_isRebinding = true;
                RefreshEnabled();
                return _m_action.PerformInteractiveRebinding(_bindingIndex);
            }
            public void EndRebinding()
            {
                _m_isRebinding = false;
                RefreshEnabled();
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


            private FrameRecord GetSnapshot(int _logicFrame)
            {
                if (_logicFrame < 0 || _logicFrame > _m_snapshotFrame ||
                    _logicFrame <= _m_snapshotFrame - _m_actionCtxBuffer.Length)
                    return default;

                FrameRecord record = _m_actionCtxBuffer[_logicFrame % _m_actionCtxBuffer.Length];
                return record.frameIndex == _logicFrame ? record : default;
            }
            private void InsertContextToBuffer(InputAction.CallbackContext _ctx)
            {
                if (!_AGameMain.instance.TryCalculateLogicFrameIndex(_ctx.time, out int frameIndex))
                    return;
                int frameCount = _AGameMain.instance.logicFrameCount;
                AdvanceLogicFrame(frameCount);

                if (frameIndex > frameCount)
                {
                    Console.LogWarning(SystemNames.Input, "Future frame context detected, adjusting to current frame.");
                    frameIndex = frameCount;
                }

                if (frameIndex < 0 || frameIndex <= frameCount - _m_actionCtxBuffer.Length)
                    return;

                int bufferIndex = frameIndex % _m_actionCtxBuffer.Length;
                ref FrameRecord record = ref _m_actionCtxBuffer[bufferIndex];
                // Do not create history from before this action was constructed.
                if (record.frameIndex != frameIndex)
                    return;
                if (_ctx.started)
                    record.started = true;
                if (_ctx.canceled)
                    record.canceled = true;
                // Older events may arrive after a newer event in the same logic frame. Keep their edges,
                // but only the latest timestamp determines the final state (equal timestamps use callback order).
                if (record.hasEvent && _ctx.time < record.lastEventTime)
                    return;
                record.hasEvent = true;
                record.lastEventTime = _ctx.time;
                record.performed = !_ctx.canceled;
                // Box once per event; later snapshots share this reference without allocating.
                record.value = _ctx.ReadValueAsObject();

                // A timestamped event may belong to an already-created historical frame. Update inherited
                // state through the gap, stopping at the next event; do not propagate started/canceled edges.
                for (int frame = frameIndex + 1; frame <= _m_snapshotFrame; frame++)
                {
                    ref FrameRecord next = ref _m_actionCtxBuffer[frame % _m_actionCtxBuffer.Length];
                    if (next.hasEvent)
                        break;
                    next.performed = record.performed;
                    next.value = record.value;
                }
            }


            // Per-frame slot in the action callback circular buffer.
            private struct FrameRecord
            {
                public int frameIndex;
                public bool hasEvent;
                public double lastEventTime;
                // Events accumulated within this frame.
                public bool started;
                public bool canceled;
                // Final active-input state, overwritten by each event in chronological order.
                public bool performed;
                // Boxed action value captured at event time. Canceled events carry null, meaning default.
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
                public override bool hasCallback { get { return _m_started != null || _m_performed != null || _m_canceled != null; } }


                private event Action<T_VALUE> _m_started;
                private event Action<T_VALUE> _m_performed;
                private event Action<T_VALUE> _m_canceled;


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
