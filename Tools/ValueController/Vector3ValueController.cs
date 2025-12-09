// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Vector3 value controller
    /// </summary>
    /// <remarks>
    /// <inheritdoc />
    /// </remarks>
    public class Vector3ValueController : _AValueController<Vector3>
    {
        public Vector3ValueController(string _name, Vector3 _initValue) 
            : base(_name, _initValue)
        {
        }
        
        
        /// <inheritdoc />
        protected override Vector3 Add(Vector3 _value1, Vector3 _value2)
        {
            return _value1 + _value2;
        }
        /// <inheritdoc />
        protected override Vector3 Lerp(Vector3 _value1, Vector3 _value2, float _t)
        {
            return Vector3.Lerp(_value1, _value2, _t);
        }
    }
}