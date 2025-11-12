// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;
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

        [NotNull] public static GUIStyle helpBoxStyle { get { return _g_helpBoxStyle ??= new GUIStyle(EditorStyles.helpBox) { fontSize = _k_helpBoxFontSize }; } }
        [NotNull] public static GUIStyle placeholderStyle { get { return _g_placeholderStyle ??= new GUIStyle(EditorStyles.label) { normal = { textColor = Color.gray } }; } }
        
        private static GUIStyle _g_helpBoxStyle;
        private static GUIStyle _g_placeholderStyle;
        

        /// <summary>
        /// Calculates the height of a label with word wrapping.
        /// </summary>
        /// <param name="_label">The label text</param>
        /// <param name="_style">The GUIStyle to use</param>
        /// <param name="_headerWidth">Width to subtract from available width (e.g., icon width)</param>
        public static float CalculateLabelHeight(string _label, GUIStyle _style, float _headerWidth)
        {
            if (_style == null)
                return 0;

            return _style.CalcHeight(new GUIContent(_label), EditorGUIUtility.currentViewWidth - _headerWidth);
        }
        /// <summary>
        /// Returns the height of the help box with the given message.
        /// </summary>
        /// <param name="_message">The message text</param>
        /// <param name="_isDecorator">True if the help box is a decorator tip</param>
        public static float GetHelpBoxHeight(string _message, bool _isDecorator = false)
        {
            float decoratorIndent = _isDecorator ? _k_helpBoxIndent : 0;
            float decoratorLineSpace = _isDecorator ? _k_helpBoxLineSpace : 0;

            float height = CalculateLabelHeight(_message, helpBoxStyle, _k_helpBoxIconWidth + decoratorIndent);
            return Mathf.Max(_k_helpBoxMinHeight, height) + decoratorLineSpace;
        }
        /// <summary>
        /// Draws a help box in the inspector with automatic layout.
        /// </summary>
        /// <param name="_message">The message text</param>
        /// <param name="_type">The message type (Info, Warning, Error)</param>
        /// <param name="_isDecorator">True if the help box is a decorator tip, the space is different from a normal help box.</param>
        public static void DrawHelpBox(string _message, MessageType _type, bool _isDecorator = false)
        {
            float height = GetHelpBoxHeight(_message, _isDecorator);
            Rect position = EditorGUILayout.GetControlRect(false, height);
            DrawHelpBox(position, _message, _type, _isDecorator);
        }
        /// <summary>
        /// Draws a help box in the inspector (IMGUI Rect version).
        /// </summary>
        /// <param name="_position">Rectangle to draw in</param>
        /// <param name="_message">The message text</param>
        /// <param name="_type">The message type (Info, Warning, Error)</param>
        /// <param name="_isDecorator">True if the help box is a decorator tip, the space is different from a normal help box.</param>
        public static void DrawHelpBox(Rect _position, string _message, MessageType _type, bool _isDecorator = false)
        {
            float decoratorIndent = _isDecorator ? _k_helpBoxIndent : 0;
            float decoratorLineSpace = _isDecorator ? _k_helpBoxLineSpace : 0;

            Rect helpPosition = _position;
            helpPosition.height = GetHelpBoxHeight(_message, _isDecorator) - decoratorLineSpace;
            helpPosition.width -= decoratorIndent;
            helpPosition.x += decoratorIndent;

            int originalFontSize = EditorStyles.helpBox.fontSize;
            EditorStyles.helpBox.fontSize = _k_helpBoxFontSize;
            EditorGUI.HelpBox(helpPosition, _message, _type);
            EditorStyles.helpBox.fontSize = originalFontSize;
        }
        /// <summary>
        /// Draws a file path field with a label, text input, and browse button for selecting a file.
        /// </summary>
        /// <param name="_label">Label text to display</param>
        /// <param name="_path">Current file path</param>
        /// <param name="_extension">File extension filter (e.g., "xlsx,xls" for Excel files, "" for all files)</param>
        /// <param name="_defaultPath">Default directory to open (optional)</param>
        public static string DrawFilePathField(string _label, string _path, string _extension = "", string _defaultPath = "")
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(_label, GUILayout.Width(EditorGUIUtility.labelWidth));
                string newPath = EditorGUILayout.TextField(_path);
                if (GUILayout.Button("Browse", GUILayout.Width(70)))
                {
                    string selectedPath = UnityEditor.EditorUtility.OpenFilePanel("Select File", _defaultPath, _extension);
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        newPath = selectedPath;
                    }
                }

                _path = newPath;
            }
            EditorGUILayout.EndHorizontal();

            return _path;
        }
        /// <summary>
        /// Draws a folder path field with a label, text input, and browse button for selecting a folder.
        /// </summary>
        /// <param name="_label">Label text to display</param>
        /// <param name="_path">Current folder path</param>
        /// <param name="_defaultPath">Default directory to open (optional)</param>
        public static string DrawFolderPathField(string _label, string _path, string _defaultPath = "")
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(_label, GUILayout.Width(EditorGUIUtility.labelWidth));

                string newPath = EditorGUILayout.TextField(_path);

                if (GUILayout.Button("Browse", GUILayout.Width(70)))
                {
                    string selectedPath = UnityEditor.EditorUtility.OpenFolderPanel("Select Folder", _defaultPath, "");
                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        newPath = selectedPath;
                    }
                }

                _path = newPath;
            }
            EditorGUILayout.EndHorizontal();

            return _path;
        }
        /// <summary>
        /// Draws a file path field with a label, text input, and browse button (IMGUI Rect version).
        /// </summary>
        /// <param name="_position">Rectangle to draw in</param>
        /// <param name="_label">Label text to display</param>
        /// <param name="_path">Current file path</param>
        /// <param name="_extension">File extension filter (e.g., "xlsx,xls" for Excel files)</param>
        /// <param name="_defaultPath">Default directory to open (optional)</param>
        public static string DrawFilePathField(Rect _position, string _label, string _path, string _extension = "", string _defaultPath = "")
        {
            float buttonWidth = 70;
            float spacing = 5;

            Rect labelRect = new Rect(_position.x, _position.y, EditorGUIUtility.labelWidth, _position.height);
            Rect textRect = new Rect(_position.x + EditorGUIUtility.labelWidth, _position.y, _position.width - EditorGUIUtility.labelWidth - buttonWidth - spacing, _position.height);
            Rect buttonRect = new Rect(_position.x + _position.width - buttonWidth, _position.y, buttonWidth, _position.height);

            EditorGUI.LabelField(labelRect, _label);
            string newPath = EditorGUI.TextField(textRect, _path);

            if (GUI.Button(buttonRect, "Browse"))
            {
                string selectedPath = UnityEditor.EditorUtility.OpenFilePanel("Select File", _defaultPath, _extension);
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    newPath = selectedPath;
                }
            }

            return newPath;
        }
        /// <summary>
        /// Draws a folder path field with a label, text input, and browse button (IMGUI Rect version).
        /// </summary>
        /// <param name="_position">Rectangle to draw in</param>
        /// <param name="_label">Label text to display</param>
        /// <param name="_path">Current folder path</param>
        /// <param name="_defaultPath">Default directory to open (optional)</param>
        public static string DrawFolderPathField(Rect _position, string _label, string _path, string _defaultPath = "")
        {
            float buttonWidth = 70;
            float spacing = 5;

            Rect labelRect = new Rect(_position.x, _position.y, EditorGUIUtility.labelWidth, _position.height);
            Rect textRect = new Rect(_position.x + EditorGUIUtility.labelWidth, _position.y, _position.width - EditorGUIUtility.labelWidth - buttonWidth - spacing, _position.height);
            Rect buttonRect = new Rect(_position.x + _position.width - buttonWidth, _position.y, buttonWidth, _position.height);

            EditorGUI.LabelField(labelRect, _label);
            string newPath = EditorGUI.TextField(textRect, _path);

            if (GUI.Button(buttonRect, "Browse"))
            {
                string selectedPath = UnityEditor.EditorUtility.OpenFolderPanel("Select Folder", _defaultPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                {
                    newPath = selectedPath;
                }
            }

            return newPath;
        }
        /// <summary>
        /// Draws a horizontal separator line.
        /// </summary>
        /// <param name="_thickness">Line thickness in pixels</param>
        /// <param name="_padding">Vertical padding before and after the line</param>
        /// <param name="_color">Line color (optional, defaults to gray)</param>
        public static void DrawSeparator(float _thickness = 1f, float _padding = 5f, Color? _color = null)
        {
            EditorGUILayout.Space(_padding);
            Rect rect = EditorGUILayout.GetControlRect(false, _thickness);
            EditorGUI.DrawRect(rect, _color ?? new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Space(_padding);
        }
        /// <summary>
        /// Draws a horizontal separator line (IMGUI Rect version).
        /// </summary>
        /// <param name="_position">Rectangle to draw in</param>
        /// <param name="_color">Line color (optional, defaults to gray)</param>
        public static void DrawSeparator(Rect _position, Color? _color = null)
        {
            EditorGUI.DrawRect(_position, _color ?? new Color(0.5f, 0.5f, 0.5f, 1));
        }
        /// <summary>
        /// Draws a search field with a label, text input, and clear button.
        /// </summary>
        /// <param name="_label">Label text to display (e.g., "Search")</param>
        /// <param name="_searchText">Current search text</param>
        /// <param name="_placeholder">Placeholder text when search is empty (optional)</param>
        public static string DrawSearchField(string _label, string _searchText, string _placeholder = "")
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(_label, GUILayout.Width(EditorGUIUtility.labelWidth));

                // Draw placeholder if search text is empty
                if (string.IsNullOrEmpty(_searchText) && !string.IsNullOrEmpty(_placeholder))
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        // Draw placeholder
                        if (string.IsNullOrEmpty(_searchText))
                        {
                            Rect textRect = EditorGUILayout.GetControlRect();
                            _searchText = EditorGUI.TextField(textRect, _searchText);
                            EditorGUI.LabelField(textRect, _placeholder, placeholderStyle);
                        }
                        else
                        {
                            _searchText = EditorGUILayout.TextField(_searchText);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    _searchText = EditorGUILayout.TextField(_searchText);
                }

                // Clear button
                if (GUILayout.Button("×", GUILayout.Width(20)))
                {
                    _searchText = "";
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndHorizontal();

            return _searchText;
        }
        /// <summary>
        /// Draws a search field with a label, text input, and clear button (IMGUI Rect version).
        /// </summary>
        /// <param name="_position">Rectangle to draw in</param>
        /// <param name="_label">Label text to display</param>
        /// <param name="_searchText">Current search text</param>
        /// <param name="_placeholder">Placeholder text when search is empty (optional)</param>
        public static string DrawSearchField(Rect _position, string _label, string _searchText, string _placeholder = "")
        {
            float buttonWidth = 20;
            float spacing = 5;

            Rect labelRect = new Rect(_position.x, _position.y, EditorGUIUtility.labelWidth, _position.height);
            Rect textRect = new Rect(_position.x + EditorGUIUtility.labelWidth, _position.y, _position.width - EditorGUIUtility.labelWidth - buttonWidth - spacing, _position.height);
            Rect buttonRect = new Rect(_position.x + _position.width - buttonWidth, _position.y, buttonWidth, _position.height);

            EditorGUI.LabelField(labelRect, _label);
            
            string newSearchText = EditorGUI.TextField(textRect, _searchText);
            
            // Draw placeholder if search text is empty
            if (string.IsNullOrEmpty(_searchText) && !string.IsNullOrEmpty(_placeholder))
            {
                EditorGUI.LabelField(textRect, _placeholder, placeholderStyle);
            }

            if (GUI.Button(buttonRect, "×"))
            {
                newSearchText = "";
                GUI.FocusControl(null);
            }

            return newSearchText;
        }
    }
}