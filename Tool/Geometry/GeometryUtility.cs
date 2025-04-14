// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    public static class GeometryUtility
    {
        [Pure]
        public static float Cross(this Vector2 _a, Vector2 _b)
        {
            return _a.x * _b.y - _a.y * _b.x;
        }
        [Pure]
        public static Vector3 Cross(this Vector3 _a, Vector3 _b)
        {
            return Vector3.Cross(_a, _b);
        }
        [Pure]
        public static Vector3 RandomPoint(this Bounds _bounds)
        {
            return new Vector3(
                Random.Range(_bounds.min.x, _bounds.max.x),
                Random.Range(_bounds.min.y, _bounds.max.y),
                Random.Range(_bounds.min.z, _bounds.max.z)
            );
        }
        [Pure]
        public static Vector2 RandomPoint(this Rect _rect)
        {
            return new Vector2(
                Random.Range(_rect.xMin, _rect.xMax),
                Random.Range(_rect.yMin, _rect.yMax)
            );
        }
        [Pure]
        public static Vector2 ClosestPoint(this Rect _rect, Vector2 _point)
        {
            return new Vector2(
                Mathf.Clamp(_point.x, _rect.xMin, _rect.xMax),
                Mathf.Clamp(_point.y, _rect.yMin, _rect.yMax)
            );
        }
        [Pure]
        public static Vector2 RotatePoint(Vector2 _point, float _radians)
        {
            float cosTheta = Mathf.Cos(_radians);
            float sinTheta = Mathf.Sin(_radians);

            float xNew = _point.x * cosTheta - _point.y * sinTheta;
            float yNew = _point.x * sinTheta + _point.y * cosTheta;

            return new Vector2(xNew, yNew);
        }
    }
}