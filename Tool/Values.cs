// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;

namespace CodaGame
{
    public static class Values
    {
        public const float Pi = Mathf.PI;
        public const float PiOver2 = Mathf.PI * 0.5f;
        public const float TwoPi = Mathf.PI * 2;
        public const float LowPrecisionTolerance = 1e-3f;
        public const float MediumPrecisionTolerance = 1e-5f;
        public const float HighPrecisionTolerance = 1e-10f;
        
        public const double LowPrecisionToleranceD = 1e-3;
        public const double MediumPrecisionToleranceD = 1e-5;
        public const double HighPrecisionToleranceD = 1e-10;
    }
}