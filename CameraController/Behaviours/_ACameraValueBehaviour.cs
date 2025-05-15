// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Base class for camera value behaviours
    /// </summary>
    /// <remarks>
    /// <inheritdoc />
    /// </remarks>
    public abstract class _ACameraValueBehaviour<T_VALUE> : _AValueBehaviourSoftBlend<T_VALUE>
        where T_VALUE : struct
    {
        // The name of the behaviour
        private readonly string _m_name;
        
        // The camera controller that this behaviour is attached to
        private CameraController _m_cameraController;
        
        
        protected _ACameraValueBehaviour(int _priority, float _fadeInTime = 0.5f, float _fadeOutTime = 0.5f) 
            : this($"CameraValueBehaviour_{Serialize.NextCameraValueBehaviour()}", _priority, _fadeInTime, _fadeOutTime)
        {
        }
        protected _ACameraValueBehaviour(string _name, int _priority, float _fadeInTime = 0.5f, float _fadeOutTime = 0.5f) 
            : base(_priority, _fadeInTime, _fadeOutTime)
        {
            _m_name = _name;
        }


        /// <summary>
        /// The name of the behaviour
        /// </summary>
        public string name { get { return _m_name; } }
        /// <summary>
        /// The current position of the camera
        /// </summary>
        public Vector3 cameraPosition
        {
            get
            {
                if (LogIfCameraControllerNull())
                    return _m_cameraController.position;

                return Vector3.zero;
            }
        }
        /// <summary>
        /// The current rotation of the camera
        /// </summary>
        public Quaternion cameraRotation
        {
            get
            {
                if (LogIfCameraControllerNull()) 
                    return _m_cameraController.rotation;

                return Quaternion.identity;
            }
        }
        /// <summary>
        /// The current size of the camera
        /// </summary>
        public float cameraSize
        {
            get
            {
                if (LogIfCameraControllerNull()) 
                    return _m_cameraController.size;

                return 0;
            }
        }
        
        
        /// <summary>
        /// Log an error if the camera controller is null
        /// </summary>
        [MemberNotNullWhen(true, nameof(_m_cameraController))]
        protected bool LogIfCameraControllerNull()
        {
            if (_m_cameraController == null)
            {
                Console.LogError(SystemNames.CameraController, _m_name, "Camera");
                return false;
            }

            return true;
        }


        /// <summary>
        /// Set the camera controller for this behaviour
        /// </summary>
        internal void SetCameraController(CameraController _cameraController)
        {
            _m_cameraController = _cameraController;
        }
        
        
        /// <inheritdoc />
        private protected sealed override void OnRemovedInternal()
        {
            base.OnRemovedInternal();
            if (_m_cameraController != null)
                Console.LogVerbose(SystemNames.CameraController, _m_cameraController.name, $"Behaviour {_m_name} removed from controller {_m_cameraController.name}.");
            _m_cameraController = null;
        }
    }
}