// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;

namespace CodaGame
{
    public static class EaseFunctions
    {
        private const float _k_c1 = 1.70158f;
        private const float _k_c2 = _k_c1 * 1.525f;
        private const float _k_c3 = _k_c1 + 1;
        private const float _k_c4 = Values.TwoPi / 3;
        private const float _k_c5 = Values.TwoPi / 4.5f;
        private const float _k_n1 = 7.5625f;
        private const float _k_d1 = 2.75f;
        

        /// <summary>
        /// Evaluate the ease function.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method evaluates the easing curve defined by the specified <see cref="EaseType"/>.
        /// The input <paramref name="_value"/> is expected to be in the normalized range of <c>0</c> to <c>1</c>,
        /// representing the progress of the animation or transition.
        /// </para>
        /// <para>
        /// If <paramref name="_value"/> falls outside the range of <c>0</c> to <c>1</c>, it will be handled according
        /// to the specified <see cref="OverflowType"/>.
        /// </para>
        /// </remarks>
        public static float Evaluate(this EaseType _easeType, float _value, OverflowType _overflowType = OverflowType.Clamp)
        {
            float x = _overflowType switch
            {
                OverflowType.Clamp => Mathf.Clamp01(_value),
                OverflowType.Repeat => Mathf.Repeat(_value, 1f),
                OverflowType.PingPong => Mathf.PingPong(_value, 1f),
                _ => _value
            };

            switch (_easeType)
            {
                case EaseType.Linear:
                    return x;
                case EaseType.InSine:
                    return 1f - Mathf.Cos(x * Values.PiOver2);
                case EaseType.OutSine:
                    return Mathf.Sin(x * Values.PiOver2);
                case EaseType.InOutSine:
                    return -(Mathf.Cos(Values.Pi * x) - 1) / 2;
                case EaseType.InQuad:
                    return x * x;
                case EaseType.OutQuad:
                    return 1 - (1 - x) * (1 - x);
                case EaseType.InOutQuad:
                    if ((x /= 0.5f) < 1) return 0.5f * x * x;
                    return 1 - (x -= 2) * x / 2;
                case EaseType.InCubic:
                    return x * x * x;
                case EaseType.OutCubic:
                    return --x * x * x + 1;
                case EaseType.InOutCubic:
                    if ((x /= 0.5f) < 1) return 0.5f * x * x * x;
                    return 1 + (x -= 2) * x * x / 2;
                case EaseType.InQuart:
                    return x * x * x * x;
                case EaseType.OutQuart:
                    return 1 - (x -= 1) * x * x * x;
                case EaseType.InOutQuart:
                    if ((x /= 0.5f) < 1) return 0.5f * x * x * x * x;
                    return 1 - (x -= 2) * x * x * x / 2;
                case EaseType.InQuint:
                    return x * x * x * x * x;
                case EaseType.OutQuint:
                    return (x -= 1) * x * x * x * x + 1;
                case EaseType.InOutQuint:
                    if ((x /= 0.5f) < 1) return 0.5f * x * x * x * x * x;
                    return 1 + (x -= 2) * x * x * x * x / 2;
                case EaseType.InExpo:
                    return x == 0 ? 0 : Mathf.Pow(2, 10 * (x - 1));
                case EaseType.OutExpo:
                    return x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);
                case EaseType.InOutExpo:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    if ((x /= 0.5f) < 1) return Mathf.Pow(2, 10 * (x - 1)) / 2;
                    return (2 - Mathf.Pow(2, 10 * (1 - x))) / 2;
                case EaseType.InCirc:
                    return 1 - Mathf.Sqrt(1 - x * x);
                case EaseType.OutCirc:
                    return Mathf.Sqrt(1 - (x -= 1) * x);
                case EaseType.InOutCirc:
                    if ((x /= 0.5f) < 1) return (1 - Mathf.Sqrt(1 - x * x)) / 2;
                    return (Mathf.Sqrt(1 - (x -= 2) * x) + 1) / 2;
                case EaseType.InBack:
                    return _k_c3 * x * x * x - _k_c1 * x * x;
                case EaseType.OutBack:
                    return 1 + _k_c3 * (x -= 1) * x * x + _k_c1 * x * x;
                case EaseType.InOutBack:
                    if ((x /= 0.5f) < 1) return x * x * ((_k_c2 + 1) * x - _k_c2) / 2;
                    return (x -= 2) * x * ((_k_c2 + 1) * x + _k_c2) / 2 + 1;
                case EaseType.InElastic:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return -Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((x * 10 - 10.75f) * _k_c4);
                case EaseType.OutElastic:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    return Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * _k_c4) + 1;
                case EaseType.InOutElastic:
                    if (x == 0) return 0;
                    if (x == 1) return 1;
                    if ((x /= 0.5f) < 1) return -(Mathf.Pow(2, 10 * x - 10) * Mathf.Sin((10 * x - 11.125f) * _k_c5)) / 2;
                    return Mathf.Pow(2, 10 - 10 * x) * Mathf.Sin((10 * x - 11.125f) * _k_c5) / 2 + 1;
                case EaseType.InBounce:
                    return 1 - _OutBounce(1 - x);
                case EaseType.OutBounce:
                    return _OutBounce(x);
                case EaseType.InOutBounce:
                    if ((x /= 0.5f) < 1) return (1 - _OutBounce(1 - x)) / 2;
                    return (1 + _OutBounce(x - 1)) / 2;
                default:
                    return 0;
            }
        }
        

        private static float _OutBounce(float _value)
        {
            if (_value < 1 / _k_d1) return _k_n1 * _value * _value;
            if (_value < 2 / _k_d1) return _k_n1 * (_value -= 1.5f / _k_d1) * _value + 0.75f; 
            if (_value < 2.5 / _k_d1) return _k_n1 * (_value -= 2.25f / _k_d1) * _value + 0.9375f; 
            return _k_n1 * (_value -= 2.625f / _k_d1) * _value + 0.984375f;
        }
    }
}
