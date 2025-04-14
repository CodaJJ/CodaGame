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
    public class LimitedVector3 : _ALimitedValue<Vector3, _IValueRange<Vector3>>
    {
        /// <summary>
        /// Create a limited Vector3 value
        /// </summary>
        /// <param name="_value">Initial value.</param>
        /// <param name="_valueRange">The range that will limit the value.</param>
        public LimitedVector3(Vector3 _value, _IValueRange<Vector3> _valueRange) 
            : base(_value, _valueRange)
        {
        }
        /// <summary>
        /// Create a limited Vector3 value
        /// </summary>
        /// <remarks>
        /// <para>Soft border size will allow the value break the limit a little bit, and recover to the range after a delay.</para>
        /// </remarks>
        /// <param name="_value">Initial value.</param>
        /// <param name="_valueRange">The range that will limit the value.</param>
        /// <param name="_softBorderSize">How much the value can break the limit.</param>
        /// <param name="_runType">Determine where the recover task will run.</param>
        /// <param name="_recoverDelay">How many frames the value will recover to the range after it's out of the range and no more changes.</param>
        public LimitedVector3(Vector3 _value, _IValueRange<Vector3> _valueRange, float _softBorderSize, UpdateType _runType = UpdateType.Update, int _recoverDelay = 1) 
            : base(_value, _valueRange, _softBorderSize, _runType, _recoverDelay)
        {
        }
        
        
        protected override Vector3 AddValue(Vector3 _value, Vector3 _deltaValue) { return _value + _deltaValue; }
        protected override Vector3 SubtractValue(Vector3 _value, Vector3 _deltaValue) { return _value - _deltaValue; }
        protected override Vector3 MultiplyValue(Vector3 _value, float _multiplier) { return _value * _multiplier; }
        protected override Vector3 Normalize(Vector3 _value) { return _value.normalized; }
        protected override float GetMagnitude(Vector3 _value) { return _value.magnitude; }
        protected override Vector3 SmoothDamp(Vector3 _current, Vector3 _target, ref Vector3 _currentVelocity, float _smoothTime)
        {
            return Vector3.SmoothDamp(_current, _target, ref _currentVelocity, _smoothTime);
        }
    }
}