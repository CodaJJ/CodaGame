// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A range defined by a <see cref="Sphere"/>.
    /// </summary>
    [Serializable]
    public class SphereRange : _IValueRange<Vector3>
    { 
        public Sphere sphere;
        
        
        public SphereRange(Vector3 _center, float _radius)
        {
            sphere = new Sphere(_center, _radius);
        }
        public SphereRange(Sphere _sphere)
        {
            sphere = _sphere;
        }
        
        
        public bool IsInRange(Vector3 _value)
        {
            return sphere.Contains(_value);
        }
        public Vector3 ClampValue(Vector3 _value)
        {
            return sphere.ClosestPoint(_value);
        }
        public Vector3 RandomValue()
        {
            return sphere.RandomPoint();
        }
    }
}