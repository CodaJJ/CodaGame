// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;

namespace CodaGame
{
    /// <summary>
    /// A controller to manage camera behaviours, offsets and constraints.
    /// </summary>
    public class CameraBehaviourController<T_VALUE>
        where T_VALUE : struct
    {
        // The camera controller that this controller is belonging to
        private readonly CameraController _m_cameraController;
        // The value controller that this controller is managing
        private readonly _AValueController<T_VALUE> _m_valueController;
        
        
        internal CameraBehaviourController(CameraController _cameraController, _AValueController<T_VALUE> _valueController)
        {
            _m_cameraController = _cameraController;
            _m_valueController = _valueController;
        }


        /// <summary>
        /// Add a behaviour to the camera controller.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="_AValueController{T_VALUE}.AddBehaviour(_AValueBehaviour{T_VALUE})" />
        /// </remarks>
        public void AddBehaviour(_ACameraValueBehaviour<T_VALUE> _behaviour)
        {
            if (_m_cameraController == null)
                return;
            if (_behaviour == null)
                return;

            if (_behaviour.IsBehaviourAdded(_m_valueController))
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Behaviour {_behaviour.name} already added.");
                return;
            }
            if (_behaviour.IsBehaviourAdded())
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Behaviour {_behaviour.name} already added to another controller.");
                return;
            }
            
            _behaviour.SetCameraController(_m_cameraController);
            _m_valueController?.AddBehaviour(_behaviour);
            Console.LogVerbose(SystemNames.CameraController, _m_cameraController.name, $"Behaviour {_behaviour.name} added to controller {_m_cameraController.name}.");
        }
        /// <summary>
        /// Remove a behaviour from the camera controller.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="_AValueController{T_VALUE}.RemoveBehaviour(_AValueBehaviour{T_VALUE})" />
        /// </remarks>
        public void RemoveBehaviour(_ACameraValueBehaviour<T_VALUE> _behaviour)
        {
            if (_m_cameraController == null)
                return;
            if (_behaviour == null)
                return;

            if (!_behaviour.IsBehaviourAdded(_m_valueController))
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Behaviour {_behaviour.name} not added to this controller.");
                return;
            }

            _m_valueController?.RemoveBehaviour(_behaviour);
        }
        /// <summary>
        /// Force remove a behaviour from the camera controller.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="_AValueController{T_VALUE}.ForceRemoveBehaviour(_AValueBehaviour{T_VALUE})" />
        /// </remarks>
        public void ForceRemoveBehaviour(_ACameraValueBehaviour<T_VALUE> _behaviour)
        {
            if (_m_cameraController == null)
                return;
            if (_behaviour == null)
                return;

            if (!_behaviour.IsBehaviourAdded(_m_valueController))
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Behaviour {_behaviour.name} not added to this controller.");
                return;
            }
            
            _m_valueController?.ForceRemoveBehaviour(_behaviour);
        }
        /// <summary>
        /// Add an offset to the camera controller.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="_AValueController{T_VALUE}.AddOffset(_AValueOffset{T_VALUE})" />
        /// </remarks>
        public void AddOffset(_ACameraValueOffset<T_VALUE> _offset)
        {
            if (_m_cameraController == null)
                return;
            if (_offset == null)
                return;

            if (_offset.IsOffsetAdded(_m_valueController))
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Offset {_offset.name} already added.");
                return;
            }
            if (_offset.IsOffsetAdded())
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Offset {_offset.name} already added to another controller.");
                return;
            }
            
            _offset.SetCameraController(_m_cameraController);
            _m_valueController?.AddOffset(_offset);
            Console.LogVerbose(SystemNames.CameraController, _m_cameraController.name, $"Offset {_offset.name} added to controller {_m_cameraController.name}.");
        }
        /// <summary>
        /// Remove an offset from the camera controller.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="_AValueController{T_VALUE}.RemoveOffset(_AValueOffset{T_VALUE})" />
        /// </remarks>
        public void RemoveOffset(_ACameraValueOffset<T_VALUE> _offset)
        {
            if (_m_cameraController == null)
                return;
            if (_offset == null)
                return;

            if (!_offset.IsOffsetAdded(_m_valueController))
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Offset {_offset.name} not added to this controller.");
                return;
            }
            
            _offset.SetCameraController(null);
            _m_valueController?.RemoveOffset(_offset);
            Console.LogVerbose(SystemNames.CameraController, _m_cameraController.name, $"Offset {_offset.name} removed from controller {_m_cameraController.name}.");
        }
        /// <summary>
        /// Add a constraint to the camera controller.
        /// </summary> 
        /// <remarks>
        /// <inheritdoc cref="_AValueController{T_VALUE}.AddConstraint(_AValueConstraint{T_VALUE})" />
        /// </remarks>
        public void AddConstraint(_ACameraValueConstraint<T_VALUE> _constraint)
        {
            if (_m_cameraController == null)
                return;
            if (_constraint == null)
                return;
            
            if (_constraint.IsConstraintAdded(_m_valueController))
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Constraint {_constraint.name} already added.");
                return;
            }
            if (_constraint.IsConstraintAdded())
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Constraint {_constraint.name} already added to another controller.");
                return;
            }
            
            _constraint.SetCameraController(_m_cameraController);
            _m_valueController?.AddConstraint(_constraint);
            Console.LogVerbose(SystemNames.CameraController, _m_cameraController.name, $"Constraint {_constraint.name} added to controller {_m_cameraController.name}.");
        }
        /// <summary>
        /// Remove a constraint from the camera controller.
        /// </summary>
        /// <remarks>
        /// <inheritdoc cref="_AValueController{T_VALUE}.RemoveConstraint(_AValueConstraint{T_VALUE})" />
        /// </remarks>
        public void RemoveConstraint(_ACameraValueConstraint<T_VALUE> _constraint)
        {
            if (_m_cameraController == null)
                return;
            if (_constraint == null)
                return;

            if (!_constraint.IsConstraintAdded(_m_valueController))
            {
                Console.LogWarning(SystemNames.CameraController, _m_cameraController.name, $"Constraint {_constraint.name} not added to this controller.");
                return;
            }
            
            _constraint.SetCameraController(null);
            _m_valueController?.RemoveConstraint(_constraint);
            Console.LogVerbose(SystemNames.CameraController, _m_cameraController.name, $"Constraint {_constraint.name} removed from controller {_m_cameraController.name}.");
        }
    }
}