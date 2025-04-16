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
    public class FloatRange : _IValueRange<float>
    {
        public float min;
        public float max;
        
        
        public FloatRange(float _min, float _max)
        {
            min = _min;
            max = _max;
        }
        
        
        public bool IsInRange(float _value)
        {
            return _value >= min && _value <= max;
        }
        public float ClampValue(float _value)
        {
            return Mathf.Clamp(_value, min, max);
        }
        public float RandomValue()
        {
            return Random.Range(min, max);
        }
    }
}