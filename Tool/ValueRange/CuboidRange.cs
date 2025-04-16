// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A range defined by a <see cref="Cuboid"/>.
    /// </summary>
    [Serializable]
    public class CuboidRange : _IValueRange<Vector3>
    {
        public Cuboid cuboid;
        
        
        public CuboidRange(Vector3 _center, Vector3 _size, Quaternion _rotation)
        {
            cuboid = new Cuboid(_center, _size, _rotation);
        }
        public CuboidRange(Vector3 _center, Vector3 _size, Vector3 _eulerRotation)
        {
            cuboid = new Cuboid(_center, _size, _eulerRotation);
        }
        public CuboidRange(Bounds _bounds, Vector3 _eulerRotation)
        {
            cuboid = new Cuboid(_bounds, _eulerRotation);
        }
        public CuboidRange(Bounds _bounds, Quaternion _rotation)
        {
            cuboid = new Cuboid(_bounds, _rotation);
        }
        public CuboidRange(Cuboid _cuboid)
        {
            cuboid = _cuboid;
        }
        
        
        public bool IsInRange(Vector3 _value)
        {
            return cuboid.Contains(_value);
        }
        public Vector3 ClampValue(Vector3 _value)
        {
            return cuboid.ClosestPoint(_value);
        }
        public Vector3 RandomValue()
        {
            return cuboid.RandomPoint();
        }
    }
}