// Copyright (c) 2026 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace CodaGame.Editor
{
    [CustomPropertyDrawer(typeof(RangeAttribute), true)]
    public class RangeDrawer : PropertyDrawerExtension
    {
        protected override void OnGUIInternal(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            RangeAttribute target = (RangeAttribute)attribute;

            EditorGUI.BeginProperty(_position, _label, _property);
            {
                if (_property.propertyType == SerializedPropertyType.Float)
                {
                    _property.floatValue = EditorGUI.Slider(_position, _label, _property.floatValue, target.min, target.max);
                }
                else if (_property.propertyType == SerializedPropertyType.Integer)
                {
                    int min = Mathf.RoundToInt(target.min);
                    int max = Mathf.RoundToInt(target.max);
                    _property.intValue = EditorGUI.IntSlider(_position, _label, _property.intValue, min, max);
                }
                else
                {
                    EditorGUI.LabelField(_position, _label.text, "Use Range with float or int only");
                }
            }
            EditorGUI.EndProperty();
        }

        protected override float GetPropertyHeightInternal(SerializedProperty _property, GUIContent _label)
        {
            return EditorGUI.GetPropertyHeight(_property, _label, true);
        }
    }
}
