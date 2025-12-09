// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Base class for all table configuration ScriptableObjects.
    /// </summary>
    public abstract class _ATableConfig<T> : ScriptableObject
    {
        [SerializeField, RuntimeReadOnly] 
        private List<T> _m_dataList;
        private List<T> _m_notNullDataList;
        
        
        [ItemNotNull]
        public ReadOnlyList<T> notNullDataList
        {
            get
            {
                if (_m_notNullDataList == null)
                {
                    _m_notNullDataList = new List<T>(_m_dataList?.Count ?? 0);
                    if (_m_dataList != null)
                    {
                        foreach (T data in _m_dataList)
                        {
                            if (data != null)
                                _m_notNullDataList.Add(data);
                        }
                    }
                }
                return _m_notNullDataList;
            }
        }
        
        
        internal List<T> dataList { get { return _m_dataList; } set { _m_dataList = value; } }
    }
}