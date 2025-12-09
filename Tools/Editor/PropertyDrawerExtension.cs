// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

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
    public class PropertyDrawerExtension : PropertyDrawer
    {
        public sealed override float GetPropertyHeight([NotNull] SerializedProperty _property, GUIContent _label)
        {
            TryFlattenProperty(_property);
            TryCuttingTheLabel(_label);
            return GetPropertyHeightInternal(_property, _label);
        }
        public sealed override void OnGUI(Rect _position, [NotNull] SerializedProperty _property, GUIContent _label)
        {
            TryFlattenProperty(_property);
            TryCuttingTheLabel(_label);
            OnGUIInternal(_position, _property, _label);
        }
        
        
        protected virtual float GetPropertyHeightInternal([NotNull] SerializedProperty _property, GUIContent _label)
        {
            return EditorGUI.GetPropertyHeight(_property, _label, true);
        }
        protected virtual void OnGUIInternal(Rect _position, [NotNull] SerializedProperty _property, GUIContent _label)
        {
            EditorGUI.PropertyField(_position, _property, _label, true);
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