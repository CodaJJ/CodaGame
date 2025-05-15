// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;

namespace CodaGame
{
    /// <summary>
    /// A controller for managing camera behavior.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Controls a specific Unity Camera instance by managing its position, rotation, and size
    /// using behavior-based value controllers. Ensures smooth and dynamic camera motion with modular behavior control.
    /// </para>
    /// </remarks>
    public class CameraController
    {
        // The name of this controller.
        private readonly string _m_name;
        // The camera instance controlled by this controller.
        private readonly Camera _m_camera;
        // The Transform component of the controlled camera.
        private readonly Transform _m_cameraTransform;
        
        // The value controllers for position, rotation, and size.
        // The ValueController manages a value that can be influenced by multiple behaviours, offsets, and constraints.
        [NotNull] private readonly Vector3ValueController _m_position;
        [NotNull] private readonly QuaternionValueController _m_rotation;
        [NotNull] private readonly FloatValueController _m_size;
        
        // The behaviour controllers are an interface for external systems to add behaviours, offsets, and constraints.
        [NotNull] private readonly CameraBehaviourController<Vector3> _m_positionController;
        [NotNull] private readonly CameraBehaviourController<Quaternion> _m_rotationController;
        [NotNull] private readonly CameraBehaviourController<float> _m_sizeController;

        // The state of the camera controller, which is a MonoBehaviour attached to the camera, shows the current state of the controller.
        // Also It is used to manage the lifecycle of the controller, if it is null, the controller is disposed, otherwise it is enabled.
        private CameraControllerState _m_controllerState;


        public CameraController(string _name, Camera _camera)
        {
            _m_name = _name;
            _m_camera = _camera;
            _m_cameraTransform = _camera == null ? null : _camera.transform;

            _m_position = new Vector3ValueController(_m_name + ".position", _m_cameraTransform == null ? Vector3.zero : _m_cameraTransform.position);
            _m_rotation = new QuaternionValueController(_m_name + ".rotation", _m_cameraTransform == null ? Quaternion.identity : _m_cameraTransform.rotation);
            _m_size = new FloatValueController(_m_name + ".size", _camera == null ? 0 : _camera.orthographic ? _camera.orthographicSize : _camera.fieldOfView);

            _m_positionController = new CameraBehaviourController<Vector3>(this, _m_position);
            _m_rotationController = new CameraBehaviourController<Quaternion>(this, _m_rotation);
            _m_sizeController = new CameraBehaviourController<float>(this, _m_size);
            
            if (_camera == null)
            {
                Console.LogError(SystemNames.CameraController, _m_name, "Create CameraController failed, camera is null.");
                return;
            }

            CameraControllerState controllerState = _camera.gameObject.GetComponent<CameraControllerState>();
            if (controllerState != null)
            {
                Console.LogError(SystemNames.CameraController, _m_name, "Create CameraController failed, the camera already controller by another CameraController", _camera);
                return;
            }
            
            _m_controllerState = _camera.gameObject.AddComponent<CameraControllerState>();
            _m_controllerState.controller = this;
            
            Console.LogSystem(SystemNames.CameraController, _m_name, $"Create CameraController success, now the {_m_camera.name} is controlled by {_m_name}.");
        }
        
        
        /// <summary>
        /// The name of this controller.
        /// </summary>
        public string name { get { return _m_name; } }
        /// <summary>
        /// The camera instance controlled by this controller.
        /// </summary>
        public Camera camera { get { LogIfDisposed(); return _m_camera; } }
        /// <summary>
        /// The Transform component of the controlled camera.
        /// </summary>
        public Transform cameraTransform { get { LogIfDisposed(); return _m_cameraTransform; } }
        /// <summary>
        /// The position of the camera.
        /// </summary>
        /// <remarks>
        /// <para>Setting this value assigns a base position to the controller, which acts as the lowest-priority input.</para>
        /// <para>Getting this value returns the final blended result after applying all active behaviors, offsets, and constraints.</para>
        /// </remarks>
        public Vector3 position { get { LogIfDisposed(); return _m_position.value; } set { LogIfDisposed(); _m_position.baseValue = value; } }
        /// <summary>
        /// The rotation of the camera.
        /// </summary>
        /// <remarks>
        /// <para>Setting this value assigns a base rotation to the controller, which acts as the lowest-priority input.</para>
        /// <para>Getting this value returns the final blended result after applying all active behaviors, offsets, and constraints.</para>
        /// </remarks>
        public Quaternion rotation { get { LogIfDisposed(); return _m_rotation.value; } set { LogIfDisposed(); _m_rotation.baseValue = value; } }
        /// <summary>
        /// The size of the camera.
        /// </summary>
        /// <remarks>
        /// <para>Depending on the camera type, this value represents either the orthographic size or the field of view.</para>
        /// <para>Setting this value assigns a base size to the controller, which acts as the lowest-priority input.</para>
        /// <para>Getting this value returns the final blended result after applying all active behaviors, offsets, and constraints.</para>
        /// </remarks>
        public float size { get { LogIfDisposed(); return _m_size.value; } set { LogIfDisposed(); _m_size.baseValue = value; } }
        /// <summary>
        /// The position controller for managing camera position behaviors.
        /// </summary>
        public CameraBehaviourController<Vector3> positionController { get { LogIfDisposed(); return _m_positionController; } }
        /// <summary>
        /// The rotation controller for managing camera rotation behaviors.
        /// </summary>
        public CameraBehaviourController<Quaternion> rotationController { get { LogIfDisposed(); return _m_rotationController; } }
        /// <summary>
        /// The size controller for managing camera size behaviors.
        /// </summary>
        public CameraBehaviourController<float> sizeController { get { LogIfDisposed(); return _m_sizeController; } }
        /// <summary>
        /// Indicates whether this controller is enabled and actively controlling the camera.
        /// </summary>
        [MemberNotNullWhen(true, nameof(_m_controllerState), nameof(_m_camera), nameof(camera))]
        public bool isEnable { get { return _m_controllerState != null; } }


        /// <summary>
        /// Disposes of this controller, detaching it from the camera and disabling further updates or access.
        /// </summary>
        public void Dispose()
        {
            if (!isEnable)
            {
                Console.LogWarning(SystemNames.CameraController, _m_name, "The CameraController is already disposed.");
                return;
            }

            Object.DestroyImmediate(_m_controllerState);
            Console.LogSystem(SystemNames.CameraController, _m_name, $"Dispose CameraController success, now the {_m_camera.name} is not controlled by {_m_name}.");
            
            _m_controllerState = null;
        }
        /// <summary>
        /// Updates the camera's position, rotation, and size based on the current values of the value controllers.
        /// </summary>
        public void Update(float _deltaTime)
        {
            LogIfDisposed();
            
            _m_position.Update(_deltaTime);
            _m_rotation.Update(_deltaTime);
            _m_size.Update(_deltaTime);
            
            if (_m_cameraTransform != null)
            {
                _m_cameraTransform.position = _m_position.value;
                _m_cameraTransform.rotation = _m_rotation.value;
            }
            
            if (_m_camera != null)
            {
                if (_m_camera.orthographic)
                    _m_camera.orthographicSize = _m_size.value;
                else
                    _m_camera.fieldOfView = _m_size.value;
            }
        }
        
        
        /// <summary>
        /// Logs a warning if the controller is disposed.
        /// </summary>
        protected void LogIfDisposed()
        {
            if (!isEnable)
                Console.LogWarning(SystemNames.CameraController, _m_name, "The CameraController is already disposed. You can't use it anymore.");
        }
    }
}