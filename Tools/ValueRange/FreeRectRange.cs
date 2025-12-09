// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A range defined by a <see cref="FreeRect"/>.
    /// </summary>
    [Serializable]
    public class FreeRectRange : _IValueRange<Vector2>
    {
        public FreeRect freeRect;
        
        
        public FreeRectRange(Vector2 _center, Vector2 _size, float _radian)
        {
            freeRect = new FreeRect(_center, _size, _radian);
        }
        public FreeRectRange(Rect _rect, float _radian)
        {
            freeRect = new FreeRect(_rect, _radian);
        }
        
        
        public bool IsInRange(Vector2 _value)
        {
            return freeRect.Contains(_value);
        }
        public Vector2 ClampValue(Vector2 _value)
        {
            return freeRect.ClosestPoint(_value);
        }
        public Vector2 RandomValue()
        {
            return freeRect.RandomPoint();
        }
    }
}