// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace CodaGame.Editor
{
    [CustomPropertyDrawer(typeof(RadianAttribute), true)]
    public class RadianDrawer : PropertyDrawerExtension
    {
        protected override void OnGUIInternal(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.BeginProperty(_position, _label, _property);
            {
                if (_property.propertyType == SerializedPropertyType.Float)
                {
                    float degrees = _property.floatValue * Mathf.Rad2Deg;

                    float newDegrees;
                    EditorGUI.BeginChangeCheck();
                    {
                        newDegrees = EditorGUI.FloatField(_position, _label, degrees);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_property.serializedObject.targetObject, "Modify Radian Value");
                        _property.floatValue = newDegrees * Mathf.Deg2Rad;
                        _property.serializedObject.ApplyModifiedProperties();
                    }
                }
                else
                    EditorGUI.LabelField(_position, _label.text, "Use Radian with float only");
            }
            EditorGUI.EndProperty();
        }
        protected override float GetPropertyHeightInternal(SerializedProperty _property, GUIContent _label)
        {
            return EditorGUI.GetPropertyHeight(_property, _label);
        }
    }
}