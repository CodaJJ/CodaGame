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
    /// A 2D line segment shape.
    /// </summary>
    [Serializable]
    public struct LineSeg2D : IEquatable<LineSeg2D>
    {
        public static bool operator ==(LineSeg2D _lhs, LineSeg2D _rhs) => _lhs.start == _rhs.start && _lhs.end == _rhs.end;
        public static bool operator !=(LineSeg2D _lhs, LineSeg2D _rhs) => !(_lhs == _rhs);
        
        
        public Vector2 start;
        public Vector2 end;
        
        
        public LineSeg2D(Vector2 _start, Vector2 _end)
        {
            start = _start;
            end = _end;
        }
        public LineSeg2D(LineSeg2D _lineSeg)
            : this(_lineSeg.start, _lineSeg.end)
        {
        }
        
        
        public float length { get { return Vector2.Distance(start, end); } }
        public Vector2 direction { get { return (end - start).normalized; } }
        public Vector2 vector { get { return end - start; } }
        public Vector2 center { get { return (start + end) / 2; } }


        public void Set(Vector2 _start, Vector2 _end)
        {
            start = _start;
            end = _end;
        }
        public void Set(LineSeg2D _lineSeg)
        {
            start = _lineSeg.start;
            end = _lineSeg.end;
        }
        /// <summary>
        /// Returns the closest point on the line to the given point.
        /// </summary>
        [Pure]
        public Vector2 ClosestPoint(Vector2 _point)
        {
            Vector2 lineVector = vector;
            float t = Vector2.Dot(_point - start, lineVector) / lineVector.sqrMagnitude;
            t = Mathf.Clamp01(t);
            return start + lineVector * t;
        }
        /// <summary>
        /// Is the point on the line segment within the given tolerance?
        /// </summary>
        [Pure]
        public bool Contains(Vector2 _point, float _tolerance = Values.MediumPrecisionTolerance)
        {
            Vector2 lineVector = vector;
            Vector2 pointVector = _point - start;
    
            float lineSqrMagnitude = lineVector.sqrMagnitude;
            float t = Vector2.Dot(pointVector, lineVector) / lineSqrMagnitude;

            if (t is < 0f or > 1f)
                return false;

            Vector2 projection = start + lineVector * t;
            float sqrDist = (projection - _point).sqrMagnitude;

            return sqrDist <= _tolerance * _tolerance;
        }
        /// <summary>
        /// Returns a random point on the line segment.
        /// </summary>
        [Pure]
        public Vector2 RandomPoint()
        {
            return Vector2.Lerp(start, end, Random.Range(0f, 1f));
        }
        public bool Equals(LineSeg2D _other)
        {
            return start.Equals(_other.start) && end.Equals(_other.end);
        }
        public override bool Equals(object _obj)
        {
            if (_obj is LineSeg2D other)
                return Equals(other);
            
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(start, end);
        }
    }
}