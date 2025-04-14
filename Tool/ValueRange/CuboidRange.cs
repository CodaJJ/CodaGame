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