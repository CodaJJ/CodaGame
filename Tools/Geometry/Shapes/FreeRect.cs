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
    /// A rectangle shape.
    /// </summary>
    /// <remarks>
    /// <para>Not like Unity's Rect, this rectangle is not axis-aligned.</para>
    /// </remarks>
    [Serializable]
    public struct FreeRect : IEquatable<FreeRect>
    {
        public static bool operator ==(FreeRect _lhs, FreeRect _rhs) => _lhs.center == _rhs.center && _lhs.halfSize == _rhs.halfSize && Mathf.Approximately(_lhs.rotation, _rhs.rotation);
        public static bool operator !=(FreeRect _lhs, FreeRect _rhs) => !(_lhs == _rhs);


        public Vector2 center;
        public Vector2 halfSize;
        [Radian]
        public float rotation;
        
        
        public FreeRect(Vector2 _center, Vector2 _size, float _radian)
        {
            center = _center;
            halfSize = _size * 0.5f;
            rotation = _radian;
        }
        public FreeRect(Rect _rect, float _radian)
            : this(_rect.center, _rect.size, _radian)
        {
        }
        
        
        public Vector2 size { get { return halfSize * 2f; } set { halfSize = value * 0.5f; } }
        public float area { get { return halfSize.x * halfSize.y * 4f; } }
        public float perimeter { get { return (halfSize.x + halfSize.y) * 4f; } }
        public float aspectRatio { get { return halfSize.x / halfSize.y; } }
        public float diagonal { get { return halfSize.magnitude * 2f; } }
        public float rotationDegree { get { return rotation * Mathf.Rad2Deg; } }
        
        
        public void Set(Vector2 _center, Vector2 _size, float _radian)
        {
            center = _center;
            halfSize = _size * 0.5f;
            rotation = _radian;
        }
        public void Set(Rect _rect, float _radian)
        {
            center = _rect.center;
            halfSize = _rect.size * 0.5f;
            rotation = _radian;
        }
        /// <summary>
        /// Returns the closest point on the rectangle to the given point.
        /// </summary>
        [Pure]
        public Vector2 ClosestPointOn(Vector2 _point)
        {
            Vector2 localPoint = GeometryUtility.RotatePoint(_point - center, -rotation);
            bool isInside =
                localPoint.x >= -halfSize.x && localPoint.x <= halfSize.x &&
                localPoint.y >= -halfSize.y && localPoint.y <= halfSize.y;

            Vector2 pointOn = localPoint;
            if (!isInside)
                pointOn = Vector2.Max(-halfSize, Vector2.Min(halfSize, localPoint));
            else
            {
                Vector2 distances = new Vector2(
                    halfSize.x - Mathf.Abs(localPoint.x),
                    halfSize.y - Mathf.Abs(localPoint.y)
                );

                if (distances.x < distances.y)
                    pointOn.x = halfSize.x * Mathf.Sign(localPoint.x);
                else
                    pointOn.y = halfSize.y * Mathf.Sign(localPoint.y);
            }

            return center + GeometryUtility.RotatePoint(pointOn, rotation);
        }
        /// <summary>
        /// Returns the closest point on the rectangle or the point itself if it is inside the rectangle.
        /// </summary>
        [Pure]
        public Vector2 ClosestPoint(Vector2 _point)
        {
            Vector2 localPoint = GeometryUtility.RotatePoint(_point - center, -rotation);
            Vector2 clampedPoint = Vector2.Max(-halfSize, Vector2.Min(halfSize, localPoint));
            return center + GeometryUtility.RotatePoint(clampedPoint, rotation);
        }
        /// <summary>
        /// Is the point inside the rectangle?
        /// </summary>
        public bool Contains(Vector2 _point)
        {
            Vector2 localPoint = GeometryUtility.RotatePoint(_point - center, -rotation);
            return
                localPoint.x >= -halfSize.x && localPoint.x <= halfSize.x &&
                localPoint.y >= -halfSize.y && localPoint.y <= halfSize.y;
        }
        /// <summary>
        /// Is the line inside the rectangle?
        /// </summary>
        public bool Contains(LineSeg2D _line)
        {
            return Contains(_line.start) && Contains(_line.end);
        }
        /// <summary>
        /// Returns a random point inside the rectangle.
        /// </summary>
        [Pure]
        public Vector2 RandomPoint()
        {
            Vector2 point = new Vector2(
                Random.Range(-halfSize.x, halfSize.x),
                Random.Range(-halfSize.y, halfSize.y)
            );
            return center + GeometryUtility.RotatePoint(point, rotation);
        }
        public bool Equals(FreeRect _other)
        {
            return center.Equals(_other.center) && halfSize.Equals(_other.halfSize) && rotation.Equals(_other.rotation);
        }
        public override bool Equals(object _obj)
        {
            if (_obj is FreeRect other)
                return Equals(other);

            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(center, halfSize, BitConverter.SingleToInt32Bits(rotation));
        }
    }
}