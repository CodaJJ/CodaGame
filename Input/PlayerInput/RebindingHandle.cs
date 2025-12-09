// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using RebindingOperation = UnityEngine.InputSystem.InputActionRebindingExtensions.RebindingOperation;

namespace CodaGame
{
    /// <summary>
    /// Handle for managing input rebinding operations
    /// </summary>
    /// <remarks>
    /// <para>This class wraps Unity's RebindingOperation and provides additional functionality.</para>
    /// <para>like device filtering, candidate limiting, and timeout handling.</para>
    /// </remarks>
    public class RebindingHandle
    {
        // Maximum number of candidate controls allowed during rebinding
        private const int _k_maxCandidates = 10;
        // Temporary list for storing candidates to be removed
        [NotNull] private readonly List<InputControl> _m_tempCandidates;

        // The active rebinding operation (null when completed or canceled)
        private RebindingOperation _m_startRebinding;
        // List of devices allowed for rebinding
        [NotNull] private readonly List<InputDevice> _m_devices;
        // Task handle for timeout functionality
        private readonly TaskHandle _m_timeOutTask;
        // Callback invoked when rebinding completes or is canceled
        private readonly CompleteCallback _m_endCallback;

        // Type of the captured input button
        private InputButtonType _m_capturedInputType;
        // The captured input control
        private InputControl _m_capturedInput;


        /// <summary>
        /// Construct a rebinding handle
        /// </summary>
        /// <param name="_startRebinding">The Unity rebinding operation to wrap</param>
        /// <param name="_devices">List of devices allowed for rebinding</param>
        /// <param name="_timeOut">Timeout duration in seconds</param>
        /// <param name="_endCallback">Callback invoked when rebinding ends (true if successful, false if cancelled)</param>
        internal RebindingHandle([NotNull] RebindingOperation _startRebinding, [NotNull] List<InputDevice> _devices, float _timeOut, CompleteCallback _endCallback)
        {
            _m_tempCandidates = new List<InputControl>();

            _m_startRebinding = _startRebinding;
            _m_devices = _devices;
            _m_endCallback = _endCallback;

            _m_startRebinding.OnPotentialMatch(OnPotentialMatch);
            _m_startRebinding.Start();

            _m_timeOutTask = Task.RunDelayActionTask(TimeOut, _timeOut, UpdateType.Update, true);
        }


        /// <summary>
        /// Event triggered when a valid input is captured during rebinding
        /// </summary>
        public event Action onInputCaptured;
        /// <summary>
        /// Event triggered when the rebinding operation times out
        /// </summary>
        public event Action onTimeOut;

        /// <summary>
        /// Type of the captured input button
        /// </summary>
        public InputButtonType capturedInputType { get { return _m_capturedInputType; } }
        /// <summary>
        /// The captured input control
        /// </summary>
        public InputControl capturedInput { get { return _m_capturedInput; } }


        /// <summary>
        /// Complete the rebinding operation and apply the captured input
        /// </summary>
        /// <remarks>
        /// <para>This will save the captured input as the new binding. After calling this method, the rebinding handle cannot be used again.</para>
        /// </remarks>
        public void Complete()
        {
            if (_m_startRebinding == null)
            {
                Console.LogWarning(SystemNames.Input, "RebindingHandle", "Rebinding operation is already done.");
                return;
            }

            _m_startRebinding.Complete();
            _m_startRebinding.Dispose();
            _m_startRebinding = null;
            _m_timeOutTask.StopTask();
            _m_endCallback?.Invoke(true);
        }
        /// <summary>
        /// Cancel the rebinding operation without applying any changes
        /// </summary>
        /// <remarks>
        /// <para>This will discard any captured input and keep the original binding.</para>
        /// <para>After calling this method, the rebinding handle cannot be used again.</para>
        /// </remarks>
        public void Cancel()
        {
            if (_m_startRebinding == null)
            {
                Console.LogWarning(SystemNames.Input, "RebindingHandle", "Rebinding operation is already done.");
                return;
            }

            _m_startRebinding.Cancel();
            _m_startRebinding.Dispose();
            _m_startRebinding = null;
            _m_timeOutTask.StopTask();
            _m_endCallback?.Invoke(false);
        }


        // Internal callback for matches during rebinding
        private void OnPotentialMatch(RebindingOperation _op)
        {
            if (_op == null)
            {
                Console.LogError(SystemNames.Input, "RebindingHandle", "Rebinding operation is null in OnPotentialMatch.");
                return;
            }

            // Filter candidate devices
            CandidateDevicesFilter(_op);
            // Check candidate count
            CandidateCountCheck(_op);
            // Update captured input
            InputControl capturedControl = _op.selectedControl;
            if (_m_capturedInput == capturedControl)
                return;
            
            _m_capturedInput = capturedControl;
            _m_capturedInputType = capturedControl.ToButtonType();
            onInputCaptured?.Invoke();
        }
        // Limit the number of candidate controls during rebinding
        private void CandidateCountCheck(RebindingOperation _op)
        {
            InputControlList<InputControl> candidates = _op.candidates;
            if (candidates.Count <= _k_maxCandidates)
                return;
            
            Console.LogWarning(SystemNames.Input, "RebindingHandle", "Too many candidate controls detected during rebinding. Limiting to first " + _k_maxCandidates + " candidates.");
            for (int i = candidates.Count; i > _k_maxCandidates; i--)
                _m_tempCandidates.Add(candidates[i - 1]);
            
            foreach (InputControl control in _m_tempCandidates)
                _op.RemoveCandidate(control);
            _m_tempCandidates.Clear();
        }
        // Filter candidate controls based on allowed devices
        private void CandidateDevicesFilter(RebindingOperation _op)
        {
            InputControlList<InputControl> candidates = _op.candidates;
            foreach (InputControl control in candidates)
            {
                InputDevice device = control.device;
                if (_m_devices.Contains(device))
                    continue;
                
                _m_tempCandidates.Add(control);
            }
            
            foreach (InputControl control in _m_tempCandidates)
                _op.RemoveCandidate(control);
            _m_tempCandidates.Clear();
        }
        // Handle timeout event
        private void TimeOut()
        {
            onTimeOut?.Invoke();
            Cancel();
        }
    }
}