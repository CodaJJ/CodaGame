// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A range defined by a <see cref="Rect"/>.
    /// </summary>
    [Serializable]
    public class RectRange : _IValueRange<Vector2>
    {
        public Rect rect;
        
        
        public bool IsInRange(Vector2 _value)
        {
            return rect.Contains(_value);
        }
        public Vector2 ClampValue(Vector2 _value)
        {
            return rect.ClosestPoint(_value);
        }
        public Vector2 RandomValue()
        {
            return rect.RandomPoint();
        }
    }
}