// Copyright (c) 2026 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Diagnostics;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Limits a numeric field to a min-max range in Inspector.
    /// Use this instead of Unity's built-in RangeAttribute when you need
    /// the field to go through Coda's PropertyDrawerExtension chain.
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class RangeAttribute : PropertyAttribute
    {
        public readonly float min;
        public readonly float max;


        public RangeAttribute(float _min, float _max)
        {
            if (_min <= _max)
            {
                min = _min;
                max = _max;
                return;
            }

            min = _max;
            max = _min;
        }
    }
}
