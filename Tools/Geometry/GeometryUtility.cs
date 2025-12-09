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
        /// <summary>
        /// Try parse Vector2 from string "(x,y)"
        /// </summary>
        [Pure]
        public static bool TryParse(string _str, out Vector2 _result)
        {
            _result = Vector2.zero;

            if (string.IsNullOrWhiteSpace(_str))
                return false;

            _str = _str.Trim();

            // Remove parentheses if present
            if (_str.StartsWith("(") && _str.EndsWith(")"))
                _str = _str.Substring(1, _str.Length - 2);

            string[] parts = _str.Split(',');
            if (parts.Length != 2)
                return false;

            if (!float.TryParse(parts[0].Trim(), out float x))
                return false;

            if (!float.TryParse(parts[1].Trim(), out float y))
                return false;

            _result = new Vector2(x, y);
            return true;
        }
        /// <summary>
        /// Try parse Vector3 from string "(x,y,z)"
        /// </summary>
        [Pure]
        public static bool TryParse(string _str, out Vector3 _result)
        {
            _result = Vector3.zero;

            if (string.IsNullOrWhiteSpace(_str))
                return false;

            _str = _str.Trim();

            // Remove parentheses if present
            if (_str.StartsWith("(") && _str.EndsWith(")"))
                _str = _str.Substring(1, _str.Length - 2);

            string[] parts = _str.Split(',');
            if (parts.Length != 3)
                return false;

            if (!float.TryParse(parts[0].Trim(), out float x))
                return false;

            if (!float.TryParse(parts[1].Trim(), out float y))
                return false;

            if (!float.TryParse(parts[2].Trim(), out float z))
                return false;

            _result = new Vector3(x, y, z);
            return true;
        }
        /// <summary>
        /// Try parse Vector4 from string "(x,y,z,w)"
        /// </summary>
        [Pure]
        public static bool TryParse(string _str, out Vector4 _result)
        {
            _result = Vector4.zero;

            if (string.IsNullOrWhiteSpace(_str))
                return false;

            _str = _str.Trim();

            // Remove parentheses if present
            if (_str.StartsWith("(") && _str.EndsWith(")"))
                _str = _str.Substring(1, _str.Length - 2);

            string[] parts = _str.Split(',');
            if (parts.Length != 4)
                return false;

            if (!float.TryParse(parts[0].Trim(), out float x))
                return false;

            if (!float.TryParse(parts[1].Trim(), out float y))
                return false;

            if (!float.TryParse(parts[2].Trim(), out float z))
                return false;

            if (!float.TryParse(parts[3].Trim(), out float w))
                return false;

            _result = new Vector4(x, y, z, w);
            return true;
        }
        /// <summary>
        /// Try parse Vector2Int from string "(x,y)"
        /// </summary>
        [Pure]
        public static bool TryParse(string _str, out Vector2Int _result)
        {
            _result = Vector2Int.zero;

            if (string.IsNullOrWhiteSpace(_str))
                return false;

            _str = _str.Trim();

            // Remove parentheses if present
            if (_str.StartsWith("(") && _str.EndsWith(")"))
                _str = _str.Substring(1, _str.Length - 2);

            string[] parts = _str.Split(',');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0].Trim(), out int x))
                return false;

            if (!int.TryParse(parts[1].Trim(), out int y))
                return false;

            _result = new Vector2Int(x, y);
            return true;
        }
        /// <summary>
        /// Try parse Vector3Int from string "(x,y,z)"
        /// </summary>
        [Pure]
        public static bool TryParse(string _str, out Vector3Int _result)
        {
            _result = Vector3Int.zero;

            if (string.IsNullOrWhiteSpace(_str))
                return false;

            _str = _str.Trim();

            // Remove parentheses if present
            if (_str.StartsWith("(") && _str.EndsWith(")"))
                _str = _str.Substring(1, _str.Length - 2);

            string[] parts = _str.Split(',');
            if (parts.Length != 3)
                return false;

            if (!int.TryParse(parts[0].Trim(), out int x))
                return false;

            if (!int.TryParse(parts[1].Trim(), out int y))
                return false;

            if (!int.TryParse(parts[2].Trim(), out int z))
                return false;
            
            _result = new Vector3Int(x, y, z);
            return true;
        }
        /// <summary>
        /// Try parse Rect from string "(x,y,width,height)"
        /// </summary>
        [Pure]
        public static bool TryParse(string _str, out Rect _result)
        {
            _result = Rect.zero;

            if (string.IsNullOrWhiteSpace(_str))
                return false;

            _str = _str.Trim();

            // Remove parentheses if present
            if (_str.StartsWith("(") && _str.EndsWith(")"))
                _str = _str.Substring(1, _str.Length - 2);

            string[] parts = _str.Split(',');
            if (parts.Length != 4)
                return false;

            if (!float.TryParse(parts[0].Trim(), out float x))
                return false;

            if (!float.TryParse(parts[1].Trim(), out float y))
                return false;

            if (!float.TryParse(parts[2].Trim(), out float width))
                return false;

            if (!float.TryParse(parts[3].Trim(), out float height))
                return false;

            _result = new Rect(x, y, width, height);
            return true;
        }
        /// <summary>
        /// Try parse Bounds from string "(centerX,centerY,centerZ),(sizeX,sizeY,sizeZ)"
        /// </summary>
        [Pure]
        public static bool TryParse(string _str, out Bounds _result)
        {
            _result = new Bounds();

            if (string.IsNullOrWhiteSpace(_str))
                return false;

            _str = _str.Trim();

            // Find the two groups: (center) and (size)
            int firstOpen = _str.IndexOf('(');
            int firstClose = _str.IndexOf(')');
            int secondOpen = _str.IndexOf('(', firstClose + 1);
            int secondClose = _str.IndexOf(')', secondOpen + 1);

            if (firstOpen == -1 || firstClose == -1 || secondOpen == -1 || secondClose == -1)
                return false;

            string centerStr = _str.Substring(firstOpen + 1, firstClose - firstOpen - 1);
            string sizeStr = _str.Substring(secondOpen + 1, secondClose - secondOpen - 1);

            // Parse center
            string[] centerParts = centerStr.Split(',');
            if (centerParts.Length != 3)
                return false;

            if (!float.TryParse(centerParts[0].Trim(), out float centerX))
                return false;

            if (!float.TryParse(centerParts[1].Trim(), out float centerY))
                return false;

            if (!float.TryParse(centerParts[2].Trim(), out float centerZ))
                return false;

            // Parse size
            string[] sizeParts = sizeStr.Split(',');
            if (sizeParts.Length != 3)
                return false;

            if (!float.TryParse(sizeParts[0].Trim(), out float sizeX))
                return false;

            if (!float.TryParse(sizeParts[1].Trim(), out float sizeY))
                return false;

            if (!float.TryParse(sizeParts[2].Trim(), out float sizeZ))
                return false;

            _result = new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ));
            return true;
        }
    }
}