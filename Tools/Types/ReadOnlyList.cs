// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace CodaGame
{
    /// <summary>
    /// A read-only List wrapper.
    /// </summary>
    public readonly struct ReadOnlyList<T> : IReadOnlyList<T>
    {
        private readonly List<T> _m_list;
        

        public ReadOnlyList(List<T> _list)
        {
            _m_list = _list;
        }
        

        public int Count { get { return _m_list?.Count ?? 0; } }
        public T this[int _index] { get { return _m_list == null ? default : _m_list[_index]; } }
        

        public bool IsNullOrEmpty()
        {
            return _m_list == null || _m_list.Count == 0;
        }
        public int IndexOf(Predicate<T> _predicate)
        {
            if (_predicate == null)
                return -1;

            for (int i = 0; i < Count; ++i)
                if (_predicate(_m_list[i]))
                    return i;

            return -1;
        }
        public bool Contains(T _value)
        {
            return _m_list?.Contains(_value) ?? false;
        }
        public List<T>.Enumerator GetEnumerator()
        {
            return _m_list?.GetEnumerator() ?? default;
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        
        public static implicit operator ReadOnlyList<T>(List<T> _list)
        {
            return new ReadOnlyList<T>(_list);
        }
    }
}