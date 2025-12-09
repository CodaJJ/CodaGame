// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Quaternion value controller
    /// </summary>
    /// <remarks>
    /// <inheritdoc />
    /// </remarks>
    public class QuaternionValueController : _AValueController<Quaternion>
    {
        public QuaternionValueController(string _name, Quaternion _initValue) 
            : base(_name, _initValue)
        {
        }
        
        
        /// <inheritdoc />
        protected override Quaternion Add(Quaternion _value1, Quaternion _value2)
        {
            return _value2 * _value1;
        }
        /// <inheritdoc />
        protected override Quaternion Lerp(Quaternion _value1, Quaternion _value2, float _t)
        {
            return Quaternion.Slerp(_value1, _value2, _t);
        }
    }
}