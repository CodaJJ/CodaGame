// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace CodaGame.Editor
{
    /// <summary>
    /// A custom property drawer for any object.
    /// </summary>
    /// <remarks>
    /// It's convenient to use this drawer to add some custom processing to unity's default property drawer.
    /// </remarks>
    [CustomPropertyDrawer(typeof(object), true)]
    [CustomPropertyDrawer(typeof(byte), true)]
    [CustomPropertyDrawer(typeof(sbyte), true)]
    [CustomPropertyDrawer(typeof(ushort), true)]
    [CustomPropertyDrawer(typeof(short), true)]
    [CustomPropertyDrawer(typeof(uint), true)]
    [CustomPropertyDrawer(typeof(int), true)]
    [CustomPropertyDrawer(typeof(ulong), true)]
    [CustomPropertyDrawer(typeof(long), true)]
    [CustomPropertyDrawer(typeof(float), true)]
    [CustomPropertyDrawer(typeof(double), true)]
    [CustomPropertyDrawer(typeof(bool), true)]
    [CustomPropertyDrawer(typeof(char), true)]
    [CustomPropertyDrawer(typeof(string), true)]
    [CustomPropertyDrawer(typeof(InspectorList<>), true)]
    public class PropertyDrawerExtension : PropertyDrawer
    {
        public sealed override float GetPropertyHeight([NotNull] SerializedProperty _property, GUIContent _label)
        {
            if (!ShouldShow(_property))
                return 0f;

            if (shouldFlatten)
                TryFlattenProperty(_property);
            TryCuttingTheLabel(_label);
            return GetPropertyHeightInternal(_property, _label);
        }
        public sealed override void OnGUI(Rect _position, [NotNull] SerializedProperty _property, GUIContent _label)
        {
            if (!ShouldShow(_property))
                return;

            if (shouldFlatten)
                TryFlattenProperty(_property);
            TryCuttingTheLabel(_label);
            OnGUIInternal(_position, _property, _label);
        }


        /// <summary>
        /// Whether single-child properties should be flattened (the SerializedProperty handed to
        /// <see cref="OnGUIInternal"/> / <see cref="GetPropertyHeightInternal"/> is advanced to the
        /// sole child). Useful for thin wrappers like <see cref="InspectorList{T}"/> where the wrapper
        /// itself is not worth a foldout. Override to <c>false</c> in drawers that render the wrapper
        /// type itself (the wrapper has meaningful Inspector UI of its own).
        /// </summary>
        protected virtual bool shouldFlatten { get { return true; } }


        protected virtual float GetPropertyHeightInternal([NotNull] SerializedProperty _property, GUIContent _label)
        {
            return EditorGUI.GetPropertyHeight(_property, _label, true);
        }
        protected virtual void OnGUIInternal(Rect _position, [NotNull] SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.PropertyField(_position, _property, _label, true);
        }
        
        
        private bool ShouldShow([NotNull] SerializedProperty _property)
        {
            if (fieldInfo == null)
                return true;

            ShowIfAttribute showIf = (ShowIfAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(ShowIfAttribute));
            if (showIf == null || string.IsNullOrEmpty(showIf.conditionFieldName))
                return true;

            SerializedProperty conditionProperty = FindConditionProperty(_property, showIf.conditionFieldName);
            if (conditionProperty == null || conditionProperty.propertyType != SerializedPropertyType.Boolean)
                return true;

            return conditionProperty.boolValue == showIf.expectedValue;
        }
        private SerializedProperty FindConditionProperty([NotNull] SerializedProperty _property, [NotNull] string _conditionFieldName)
        {
            SerializedObject serializedObject = _property.serializedObject;
            
            // Global path (for cases like "m_settings.m_useAdvanced").
            SerializedProperty conditionProperty = serializedObject.FindProperty(_conditionFieldName);
            if (conditionProperty != null)
                return conditionProperty;

            // Relative path lookup from current field upward to support nested structs/classes.
            string currentPath = _property.propertyPath;
            int splitIndex = currentPath.LastIndexOf('.');
            while (splitIndex >= 0)
            {
                string parentPath = currentPath.Substring(0, splitIndex);
                string candidatePath = $"{parentPath}.{_conditionFieldName}";

                conditionProperty = serializedObject.FindProperty(candidatePath);
                if (conditionProperty != null)
                    return conditionProperty;

                currentPath = parentPath;
                splitIndex = currentPath.LastIndexOf('.');
            }

            return null;
        }
        /// <summary>
        /// Try to flatten the property if it has only one child.
        /// </summary>
        private void TryFlattenProperty([NotNull] SerializedProperty _property)
        {
            SerializedProperty copy = _property.Copy();
            
            // Check if the property only one child.
            if (!copy.NextVisible(true))
                return;
            if (copy.depth <= _property.depth)
                return;
            int depth = copy.depth;
            if (copy.NextVisible(false) && copy.depth >= depth)
                return;

            // Flatten the property.
            _property.NextVisible(true);
        }
        /// <summary>
        /// Try to cut the label to the last part after underscore.
        /// </summary>
        private void TryCuttingTheLabel(GUIContent _label)
        {
            string name = _label.text;
            if (!name.StartsWith("M_"))
                return;
            
            _label.text = name.Substring(2).ToUpperFirstChar();
        }
    }
}
