// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Vector2 value controller
    /// </summary>
    /// <remarks>
    /// <inheritdoc />
    /// </remarks>
    public class Vector2ValueController : _AValueController<Vector2>
    {
        public Vector2ValueController(string _name, Vector2 _initValue) 
            : base(_name, _initValue)
        {
        }


        /// <inheritdoc />
        protected override Vector2 Add(Vector2 _value1, Vector2 _value2)
        {
            return _value1 + _value2;
        }
        /// <inheritdoc />
        protected override Vector2 Lerp(Vector2 _value1, Vector2 _value2, float _t)
        {
            return Vector2.Lerp(_value1, _value2, _t);
        }
    }
}