// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A limited Vector2 value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It's kind of a value that can be limited by a range,
    /// and range's soft border size will allow the value break the limit a little bit.
    /// </para>
    /// </remarks>
    public class LimitedVector2 : _ALimitedValue<Vector2, _IValueRange<Vector2>>
    {
        /// <summary>
        /// Create a limited Vector2 value
        /// </summary>
        /// <param name="_value">Initial value.</param>
        /// <param name="_valueRange">The range that will limit the value.</param>
        public LimitedVector2(Vector2 _value, _IValueRange<Vector2> _valueRange) 
            : base(_value, _valueRange)
        {
        }
        /// <summary>
        /// Create a limited Vector2 value
        /// </summary>
        /// <remarks>
        /// <para>Soft border size will allow the value break the limit a little bit, and recover to the range after a delay.</para>
        /// </remarks>
        /// <param name="_value">Initial value.</param>
        /// <param name="_valueRange">The range that will limit the value.</param>
        /// <param name="_softBorderSize">How much the value can break the limit.</param>
        /// <param name="_runType">Determine where the recover task will run.</param>
        /// <param name="_recoverDelay">How many frames the value will recover to the range after it's out of the range and no more changes.</param>
        public LimitedVector2(Vector2 _value, _IValueRange<Vector2> _valueRange, float _softBorderSize, UpdateType _runType = UpdateType.Update, int _recoverDelay = 1) 
            : base(_value, _valueRange, _softBorderSize, _runType, _recoverDelay)
        {
        }

        
        protected override Vector2 AddValue(Vector2 _value, Vector2 _deltaValue) { return _value + _deltaValue; }
        protected override Vector2 SubtractValue(Vector2 _value, Vector2 _deltaValue) { return _value - _deltaValue; }
        protected override Vector2 MultiplyValue(Vector2 _value, float _multiplier) { return _value * _multiplier; }
        protected override Vector2 Normalize(Vector2 _value) { return _value.normalized; }
        protected override float GetMagnitude(Vector2 _value) { return _value.magnitude; }
        protected override Vector2 SmoothDamp(Vector2 _current, Vector2 _target, ref Vector2 _currentVelocity, float _smoothTime)
        {
            return Vector2.SmoothDamp(_current, _target, ref _currentVelocity, _smoothTime);
        }
    }
}