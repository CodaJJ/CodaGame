// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodaGame
{
    /// <summary>
    /// A circle shape.
    /// </summary>
    [Serializable]
    public struct Circle : IEquatable<Circle>
    {
        public static bool operator ==(Circle _lhs, Circle _rhs) => _lhs.center == _rhs.center && Mathf.Approximately(_lhs.radius, _rhs.radius);
        public static bool operator !=(Circle _lhs, Circle _rhs) => !(_lhs == _rhs);
        
        
        public Vector2 center;
        public float radius;
        
     
        public Circle(Vector2 _center, float _radius)
        {
            center = _center;
            radius = _radius;
        }
        
        
        public float area { get { return Values.Pi * radius * radius; } }
        public float circumference { get { return Values.TwoPi * radius; } }


        public void Set(Vector2 _center, float _radius)
        {
            center = _center;
            radius = _radius;
        }
        /// <summary>
        /// Returns the closest point on the circle to the given point.
        /// </summary>
        [Pure]
        public Vector2 ClosestPointOn(Vector2 _point)
        {
            Vector2 direction = _point - center;
            direction.Normalize();
            return center + direction * radius;
        }
        /// <summary>
        /// Returns the closest point on the circle or the point itself if it is inside the circle.
        /// </summary>
        [Pure]
        public Vector2 ClosestPoint(Vector2 _point)
        {
            Vector2 direction = _point - center;
            if (direction.sqrMagnitude <= radius * radius)
                return _point;
            
            direction.Normalize();
            return center + direction * radius;
        }
        /// <summary>
        /// Is the point inside the circle?
        /// </summary>
        [Pure]
        public bool Contains(Vector2 _point)
        {
            return Vector2.SqrMagnitude(_point - center) <= radius * radius;
        }
        /// <summary>
        /// Is the line inside the circle?
        /// </summary>
        [Pure]
        public bool Contains(LineSeg2D _line)
        {
            return Contains(_line.start) && Contains(_line.end);
        }
        /// <summary>
        /// Is the given circle inside the circle?
        /// </summary>
        [Pure]
        public bool Contains(Circle _circle)
        {
            return Vector2.SqrMagnitude(_circle.center - center) <= (radius - _circle.radius) * (radius - _circle.radius);
        }
        /// <summary>
        /// Returns a random point inside the circle.
        /// </summary>
        [Pure]
        public Vector2 RandomPoint()
        {
            return Random.insideUnitCircle * radius + center;
        }
        public bool Equals(Circle _other)
        {
            return center.Equals(_other.center) && radius.Equals(_other.radius);
        }
        public override bool Equals(object _obj)
        {
            if (_obj is Circle other)
                return Equals(other);
            
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(center, BitConverter.SingleToInt32Bits(radius));
        }
    }
}