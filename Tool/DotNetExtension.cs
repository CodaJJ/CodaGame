// Copyright (c) 2024 Coda
// 
// This file is part of Unity Game Framework, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;

namespace CodaGame
{
    public static class DotNetExtension
    {
        #region List

        public static T GetFirstAndRemove<T>(this List<T> _list)
        {
            if (_list.Count == 0)
                return default;
            
            T result = _list[0];
            _list.RemoveAt(0);
            return result;
        }
        public static T GetLastAndRemove<T>(this List<T> _list)
        {
            if (_list.Count == 0)
                return default;
            
            T result = _list[^1];
            _list.RemoveAt(_list.Count - 1);
            return result;
        }
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
        /// <para>Uses binary search to insert the value into the sorted list, and keeps the elements adding order.</para>
        /// </remarks>
        public static void InsertSorted<T>(this List<T> _list, T _value) 
            where T : System.IComparable<T>
        {
            int index = _list.BinarySearch(_value);
            if (index >= 0)
            {
                while (index + 1 < _list.Count) 
                {
                    T next = _list[index + 1];
                    if (next == null)
                    {
                        if (_value != null)
                            break;
                        
                        index++;
                    }
                    else if (next.CompareTo(_value) == 0)
                        index++;
                    else
                        break;
                }

                _list.Insert(index + 1, _value);
            }
            else
                _list.Insert(~index, _value);
        }
        /// <summary>
        /// Insert a value into a sorted list.
        /// </summary>
        /// <remarks>
        /// <para>Uses binary search to insert the value into the sorted list, and keeps the elements adding order.</para>
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
            
            while (index < _list.Count)
            {
                T current = _list[index];
                if (current == null)
                {
                    if (_value != null)
                        break;
                    
                    _list.RemoveAt(index);
                    return true;
                }

                if (current.Equals(_value))
                {
                    _list.RemoveAt(index);
                    return true;
                }
                
                if (current.CompareTo(_value) == 0)
                    index++;
                else
                    break;
            }

            while (index - 1 >= 0)
            {
                T current = _list[index - 1];
                if (current == null)
                {
                    if (_value != null)
                        break;
                    
                    _list.RemoveAt(index - 1);
                    return true;
                }
                
                if (current.Equals(_value))
                {
                    _list.RemoveAt(index - 1);
                    return true;
                }
                
                if (current.CompareTo(_value) == 0)
                    index--;
                else
                    break;
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
            int index = _list.BinarySearch(_value, _comparer);
            if (index < 0)
                return false;

            while (index < _list.Count)
            {
                T current = _list[index];
                if (_comparer.Compare(current, _value) != 0)
                    break;
                
                if (current == null)
                {
                    if (_value != null)
                        break;
                    
                    _list.RemoveAt(index);
                    return true;
                }
                
                if (current.Equals(_value))
                {
                    _list.RemoveAt(index);
                    return true;
                }
                
                index++;
            }
            
            while (index - 1 >= 0)
            {
                T current = _list[index - 1];
                if (_comparer.Compare(current, _value) != 0)
                    break;
                
                if (current == null)
                {
                    if (_value != null)
                        break;
                    
                    _list.RemoveAt(index - 1);
                    return true;
                }
                
                if (current.Equals(_value))
                {
                    _list.RemoveAt(index - 1);
                    return true;
                }
                
                index--;
            }

            return false;
        }

        #endregion
        
        #region Dictionary

        public static T_VALUE GetValueAndRemove<T_KEY, T_VALUE>(this Dictionary<T_KEY, T_VALUE> _dictionary, T_KEY _key)
        {
            T_VALUE value = _dictionary[_key];
            _dictionary.Remove(_key);
            return value;
        }
        
        #endregion
    }
}