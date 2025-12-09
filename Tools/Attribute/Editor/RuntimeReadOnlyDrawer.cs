// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace CodaGame.Editor
{
    [CustomPropertyDrawer(typeof(RuntimeReadOnlyAttribute))]
    public class RuntimeReadOnlyDrawer : PropertyDrawerExtension
    {
        protected sealed override void OnGUIInternal(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            bool previousGUIState = GUI.enabled;
        
            if (Application.isPlaying)
                GUI.enabled = false;
            EditorGUI.PropertyField(_position, _property, _label, true);
            GUI.enabled = previousGUIState;
        }
    
        protected override float GetPropertyHeightInternal(SerializedProperty _property, GUIContent _label)
        {
            return EditorGUI.GetPropertyHeight(_property, _label, true);
        }
    }
}