// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A range defined by a <see cref="LineSeg3D"/>.
    /// </summary>
    [Serializable]
    public class LineSeg3DRange : _IValueRange<Vector3>
    {
        public LineSeg3D lineSeg3D;
        
        
        public bool IsInRange(Vector3 _value)
        {
            return lineSeg3D.Contains(_value);
        }
        public Vector3 ClampValue(Vector3 _value)
        {
            return lineSeg3D.ClosestPoint(_value);
        }
        public Vector3 RandomValue()
        {
            return lineSeg3D.RandomPoint();
        }
    }
}