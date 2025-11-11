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

namespace CodaGame
{
    public partial class PlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM>
        where T_ACTION_MAP_ENUM : Enum
        where T_ACTION_ENUM : Enum
    {
        /// <summary>
        /// Internal action management class
        /// </summary>
        private class InputActionInternal
        {
            [NotNull] private readonly PlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _m_playerInput;
            // Cached action callback context circular buffer
            [NotNull] private readonly InputAction.CallbackContext[] _m_actionCtxBuffer;
            // Specific type event management dictionary
            [NotNull] private readonly Dictionary<Type, _ASpecificTypeEvent> _m_specificTypeEvents;
            // Action object
            [NotNull] private readonly InputAction _m_action;
            // Enable count
            private int _m_enabledCount;

            private event Action _m_started;
            private event Action _m_performed;
            private event Action _m_canceled;

            private event Action<InputAction.CallbackContext> _m_startedWithCtx;
            private event Action<InputAction.CallbackContext> _m_performedWithCtx;
            private event Action<InputAction.CallbackContext> _m_canceledWithCtx;


            public InputActionInternal([NotNull] PlayerInput<T_ACTION_MAP_ENUM, T_ACTION_ENUM> _playerInput, [NotNull] InputAction _action)
            {
                _m_playerInput = _playerInput;
                _m_specificTypeEvents = new Dictionary<Type, _ASpecificTypeEvent>();
                _m_actionCtxBuffer = new InputAction.CallbackContext[Mathf.CeilToInt(_k_actionBufferTime * GameMain.instance.logicFps)];

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

            public void Enable()
            {
                if (_m_enabledCount == 0)
                    _m_action.Enable();
                _m_enabledCount++;
            }
            public void Disable()
            {
                _m_enabledCount--;
                if (_m_enabledCount <= 0)
                {
                    _m_enabledCount = 0;
                    _m_action.Disable();
                }
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
            public bool WasActionStarted(int _logicFrame)
            {
                return GetContextForFrame(_logicFrame)?.started ?? false;
            }
            public bool WasActionPerformed(int _logicFrame)
            {
                return GetContextForFrame(_logicFrame)?.performed ?? false;
            }
            public bool WasActionCanceled(int _logicFrame)
            {
                return GetContextForFrame(_logicFrame)?.canceled ?? false;
            }
            public T_VALUE ReadValue<T_VALUE>(int _logicFrame)
                where T_VALUE : struct
            {
                return GetContextForFrame(_logicFrame)?.ReadValue<T_VALUE>() ?? default;
            }
            public InputActionRebindingExtensions.RebindingOperation StartRebinding(int _bindingIndex)
            {
                return _m_action.PerformInteractiveRebinding(_bindingIndex);
            }
            public InputControl GetBindingControl(int _bindingIndex)
            {
                ReadOnlyArray<InputControl> controls = _m_action.controls;
                if (_bindingIndex < 0 || _bindingIndex >= controls.Count)
                    return null;

                return controls[_bindingIndex];
            }


            private void InsertContextToBuffer(InputAction.CallbackContext _ctx)
            {
                int frameIndex = GameMain.instance.CalculateLogicFrameIndex(_ctx.time);
                int frameCount = GameMain.instance.CalculateLogicFrameIndex(Time.realtimeSinceStartupAsDouble);

                if (frameIndex > frameCount)
                {
                    Console.LogWarning(SystemNames.Input, "Future frame context detected, adjusting to current frame.");
                    frameIndex = frameCount;
                }

                if (frameIndex < frameCount - _m_actionCtxBuffer.Length)
                    return;

                int bufferIndex = frameIndex % _m_actionCtxBuffer.Length;
                _m_actionCtxBuffer[bufferIndex] = _ctx;
            }
            private InputAction.CallbackContext? GetContextForFrame(int _logicFrame)
            {
                int bufferIndex = _logicFrame % _m_actionCtxBuffer.Length;
                InputAction.CallbackContext ctx = _m_actionCtxBuffer[bufferIndex];

                int ctxFrame = GameMain.instance.CalculateLogicFrameIndex(ctx.time);
                return ctxFrame == _logicFrame ? ctx : null;
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