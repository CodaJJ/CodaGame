// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;

namespace CodaGame.Base
{
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    internal class CameraControllerState : MonoBehaviour
    {
        public CameraController controller;
        
        // todo: show the camera controller state in the inspector
    }
}