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
    /// A Sphere shape.
    /// </summary>
    [Serializable]
    public struct Sphere : IEquatable<Sphere>
    {
        public static bool operator ==(Sphere _lhs, Sphere _rhs) => _lhs.center == _rhs.center && Mathf.Approximately(_lhs.radius, _rhs.radius);
        public static bool operator !=(Sphere _lhs, Sphere _rhs) => !(_lhs == _rhs);
        
    
        public Vector3 center;
        public float radius;
        
        
        public Sphere(Vector3 _center, float _radius)
        {
            center = _center;
            radius = _radius;
        }
        
        
        public float volume { get { return 4f / 3f * Values.Pi * radius * radius * radius; } }
        
        
        public void Set(Vector3 _center, float _radius)
        {
            center = _center;
            radius = _radius;
        }
        /// <summary>
        /// Returns the closest point on the sphere to the given point.
        /// </summary>
        [Pure]
        public Vector3 ClosestPointOn(Vector3 _point)
        {
            Vector3 direction = _point - center;
            direction.Normalize();
            return center + direction * radius;
        }
        /// <summary>
        /// Returns the closest point on the circle or the point itself if it is inside the circle.
        /// </summary>
        [Pure]
        public Vector3 ClosestPoint(Vector3 _point)
        {
            Vector3 direction = _point - center;
            if (direction.sqrMagnitude <= radius * radius)
                return _point;
            
            direction.Normalize();
            return center + direction * radius;
        }
        /// <summary>
        /// Is the point inside the sphere?
        /// </summary>
        [Pure]
        public bool Contains(Vector3 _point)
        {
            return Vector3.SqrMagnitude(_point - center) <= radius * radius;
        }
        /// <summary>
        /// Is the line inside the sphere?
        /// </summary>
        [Pure]
        public bool Contains(LineSeg3D _line)
        {
            return Contains(_line.start) && Contains(_line.end);
        }
        /// <summary>
        /// Is the given sphere inside the sphere?
        /// </summary>
        [Pure]
        public bool Contains(Sphere _sphere)
        {
            return Vector3.SqrMagnitude(_sphere.center - center) <= (radius - _sphere.radius) * (radius - _sphere.radius);
        }
        /// <summary>
        /// Returns a random point inside the sphere.
        /// </summary>
        [Pure]
        public Vector3 RandomPoint()
        {
            return Random.insideUnitSphere * radius + center;
        }
        public bool Equals(Sphere _other)
        {
            return center.Equals(_other.center) && radius.Equals(_other.radius);
        }
        public override bool Equals(object _obj)
        {
            if (_obj is Sphere other)
                return Equals(other);
            
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(center, BitConverter.SingleToInt32Bits(radius));
        }
    }
}