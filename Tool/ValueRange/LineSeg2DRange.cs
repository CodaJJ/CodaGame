// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A range defined by a <see cref="LineSeg2D"/>.
    /// </summary>
    [Serializable]
    public class LineSeg2DRange : _IValueRange<Vector2>
    {
        public LineSeg2D lineSeg2D;
        
        
        public bool IsInRange(Vector2 _value)
        {
            return lineSeg2D.Contains(_value);
        }
        public Vector2 ClampValue(Vector2 _value)
        {
            return lineSeg2D.ClosestPoint(_value);
        }
        public Vector2 RandomValue()
        {
            return lineSeg2D.RandomPoint();
        }
    }
}