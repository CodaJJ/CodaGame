// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Base class for value behaviours that use soft blending
    /// </summary>
    /// <remarks>
    /// <para>InOutSine interpolation is default for fade in and fade out.</para>
    /// </remarks>
    public abstract class _AValueBehaviourSoftBlend<T_VALUE> : _AValueBehaviour<T_VALUE>
        where T_VALUE : struct
    {
        protected _AValueBehaviourSoftBlend(int _priority, float _fadeInTime = 0.5f, float _fadeOutTime = 0.5f) 
            : base(_priority, _fadeInTime, _fadeOutTime)
        {
        }


        /// <inheritdoc />
        protected override float FadeIn(float _value)
        {
            // InOutSine
            return -(Mathf.Cos(Values.Pi * _value) - 1) / 2;
        }
        /// <inheritdoc />
        protected override float InverseFadeIn(float _value)
        {
            // Inverse InOutSine
            return Mathf.Acos(1f - 2f * _value) / Values.Pi;
        }
        /// <inheritdoc />
        protected override float FadeOut(float _value)
        {
            return FadeIn(_value);
        }
        /// <inheritdoc />
        protected override float InverseFadeOut(float _value)
        {
            return InverseFadeIn(_value);
        }
    }
}