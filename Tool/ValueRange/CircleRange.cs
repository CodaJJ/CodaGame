// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A range defined by a <see cref="Circle"/>.
    /// </summary>
    [Serializable]
    public class CircleRange : _IValueRange<Vector2>
    {
        public Circle circle;
        
        
        public CircleRange(Vector2 _center, float _radius)
        {
            circle = new Circle(_center, _radius);
        }
        public CircleRange(Circle _circle)
        {
            circle = _circle;
        }
        
        
        public bool IsInRange(Vector2 _value)
        {
            return circle.Contains(_value);
        }
        public Vector2 ClampValue(Vector2 _value)
        {
            return circle.ClosestPoint(_value);
        }
        public Vector2 RandomValue()
        {
            return circle.RandomPoint();
        }
    }
}