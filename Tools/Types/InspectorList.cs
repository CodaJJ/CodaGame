// Copyright (c) 2026 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodaGame
{
    /// <summary>
    /// A serializable List wrapper for Inspector display.
    /// </summary>
    /// <remarks>
    /// Unity does not treat List subclasses as built-in array properties.
    /// This wrapper keeps the real serialized field as <see cref="_m_list"/>,
    /// while exposing a List-like API so call sites can use it almost like <see cref="List{T}"/>.
    /// </remarks>
    [Serializable]
    public class InspectorList<T> : IList<T>, IReadOnlyList<T>
    {
        [NotNull, SerializeField] private List<T> _m_list;


        public InspectorList()
        {
            _m_list = new List<T>();
        }
        public InspectorList(int _capacity)
        {
            _m_list = new List<T>(_capacity);
        }
        public InspectorList(IEnumerable<T> _collection)
        {
            _m_list = _collection == null ? new List<T>() : new List<T>(_collection);
        }


        /// <summary>
        /// Number of items in the list.
        /// </summary>
        public int Count { get { return _m_list.Count; } }
        /// <summary>
        /// Backing list capacity.
        /// </summary>
        public int Capacity
        {
            get { return _m_list.Capacity; }
            set { _m_list.Capacity = value; }
        }
        public bool IsReadOnly { get { return false; } }
        public T this[int _index]
        {
            get { return _m_list[_index]; }
            set { _m_list[_index] = value; }
        }


        public void Add(T _item)
        {
            _m_list.Add(_item);
        }
        public void AddRange(IEnumerable<T> _collection)
        {
            _m_list.AddRange(_collection);
        }
        public void Clear()
        {
            _m_list.Clear();
        }
        public bool Contains(T _item)
        {
            return _m_list.Contains(_item);
        }
        public void CopyTo(T[] _array, int _arrayIndex)
        {
            _m_list.CopyTo(_array, _arrayIndex);
        }
        public int IndexOf(T _item)
        {
            return _m_list.IndexOf(_item);
        }
        public void Insert(int _index, T _item)
        {
            _m_list.Insert(_index, _item);
        }
        public bool Remove(T _item)
        {
            return _m_list.Remove(_item);
        }
        public int RemoveAll(Predicate<T> _match)
        {
            return _m_list.RemoveAll(_match);
        }
        public void RemoveAt(int _index)
        {
            _m_list.RemoveAt(_index);
        }
        public void Reverse()
        {
            _m_list.Reverse();
        }
        public void Reverse(int _index, int _count)
        {
            _m_list.Reverse(_index, _count);
        }
        public void Sort()
        {
            _m_list.Sort();
        }
        public void Sort(IComparer<T> _comparer)
        {
            _m_list.Sort(_comparer);
        }
        public void Sort(Comparison<T> _comparison)
        {
            _m_list.Sort(_comparison);
        }
        public void Sort(int _index, int _count, IComparer<T> _comparer)
        {
            _m_list.Sort(_index, _count, _comparer);
        }
        public T[] ToArray()
        {
            return _m_list.ToArray();
        }
        public List<T>.Enumerator GetEnumerator()
        {
            return _m_list.GetEnumerator();
        }
        public T GetRandomElement()
        {
            if (_m_list.Count == 0)
                return default;
            
            return _m_list[Random.Range(0, _m_list.Count)];
        }

        
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        
        /// <summary>
        /// Gets the real backing list.
        /// </summary>
        public List<T> AsList()
        {
            return _m_list;
        }


        public static implicit operator List<T>(InspectorList<T> _list)
        {
            return _list?.AsList();
        }
        public static implicit operator ReadOnlyList<T>(InspectorList<T> _list)
        {
            return _list == null ? default : new ReadOnlyList<T>(_list.AsList());
        }
        public static implicit operator InspectorList<T>(List<T> _list)
        {
            return new InspectorList<T>(_list);
        }
    }
}
