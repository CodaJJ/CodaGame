// Copyright (c) 2024 Coda
// 
// This file is part of Unity Game Framework, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;

namespace CodaGame
{
    
    public static class DotNetExtension
    {
        /// <summary>
        /// Compare two values, and check if they are equal.
        /// </summary>
        /// <remarks>
        /// <para>It's different from the default <see cref="object.Equals(object, object)"/> method, using <see cref="System.IComparable{T}"/> to compare the values.</para>
        /// </remarks>
        public static bool CompareEqual<T>(T _a, T _b)
            where T : System.IComparable<T>
        {
            if (_a is null || _b is null)
                return _a is null && _b is null;
            return _a.CompareTo(_b) == 0;
        }
        
        #region List

        /// <summary>
        /// Get the first element of the list, and remove it from the list.
        /// </summary>
        public static T GetFirstAndRemove<T>(this List<T> _list)
        {
            if (_list.Count == 0)
                return default;
            
            T result = _list[0];
            _list.RemoveAt(0);
            return result;
        }
        /// <summary>
        /// Get the last element of the list, and remove it from the list.
        /// </summary>
        public static T GetLastAndRemove<T>(this List<T> _list)
        {
            if (_list.Count == 0)
                return default;
            
            T result = _list[^1];
            _list.RemoveAt(_list.Count - 1);
            return result;
        }
        /// <summary>
        /// Remove the last element of the list.
        /// </summary>
        public static bool RemoveLast<T>(this List<T> _list)
        {
            if (_list.Count == 0)
                return false;
            
            _list.RemoveAt(_list.Count - 1);
            return true;
        }
        /// <summary>
        /// Insert a value into a sorted list.
        /// </summary>
        /// <remarks>
        /// <para>Uses binary search to insert the value into the sorted list, and preserves the insertion order among equal elements.</para>
        /// </remarks>
        public static void InsertSorted<T>(this List<T> _list, T _value) 
            where T : System.IComparable<T>
        {
            int index = _list.BinarySearch(_value);
            if (index >= 0)
            {
                while (index + 1 < _list.Count && CompareEqual(_list[index + 1], _value))
                    index++;

                _list.Insert(index + 1, _value);
            }
            else
                _list.Insert(~index, _value);
        }
        /// <summary>
        /// Insert a value into a sorted list.
        /// </summary>
        /// <remarks>
        /// <para>Uses binary search to insert the value into the sorted list, and preserves the insertion order among equal elements.</para>
        /// </remarks>
        public static void InsertSorted<T>(this List<T> _list, T _value, IComparer<T> _comparer)
        {
            _comparer ??= Comparer<T>.Default;
            int index = _list.BinarySearch(_value, _comparer);
            if (index >= 0)
            {
                while (index + 1 < _list.Count && _comparer.Compare(_list[index + 1], _value) == 0)
                    index++;

                _list.Insert(index + 1, _value);
            }
            else
                _list.Insert(~index, _value);
        }
        /// <summary>
        /// Remove a value from a sorted list.
        /// </summary>
        /// <remarks>
        /// <para>Uses binary search to remove the value from the sorted list.</para>
        /// <para>After binary search, it will check the elements before and after the found index to remove the value.</para>
        /// </remarks>
        public static bool RemoveSorted<T>(this List<T> _list, T _value)
            where T : System.IComparable<T>
        {
            int index = _list.BinarySearch(_value);
            if (index < 0)
                return false;

            for (int i = index; i >= 0 && CompareEqual(_list[i], _value); i--)
            {
                if (Equals(_list[i], _value))
                {
                    _list.RemoveAt(i);
                    return true;
                }
            }

            for (int i = index + 1; i < _list.Count && CompareEqual(_list[i], _value); i++)
            {
                if (Equals(_list[i], _value))
                {
                    _list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Remove a value from a sorted list.
        /// </summary>
        /// <remarks>
        /// <para>Uses binary search to remove the value from the sorted list.</para>
        /// <para>After binary search, it will check the elements before and after the found index to remove the value.</para>
        /// </remarks>
        public static bool RemoveSorted<T>(this List<T> _list, T _value, IComparer<T> _comparer)
        {
            _comparer ??= Comparer<T>.Default;
            int index = _list.BinarySearch(_value);
            if (index < 0)
                return false;

            for (int i = index; i >= 0 && _comparer.Compare(_list[i], _value) == 0; i--)
            {
                if (Equals(_list[i], _value))
                {
                    _list.RemoveAt(i);
                    return true;
                }
            }

            for (int i = index + 1; i < _list.Count && _comparer.Compare(_list[i], _value) == 0; i++)
            {
                if (Equals(_list[i], _value))
                {
                    _list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Set the count of the list, and fill the new elements with default value.
        /// </summary>
        /// <remarks>
        /// <para>Or remove the extra elements if the count is less than the current count.</para>
        /// </remarks>
        public static void SetCount<T>(this List<T> _list, int _count)
        {
            if (_list.Count > _count)
                _list.RemoveRange(_count, _list.Count - _count);
            else if (_list.Count < _count)
            {
                int addCount = _count - _list.Count;
                for (int i = 0; i < addCount; i++)
                    _list.Add(default);
            }
        }

        #endregion
        
        #region Dictionary
        
        /// <summary>
        /// Get the value of the dictionary, and remove it from the dictionary.
        /// </summary>
        public static T_VALUE GetValueAndRemove<T_KEY, T_VALUE>(this Dictionary<T_KEY, T_VALUE> _dictionary, T_KEY _key)
        {
            T_VALUE value = _dictionary[_key];
            _dictionary.Remove(_key);
            return value;
        }
        
        #endregion
    }
}