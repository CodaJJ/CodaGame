// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    public interface _IValueRange<T_VALUE>
    {
        bool IsInRange(T_VALUE _value);
        T_VALUE ClampValue(T_VALUE _value);
        T_VALUE RandomValue();
    }
}