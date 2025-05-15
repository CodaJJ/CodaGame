// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A limited int value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It's kind of a value that can be limited by a range,
    /// and range's soft border size will allow the value break the limit a little bit.
    /// </para>
    /// </remarks>
    public class LimitedInt : _ALimitedValue<int, _IValueRange<int>>
    {
        /// <summary>
        /// Create a limited float value
        /// </summary>
        /// <param name="_value">Initial value.</param>
        /// <param name="_valueRange">The range that will limit the value.</param>
        public LimitedInt(int _value, _IValueRange<int> _valueRange) 
            : base(_value, _valueRange)
        {
        }

        
        protected override int AddValue(int _value, int _deltaValue) { return _value + _deltaValue; }
        protected override int SubtractValue(int _value, int _deltaValue) { return _value - _deltaValue; }
        protected override int MultiplyValue(int _value, float _multiplier) { return (int)(_value * _multiplier); }
        protected override int Normalize(int _value)
        {
            return _value switch
            {
                > 0 => 1,
                < 0 => -1,
                _ => 0
            };
        }
        protected override float GetMagnitude(int _value) { return Mathf.Abs(_value); }
        protected override int SmoothDamp(int _current, int _target, ref int _currentVelocity, float _smoothTime, float _deltaTime)
        {
            return _target;
        }
    }
}