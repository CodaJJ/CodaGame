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
    public class PropertyDrawerExtension : PropertyDrawer
    {
        public override float GetPropertyHeight([NotNull] SerializedProperty _property, GUIContent _label)
        {
            TryFlattenProperty(_property, _label);
            return EditorGUI.GetPropertyHeight(_property, _label);
        }
        public override void OnGUI(Rect _position, [NotNull] SerializedProperty _property, GUIContent _label)
        {
            TryFlattenProperty(_property, _label);
            EditorGUI.PropertyField(_position, _property, _label, true);
        }


        /// <summary>
        /// Try to flatten the property if it has only one child.
        /// </summary>
        private void TryFlattenProperty([NotNull] SerializedProperty _property, GUIContent _label)
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
    }
}