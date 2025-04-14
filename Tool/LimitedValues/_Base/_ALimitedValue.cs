// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame.Base
{
    /// <summary>
    /// A base class for limited values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It's kind of a value that can be limited by a range,
    /// and range's soft border size will allow the value break the limit a little bit.
    /// </para>
    /// </remarks>
    public abstract class _ALimitedValue<T_VALUE, T_RANGE>
        where T_VALUE : struct
        where T_RANGE : _IValueRange<T_VALUE>
    {
        private readonly T_RANGE _m_valueRange;
        private readonly float _m_softBorderSize;
        // The task that will recover the value to the range when it's out of the range.
        private readonly TaskHandle _m_recoverTask;
        // The value will recover to the range after this delay, if it's out of the range and no more changes.
        private readonly int _m_recoverDelay;
        
        private T_VALUE _m_value;
        // A counter to count the recover delay.
        private int _m_recoverCounter;
        // It's use SmoothDamp to recover the value, so it needs a velocity.
        private T_VALUE _m_smoothDampVelocity;


        /// <summary>
        /// Create a limited value
        /// </summary>
        /// <param name="_value">Initial value.</param>
        /// <param name="_valueRange">The range that will limit the value.</param>
        protected _ALimitedValue(T_VALUE _value, T_RANGE _valueRange)
        {
            _m_valueRange = _valueRange;
            
            value = _value;
        }
        /// <summary>
        /// Create a limited value
        /// </summary>
        /// <remarks>
        /// <para>Soft border size will allow the value break the limit a little bit, and recover to the range after a delay.</para>
        /// </remarks>
        /// <param name="_value">Initial value.</param>
        /// <param name="_valueRange">The range that will limit the value.</param>
        /// <param name="_softBorderSize">How much the value can break the limit.</param>
        /// <param name="_runType">Determine where the recover task will run.</param>
        /// <param name="_recoverDelay">How many frames the value will recover to the range after it's out of the range and no more changes.</param>
        protected _ALimitedValue(T_VALUE _value, T_RANGE _valueRange, float _softBorderSize, UpdateType _runType = UpdateType.Update, int _recoverDelay = 1)
        {
            _m_valueRange = _valueRange;
            _m_softBorderSize = _softBorderSize;
            if (_m_recoverDelay >= 0 && _m_softBorderSize > 0)
            {
                _m_recoverTask = Task.RunContinuousActionTask(_RecoverValue, -1, _runType);
                _m_recoverDelay = _recoverDelay;
            }
            
            value = _value;
        }
        ~_ALimitedValue()
        {
            _m_recoverTask.StopTask();
        }


        /// <summary>
        /// The value after limited.
        /// </summary>
        /// <remarks>
        /// <para>Notice that the value will be limited by the range.</para>
        /// <para>
        /// If the value you set is out of the range, it will be clamped to the range,
        /// or break the limit a little bit (Not as much as you set) if the soft border size is set,
        /// and recover to the range after a delay.
        /// </para>
        /// <para>If you want to add a delta to the value, use <see cref="AppleDeltaValue"/> instead.</para>
        /// </remarks>
        public T_VALUE value
        {
            get { return _m_value; }
            set
            {
                if (_m_valueRange == null)
                {
                    _m_value = value;
                    return;
                }

                T_VALUE clampedValue = _m_valueRange.ClampValue(value);
                _m_value = _SoftTheClampedValue(value, clampedValue);
                _m_recoverCounter = _m_recoverDelay;
                _m_smoothDampVelocity = default;
            }
        }


        /// <summary>
        /// Add a delta value to the value.
        /// </summary>
        /// <remarks>
        /// <para>Use this method when you want to add a delta to the value.</para>
        /// <para>
        /// Because, when the value is out of the range and the soft border size is set,
        /// the value will break the limit a little bit BUT not as much as you set,
        /// so it's hard to change the value smoothly if you set the value directly.
        /// </para>
        /// </remarks>
        /// <param name="_deltaValue">The delta value you want to add.</param>
        public void AppleDeltaValue(T_VALUE _deltaValue)
        {
            if (_m_valueRange == null)
            {
                value = AddValue(_m_value, _deltaValue);
                return;
            }

            T_VALUE clampedValue = _m_valueRange.ClampValue(value);
            T_VALUE originValue = _GetTheOriginValue(value, clampedValue);
            value = AddValue(originValue, _deltaValue);
        }


        // It seems that the Generic type can not do the math operation directly, so you need to implement these methods for T_VALUE.
        
        /// <summary>
        /// Implement T_VALUE + T_VALUE
        /// </summary>
        protected abstract T_VALUE AddValue(T_VALUE _value, T_VALUE _deltaValue);
        /// <summary>
        /// Implement T_VALUE - T_VALUE
        /// </summary>
        protected abstract T_VALUE SubtractValue(T_VALUE _value, T_VALUE _deltaValue);
        /// <summary>
        /// Implement T_VALUE * float
        /// </summary>
        protected abstract T_VALUE MultiplyValue(T_VALUE _value, float _multiplier);
        /// <summary>
        /// Implement the normalize method for T_VALUE
        /// </summary>
        protected abstract T_VALUE Normalize(T_VALUE _value);
        /// <summary>
        /// Implement the magnitude method for T_VALUE
        /// </summary>
        protected abstract float GetMagnitude(T_VALUE _value);
        /// <summary>
        /// Implement the SmoothDamp method for T_VALUE
        /// </summary>
        protected abstract T_VALUE SmoothDamp(T_VALUE _current, T_VALUE _target, ref T_VALUE _currentVelocity, float _smoothTime);


        private T_VALUE _SoftTheClampedValue(T_VALUE _value, T_VALUE _clampedValue)
        {
            if (_m_softBorderSize <= 0)
                return _clampedValue;
            
            if (_value.Equals(_clampedValue))
                return _clampedValue;
            
            T_VALUE deltaValue = SubtractValue(_value, _clampedValue);
            T_VALUE deltaValueUnit = Normalize(deltaValue);
            float deltaValueMagnitude = GetMagnitude(deltaValue);
            return AddValue(_clampedValue, MultiplyValue(deltaValueUnit, _m_softBorderSize * (1 - 1 / (deltaValueMagnitude / _m_softBorderSize + 1))));
        }
        private T_VALUE _GetTheOriginValue(T_VALUE _value, T_VALUE _clampedValue)
        {
            if (_m_softBorderSize <= 0)
                return _value;

            if (_value.Equals(_clampedValue))
                return _value;
            
            T_VALUE deltaValue = SubtractValue(_value, _clampedValue);
            T_VALUE deltaValueUnit = Normalize(deltaValue);
            float deltaValueMagnitude = GetMagnitude(deltaValue);
            return AddValue(_clampedValue, MultiplyValue(deltaValueUnit, (1 / (1 - deltaValueMagnitude / _m_softBorderSize) - 1) * _m_softBorderSize));
        }
        private void _RecoverValue(float _deltaTime)
        {
            if (_m_recoverCounter > 0)
            {
                _m_recoverCounter--;
                return;
            }
            
            if (_m_valueRange == null)
                return;

            _m_value = SmoothDamp(_m_value, _m_valueRange.ClampValue(_m_value), ref _m_smoothDampVelocity, 0.1f);
        }
    }
}