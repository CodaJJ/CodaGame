// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A range defined by a <see cref="Bounds"/>.
    /// </summary>
    [Serializable]
    public class BoundsRange : _IValueRange<Vector3>
    {
        public Bounds bounds;
        
        
        public BoundsRange(Vector3 _center, Vector3 _size)
        {
            bounds = new Bounds(_center, _size);
        }
        public BoundsRange(Bounds _bounds)
        {
            bounds = _bounds;
        }
        
        
        public bool IsInRange(Vector3 _value)
        {
            return bounds.Contains(_value);
        }
        public Vector3 ClampValue(Vector3 _value)
        {
            return bounds.ClosestPoint(_value);
        }
        public Vector3 RandomValue()
        {
            return bounds.RandomPoint();
        }
    }
}