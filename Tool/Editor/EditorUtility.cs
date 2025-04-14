// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace CodaGame.Editor
{
    public static class EditorUtility
    {
        private const int _k_helpBoxIndent = 15;
        private const int _k_helpBoxIconWidth = 55;
        private const int _k_helpBoxFontSize = 13;
        private const int _k_helpBoxMinHeight = 40;
        private const int _k_helpBoxLineSpace = 2;
        
        
        /// <summary>
        /// Calculates the height of a label.
        /// </summary>
        public static float CalculateLabelHeight(string _label, GUIStyle _style, float _headerWith)
        {
            if (_style == null)
                return 0;
            
            return _style.CalcHeight(new GUIContent(_label), EditorGUIUtility.currentViewWidth - _headerWith);
        }
        /// <remarks>
        /// Returns the height of the help box.
        /// </remarks>
        public static float GetHelpBoxHeight(string _message, bool _isDecorator = false)
        {
            float decoratorIndent = _isDecorator ? _k_helpBoxIndent : 0;
            float decoratorLineSpace = _isDecorator ? _k_helpBoxLineSpace : 0;
            
            GUIStyle style = new GUIStyle(EditorStyles.helpBox);
            style.fontSize = _k_helpBoxFontSize;
            float height = CalculateLabelHeight(_message, style, _k_helpBoxIconWidth + decoratorIndent);
            return Mathf.Max(_k_helpBoxMinHeight, height) + decoratorLineSpace;
        }
        /// <summary>
        /// Draws a help box in the inspector.
        /// </summary>
        /// <param name="_isDecorator">True if the help box is a decorator tip, the space is different from a normal help box.</param>
        public static void DrawHelpBox(Rect _position, string _message, MessageType _type, bool _isDecorator = false)
        {
            float decoratorIndent = _isDecorator ? _k_helpBoxIndent : 0;
            float decoratorLineSpace = _isDecorator ? _k_helpBoxLineSpace : 0;
            
            int originSize = EditorStyles.helpBox.fontSize;
            EditorStyles.helpBox.fontSize = _k_helpBoxFontSize;
            {
                Rect helpPosition = _position;
                helpPosition.height = GetHelpBoxHeight(_message, _isDecorator) - decoratorLineSpace;
                helpPosition.width -= decoratorIndent;
                helpPosition.x += decoratorIndent;

                EditorGUI.HelpBox(helpPosition, _message, _type);
            }
            EditorStyles.helpBox.fontSize = originSize;
        }
    }
}