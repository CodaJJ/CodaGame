// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Float value controller
    /// </summary>
    /// <remarks> 
    /// <inheritdoc />
    /// </remarks>
    public class FloatValueController : _AValueController<float>
    {
        public FloatValueController(string _name, float _initValue) 
            : base(_name, _initValue)
        {
        }
        

        /// <inheritdoc />
        protected override float Add(float _value1, float _value2)
        {
            return _value1 + _value2;
        }
        /// <inheritdoc />
        protected override float Lerp(float _value1, float _value2, float _t)
        {
            return Mathf.Lerp(_value1, _value2, _t);
        }
    }
}