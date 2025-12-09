// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Diagnostics;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Indicates that the property is a radian value, which will be displayed in the inspector as a degree value.
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public class RadianAttribute : PropertyAttribute
    {
    }
}