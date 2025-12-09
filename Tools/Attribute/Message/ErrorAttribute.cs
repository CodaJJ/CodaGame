// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEditor;

namespace CodaGame
{
    /// <summary>
    /// Show an error message in the inspector.
    /// </summary>
    public class ErrorAttribute : _AMessageAttribute
    {
        public ErrorAttribute(string _content) 
            : base(_content)
        {
        }


        public override MessageType type { get { return MessageType.Error; } }  
    }
}