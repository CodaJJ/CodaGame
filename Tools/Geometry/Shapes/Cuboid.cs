// Copyright (c) 2025 Coda
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
    /// A cuboid shape.
    /// </summary>
    [Serializable]
    public struct Cuboid : IEquatable<Cuboid>
    {
        public static bool operator ==(Cuboid _lhs, Cuboid _rhs) => _lhs.center == _rhs.center && _lhs.halfSize == _rhs.halfSize && _lhs.rotation == _rhs.rotation;
        public static bool operator !=(Cuboid _lhs, Cuboid _rhs) => !(_lhs == _rhs);


        public Vector3 center;
        public Vector3 halfSize;
        public Quaternion rotation;
        
        
        public Cuboid(Vector3 _center, Vector3 _size, Quaternion _rotation)
        {
            center = _center;
            halfSize = _size * 0.5f;
            rotation = _rotation;
        }
        public Cuboid(Vector3 _center, Vector3 _size, Vector3 _eulerRotation)
            : this(_center, _size, Quaternion.Euler(_eulerRotation))
        {
        }
        public Cuboid(Bounds _bounds, Vector3 _eulerRotation)
            : this(_bounds.center, _bounds.size, _eulerRotation)
        {
        }
        public Cuboid(Bounds _bounds, Quaternion _rotation)
            : this(_bounds.center, _bounds.size, _rotation)
        {
        }
        
        
        public Vector3 size { get { return halfSize * 2f; } set { halfSize = value * 0.5f; } }
        public Quaternion inverseRotation { get { return Quaternion.Inverse(rotation); } }
        public float volume { get { return halfSize.x * halfSize.y * halfSize.z * 8; } }
        
        
        public void Set(Vector3 _center, Vector3 _size, Quaternion _rotation)
        {
            center = _center;
            halfSize = _size * 0.5f;
            rotation = _rotation;
        }
        public void Set(Vector3 _center, Vector3 _size, Vector3 _eulerRotation)
        {
            Set(_center, _size, Quaternion.Euler(_eulerRotation));
        }
        public void Set(Bounds _bounds, Vector3 _eulerRotation)
        {
            Set(_bounds.center, _bounds.size, _eulerRotation);
        }
        public void Set(Bounds _bounds, Quaternion _rotation)
        {
            Set(_bounds.center, _bounds.size, _rotation);
        }
        /// <summary>
        /// Returns the closest point on the cuboid to the given point.
        /// </summary>
        [Pure]
        public Vector3 ClosestPointOn(Vector3 _point)
        {
            Vector3 localPoint = inverseRotation * (_point - center);
            bool isInside =
                localPoint.x >= -halfSize.x && localPoint.x <= halfSize.x &&
                localPoint.y >= -halfSize.y && localPoint.y <= halfSize.y &&
                localPoint.z >= -halfSize.z && localPoint.z <= halfSize.z;

            Vector3 pointOn = localPoint;
            if (!isInside)
                pointOn = Vector3.Max(-halfSize, Vector3.Min(halfSize, localPoint));
            else
            {
                Vector3 distances = new Vector3(
                    halfSize.x - Mathf.Abs(localPoint.x),
                    halfSize.y - Mathf.Abs(localPoint.y),
                    halfSize.z - Mathf.Abs(localPoint.z)
                );

                if (distances.x < distances.y && distances.x < distances.z)
                    pointOn.x = halfSize.x * Mathf.Sign(localPoint.x);
                else if (distances.y < distances.z)
                    pointOn.y = halfSize.y * Mathf.Sign(localPoint.y);
                else
                    pointOn.z = halfSize.z * Mathf.Sign(localPoint.z);
            }

            return center + rotation * pointOn;
        }
        /// <summary>
        /// Returns the closest point on the cuboid or the point itself if it is inside the cuboid.
        /// </summary>
        [Pure]
        public Vector3 ClosestPoint(Vector3 _point)
        {
            Vector3 localPoint = inverseRotation * (_point - center);
            Vector3 clampedPoint = Vector3.Max(-halfSize, Vector3.Min(halfSize, localPoint));
            return center + rotation * clampedPoint;
        }
        /// <summary>
        /// Is the point inside the cuboid?
        /// </summary>
        [Pure]
        public bool Contains(Vector3 _point)
        {
            Vector3 localPoint = inverseRotation * (_point - center);
            return localPoint.x >= -halfSize.x && localPoint.x <= halfSize.x &&
                   localPoint.y >= -halfSize.y && localPoint.y <= halfSize.y &&
                   localPoint.z >= -halfSize.z && localPoint.z <= halfSize.z;   
        }
        /// <summary>
        /// Is the line inside the cuboid?
        /// </summary>
        [Pure]
        public bool Contains(LineSeg3D _line)
        {
            return Contains(_line.start) && Contains(_line.end);
        }
        /// <summary>
        /// Returns a random point inside the cuboid.
        /// </summary>
        [Pure]
        public Vector3 RandomPoint()
        {
            Vector3 point = new Vector3(
                Random.Range(-halfSize.x, halfSize.x),
                Random.Range(-halfSize.y, halfSize.y),
                Random.Range(-halfSize.z, halfSize.z)
            );
            return center + rotation * point;
        }
        public bool Equals(Cuboid _other)
        {
            return center.Equals(_other.center) && halfSize.Equals(_other.halfSize) && rotation.Equals(_other.rotation);
        }
        public override bool Equals(object _obj)
        {
            if (_obj is Cuboid other)
                return Equals(other);

            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(center, halfSize, rotation);
        }
    }
}