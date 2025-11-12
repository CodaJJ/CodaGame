// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodaGame
{
    public abstract class _ATableConfig<T> : ScriptableObject
    {
        public List<T> dataList;


        public Type GetDataType()
        {
            return typeof(T);
        }
    }
}