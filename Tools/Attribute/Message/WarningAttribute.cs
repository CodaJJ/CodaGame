// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEditor;

namespace CodaGame
{
    /// <summary>
    /// Show a warning message in the inspector.
    /// </summary>
    public class WarningAttribute : _AMessageAttribute
    {
        public WarningAttribute(string _content) 
            : base(_content)
        {
        }


        public override MessageType type { get { return MessageType.Warning; } }  
    }
}