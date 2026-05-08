// Copyright (c) 2026 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Diagnostics;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Shows the decorated field only when a target bool field matches the expected value.
    /// </summary>
    /// <example>
    /// <code>
    /// [SerializeField] private bool _m_useAdvanced;
    /// [SerializeField, ShowIf(nameof(_m_useAdvanced))] private float _m_advancedValue;
    /// </code>
    /// </example>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string conditionFieldName;
        public readonly bool expectedValue;


        public ShowIfAttribute(string _conditionFieldName, bool _expectedValue = true)
        {
            conditionFieldName = _conditionFieldName;
            expectedValue = _expectedValue;
        }
    }
}
