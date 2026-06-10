// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
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
        // This drawer is registered for typeof(object) with useForChildren, so PropertyAttribute
        // subclasses match it too — Unity builds a drawer chain with one instance per attribute
        // plus one for the field type, re-entering through EditorGUI.PropertyField /
        // GetPropertyHeight. Only the outermost instance of a property may apply the wrapping
        // behaviors (ReadOnly, Required), otherwise they stack once per chain link.
        private static readonly HashSet<string> _g_wrappingPropertyPaths = new HashSet<string>();


        public sealed override float GetPropertyHeight([NotNull] SerializedProperty _property, GUIContent _label)
        {
            if (!ShouldShow(_property))
                return 0f;

            if (shouldFlatten)
                TryFlattenProperty(_property);
            TryCuttingTheLabel(_label);

            if (!_g_wrappingPropertyPaths.Add(_property.propertyPath))
                return GetPropertyHeightInternal(_property, _label);

            try
            {
                float height = GetPropertyHeightInternal(_property, _label);
                string requiredMessage = GetRequiredErrorMessage(_property);
                if (requiredMessage != null)
                    height += EditorUtility.GetHelpBoxHeight(requiredMessage, true);
                return height;
            }
            finally
            {
                _g_wrappingPropertyPaths.Remove(_property.propertyPath);
            }
        }
        public sealed override void OnGUI(Rect _position, [NotNull] SerializedProperty _property, GUIContent _label)
        {
            if (!ShouldShow(_property))
                return;

            if (shouldFlatten)
                TryFlattenProperty(_property);
            TryCuttingTheLabel(_label);

            if (!_g_wrappingPropertyPaths.Add(_property.propertyPath))
            {
                OnGUIInternal(_position, _property, _label);
                return;
            }

            try
            {
                string requiredMessage = GetRequiredErrorMessage(_property);
                Rect fieldPosition = _position;
                if (requiredMessage != null)
                    fieldPosition.height -= EditorUtility.GetHelpBoxHeight(requiredMessage, true);

                bool previousGUIState = GUI.enabled;
                if (Application.isPlaying && IsRuntimeReadOnly())
                    GUI.enabled = false;
                OnGUIInternal(fieldPosition, _property, _label);
                GUI.enabled = previousGUIState;

                if (requiredMessage == null)
                    return;

                Rect helpBoxPosition = _position;
                helpBoxPosition.y += fieldPosition.height;
                helpBoxPosition.height = EditorUtility.GetHelpBoxHeight(requiredMessage, true);
                EditorUtility.DrawHelpBox(helpBoxPosition, requiredMessage, MessageType.Error, true);
            }
            finally
            {
                _g_wrappingPropertyPaths.Remove(_property.propertyPath);
            }
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
        
        
        /// <summary>
        /// Whether the field is marked with <see cref="RuntimeReadOnlyAttribute"/>.
        /// </summary>
        private bool IsRuntimeReadOnly()
        {
            return fieldInfo != null && Attribute.GetCustomAttribute(fieldInfo, typeof(RuntimeReadOnlyAttribute)) != null;
        }
        /// <summary>
        /// Returns the error message to show when a <see cref="RequiredAttribute"/> field is not wired,
        /// or null when the field is not required / not an object reference / already wired.
        /// </summary>
        private string GetRequiredErrorMessage([NotNull] SerializedProperty _property)
        {
            if (fieldInfo == null)
                return null;

            RequiredAttribute required = (RequiredAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(RequiredAttribute));
            if (required == null)
                return null;
            if (_property.propertyType != SerializedPropertyType.ObjectReference || _property.objectReferenceValue != null)
                return null;

            return string.IsNullOrEmpty(required.message) ? $"'{fieldInfo.Name}' is required. Wire it in the Inspector." : required.message;
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
