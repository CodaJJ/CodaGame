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
    /// A 3D line segment shape.
    /// </summary>
    [Serializable]
    public struct LineSeg3D : IEquatable<LineSeg3D>
    {
        public static bool operator ==(LineSeg3D _lhs, LineSeg3D _rhs) => _lhs.start == _rhs.start && _lhs.end == _rhs.end;
        public static bool operator !=(LineSeg3D _lhs, LineSeg3D _rhs) => !(_lhs == _rhs);
        
    
        public Vector3 start;
        public Vector3 end;
        
        
        public LineSeg3D(Vector3 _start, Vector3 _end)
        {
            start = _start;
            end = _end;
        }
        public LineSeg3D(LineSeg3D _lineSeg)
            : this(_lineSeg.start, _lineSeg.end)
        {
        }
        
        
        public float length { get { return Vector3.Distance(start, end); } }
        public Vector3 direction { get { return (end - start).normalized; } }
        public Vector3 vector { get { return end - start; } }
        public Vector3 center { get { return (start + end) / 2; } }
        
        
        public void Set(Vector3 _start, Vector3 _end)
        {
            start = _start;
            end = _end;
        }
        public void Set(LineSeg3D _lineSeg)
        {
            start = _lineSeg.start;
            end = _lineSeg.end;
        }
        /// <summary>
        /// Returns the closest point on the line to the given point.
        /// </summary>
        [Pure]
        public Vector3 ClosestPoint(Vector3 _point)
        {
            Vector3 lineVector = vector;
            float t = Vector3.Dot(_point - start, lineVector) / lineVector.sqrMagnitude;
            t = Mathf.Clamp01(t);
            return start + t * lineVector;
        }
        /// <summary>
        /// Is the point on the line segment within the given tolerance?
        /// </summary>
        [Pure]
        public bool Contains(Vector3 _point, float _tolerance = Values.MediumPrecisionTolerance)
        {
            Vector3 lineVector = vector;
            Vector3 pointVector = _point - start;
    
            float lineSqrMagnitude = lineVector.sqrMagnitude;
            float t = Vector3.Dot(pointVector, lineVector) / lineSqrMagnitude;

            if (t is < 0f or > 1f)
                return false;

            Vector3 projection = start + lineVector * t;
            float sqrDist = (projection - _point).sqrMagnitude;

            return sqrDist <= _tolerance * _tolerance;
        }
        /// <summary>
        /// Returns a random point on the line segment.
        /// </summary>
        [Pure]
        public Vector3 RandomPoint()
        {
            return Random.Range(0f, 1f) * vector + start;
        }
        public bool Equals(LineSeg3D _other)
        {
            return start.Equals(_other.start) && end.Equals(_other.end);
        }
        public override bool Equals(object _obj)
        {
            if (_obj is LineSeg3D lineSeg)
                return Equals(lineSeg);
            
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(start, end);
        }
    }
}