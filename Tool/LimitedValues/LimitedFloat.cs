// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A limited float value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It's kind of a value that can be limited by a range,
    /// and range's soft border size will allow the value break the limit a little bit.
    /// </para>
    /// </remarks>
    public class LimitedFloat : _ALimitedValue<float, _IValueRange<float>>
    {
        /// <summary>
        /// Create a limited float value
        /// </summary>
        /// <param name="_value">Initial value.</param>
        /// <param name="_valueRange">The range that will limit the value.</param>
        public LimitedFloat(float _value, _IValueRange<float> _valueRange) 
            : base(_value, _valueRange)
        {
        }
        /// <summary>
        /// Create a limited float value
        /// </summary>
        /// <remarks>
        /// <para>Soft border size will allow the value break the limit a little bit, and recover to the range after a delay.</para>
        /// </remarks>
        /// <param name="_value">Initial value.</param>
        /// <param name="_valueRange">The range that will limit the value.</param>
        /// <param name="_softBorderSize">How much the value can break the limit.</param>
        /// <param name="_runType">Determine where the recover task will run.</param>
        /// <param name="_recoverDelay">How many frames the value will recover to the range after it's out of the range and no more changes.</param>
        public LimitedFloat(float _value, _IValueRange<float> _valueRange, float _softBorderSize, UpdateType _runType = UpdateType.Update, int _recoverDelay = 1) 
            : base(_value, _valueRange, _softBorderSize, _runType, _recoverDelay)
        {
        }
        

        protected override float AddValue(float _value, float _deltaValue) { return _value + _deltaValue; }
        protected override float SubtractValue(float _value, float _deltaValue) { return _value - _deltaValue; }
        protected override float MultiplyValue(float _value, float _multiplier) { return _value * _multiplier; }
        protected override float Normalize(float _value) { return Mathf.Sign(_value); }
        protected override float GetMagnitude(float _value) { return Mathf.Abs(_value); }
        protected override float SmoothDamp(float _current, float _target, ref float _currentVelocity, float _smoothTime, float _deltaTime)
        {
            return Mathf.SmoothDamp(_current, _target, ref _currentVelocity, _smoothTime, float.PositiveInfinity, _deltaTime);
        }
    }
}