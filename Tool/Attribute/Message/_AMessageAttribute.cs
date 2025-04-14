// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Diagnostics;
using UnityEngine;
using UnityEditor;

namespace CodaGame
{ 
    /// <summary>
    /// Show a message in the inspector.
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    public abstract class _AMessageAttribute : PropertyAttribute
    {
        private readonly string _m_content;
        

        public _AMessageAttribute(string _content)
        {
            _m_content = _content;
        }
        

        public string content { get { return _m_content; } }
        public abstract MessageType type { get; }
    }
}