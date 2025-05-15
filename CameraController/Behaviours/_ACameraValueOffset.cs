// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Base class for camera value offsets
    /// </summary>
    public abstract class _ACameraValueOffset<T_VALUE> : _AValueOffset<T_VALUE>
        where T_VALUE : struct
    {
        // The name of the offset
        private readonly string _m_name;
        
        // The camera controller that this offset is attached to
        private CameraController _m_cameraController;


        protected _ACameraValueOffset()
            : this($"CameraValueOffset_{Serialize.NextCameraValueOffset()}")
        {
        }
        protected _ACameraValueOffset(string _name)
        {
            _m_name = _name;
        }
        
        
        /// <summary>
        /// The name of the offset
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
        /// Set the camera controller that this offset is attached to
        /// </summary>
        internal void SetCameraController(CameraController _cameraController)
        {
            _m_cameraController = _cameraController;
        }
        
        
        /// <summary>
        /// Log an error if the camera controller is null
        /// </summary>
        [MemberNotNullWhen(true, nameof(_m_cameraController))]
        private bool LogIfCameraControllerNull()
        {
            if (_m_cameraController == null)
            {
                Console.LogError(SystemNames.CameraController, _m_name, "Camera");
                return false;
            }

            return true;
        }
    }
}