// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Utility class for working with enum types as arrays.
    /// </summary>
    public static class EnumArray<T> where T : Enum
    {
        [ItemNotNull, NotNull] public static readonly T[] Values = (T[])Enum.GetValues(typeof(T));
        public static readonly int Length = Values.Length;
        
        
        public static T GetValue(int _index)
        {
            return Values[_index];
        }
        public static bool TryGetValue(int _index, out T _value)
        {
            if (_index < 0 || _index >= Length)
            {
                _value = default;
                return false;
            }
            _value = Values[_index];
            return true;
        }
        public static int IndexOf(T _value)
        {
            return Array.IndexOf(Values, _value);
        }
        public static T Random()
        {
            return Values[UnityEngine.Random.Range(0, Length)];
        }
    }
}