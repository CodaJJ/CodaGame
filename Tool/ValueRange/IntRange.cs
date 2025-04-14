// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodaGame
{
    /// <summary>
    /// A range defined by a minimum and maximum value.
    /// </summary>
    [Serializable]
    public struct IntRange : _IValueRange<int>
    {
        public int min;
        public int max;
        
        
        public bool IsInRange(int _value)
        {
            return _value >= min && _value <= max;
        }
        public int ClampValue(int _value)
        {
            return Mathf.Clamp(_value, min, max);
        }
        public int RandomValue()
        {
            return Random.Range(min, max + 1);
        }
    }
}