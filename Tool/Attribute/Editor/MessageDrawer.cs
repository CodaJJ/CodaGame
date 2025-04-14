// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace CodaGame.Editor
{
    [CustomPropertyDrawer(typeof(_AMessageAttribute), true)]
    public class MessageDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect _position)
        {
            _AMessageAttribute target = (_AMessageAttribute)attribute;
            EditorUtility.DrawHelpBox(_position, target.content, target.type, true);
        }
        public override float GetHeight()
        {
            _AMessageAttribute target = (_AMessageAttribute)attribute;
            return EditorUtility.GetHelpBoxHeight(target.content, true);
        }
    }
}