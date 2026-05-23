// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodaGame.Editor
{
    /// <summary>
    /// Property drawer for <see cref="MultiStateShow{T_STATE}"/>. Auto-syncs the serialized entry list to the
    /// enum values (one entry per value, in declaration order), hides the per-entry "state" field, and shows
    /// one labelled GameObject list per enum value — so the Inspector workflow is "drop GameObjects into the
    /// right row" with no enum picker.
    /// </summary>
    /// <remarks>
    /// <para>The sync runs every <see cref="OnGUI"/>:</para>
    /// <list type="bullet">
    /// <item>If the entry count or per-index state values don't match the current enum declaration, the list
    ///       is rebuilt. Targets are preserved by matching old <c>state</c> → new <c>state</c>, so reordering
    ///       enum values, adding new ones, or deleting old ones is handled transparently.</item>
    /// <item>If the field is collapsed (foldout closed), sync still runs — it's cheap and keeps disk state
    ///       canonical.</item>
    /// </list>
    /// <para>Registered for the open generic <c>MultiStateShow&lt;&gt;</c> via the <c>useForChildren</c>
    /// overload; Unity 2020.1+ resolves the drawer for any concrete instantiation.</para>
    /// </remarks>
    [CustomPropertyDrawer(typeof(MultiStateShow<>), true)]
    internal sealed class MultiStateShowDrawer : PropertyDrawerExtension
    {
        private const string _k_entriesField = "_m_entries";
        private const string _k_stateField   = "state";
        private const string _k_targetsField = "targets";

        private const float _k_rowSpacing = 2f;


        // The base class flattens single-child properties to their sole child. We render the MultiStateShow
        // root itself (foldout + one row per enum value), so we need the property as-passed.
        protected override bool shouldFlatten { get { return false; } }


        protected override float GetPropertyHeightInternal(SerializedProperty _property, GUIContent _label)
        {
            float headerHeight = EditorGUIUtility.singleLineHeight;
            if (!_property.isExpanded)
                return headerHeight;

            Type enumType = ResolveEnumType();
            if (enumType == null)
                return headerHeight + EditorGUIUtility.singleLineHeight; // error line

            Array values = Enum.GetValues(enumType);
            SerializedProperty entries = _property.FindPropertyRelative(_k_entriesField);
            if (entries == null)
                return headerHeight + EditorGUIUtility.singleLineHeight;

            EnsureEntries(entries, values);

            float total = headerHeight + _k_rowSpacing;
            for (int i = 0; i < values.Length; i++)
            {
                SerializedProperty targets = entries.GetArrayElementAtIndex(i).FindPropertyRelative(_k_targetsField);
                total += EditorGUI.GetPropertyHeight(targets, true) + _k_rowSpacing;
            }
            return total;
        }
        protected override void OnGUIInternal(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            Rect headerRect = new Rect(_position.x, _position.y, _position.width, EditorGUIUtility.singleLineHeight);
            _property.isExpanded = EditorGUI.Foldout(headerRect, _property.isExpanded, _label, true);
            if (!_property.isExpanded)
                return;

            Type enumType = ResolveEnumType();
            if (enumType == null)
            {
                Rect errRect = new Rect(_position.x, _position.y + EditorGUIUtility.singleLineHeight, _position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.HelpBox(errRect, "MultiStateShow: could not resolve enum type from field.", MessageType.Error);
                return;
            }

            Array values = Enum.GetValues(enumType);
            SerializedProperty entries = _property.FindPropertyRelative(_k_entriesField);
            if (entries == null)
                return;

            EnsureEntries(entries, values);

            float y = _position.y + EditorGUIUtility.singleLineHeight + _k_rowSpacing;
            EditorGUI.indentLevel++;
            for (int i = 0; i < values.Length; i++)
            {
                SerializedProperty entry = entries.GetArrayElementAtIndex(i);
                SerializedProperty targets = entry.FindPropertyRelative(_k_targetsField);
                float h = EditorGUI.GetPropertyHeight(targets, true);
                Rect rowRect = new Rect(_position.x, y, _position.width, h);
                EditorGUI.PropertyField(rowRect, targets, new GUIContent(ObjectNames.NicifyVariableName(values.GetValue(i).ToString())), true);
                y += h + _k_rowSpacing;
            }
            EditorGUI.indentLevel--;
        }


        // Walk the field's declared type up to MultiStateShow<TEnum> and return the TEnum argument. Handles
        // both direct fields (MultiStateShow<E> field) and nested cases (List<MultiStateShow<E>> elements
        // use Generic Type Definition lookup via fieldInfo on the list's element type — but Unity routes
        // those through a different code path, so we mostly handle the direct case here).
        private Type ResolveEnumType()
        {
            Type t = fieldInfo?.FieldType;
            while (t != null && t != typeof(object))
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(MultiStateShow<>))
                    return t.GetGenericArguments()[0];
                t = t.BaseType;
            }
            // Fallback: list / array elements.
            Type elementType = t == null ? null : t;
            if (fieldInfo != null)
            {
                Type ft = fieldInfo.FieldType;
                if (ft.IsArray)
                    elementType = ft.GetElementType();
                else if (ft.IsGenericType && ft.GetGenericTypeDefinition() == typeof(List<>))
                    elementType = ft.GetGenericArguments()[0];
            }
            while (elementType != null && elementType != typeof(object))
            {
                if (elementType.IsGenericType && elementType.GetGenericTypeDefinition() == typeof(MultiStateShow<>))
                    return elementType.GetGenericArguments()[0];
                elementType = elementType.BaseType;
            }
            return null;
        }
        // Ensure the entries array has exactly one element per enum value, in declaration order, with
        // matching `state` ints. Preserves target lists across enum mutations by snapshotting them by state
        // value before rebuilding.
        private static void EnsureEntries(SerializedProperty _entries, Array _enumValues)
        {
            // Quick path: already canonical?
            if (_entries.arraySize == _enumValues.Length)
            {
                bool ok = true;
                for (int i = 0; i < _enumValues.Length; i++)
                {
                    SerializedProperty stateProp = _entries.GetArrayElementAtIndex(i).FindPropertyRelative(_k_stateField);
                    if (stateProp.intValue != Convert.ToInt32(_enumValues.GetValue(i)))
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                    return;
            }

            // Snapshot existing entries: state int → target object references.
            Dictionary<int, UnityEngine.Object[]> snapshot = new Dictionary<int, UnityEngine.Object[]>();
            for (int i = 0; i < _entries.arraySize; i++)
            {
                SerializedProperty entry = _entries.GetArrayElementAtIndex(i);
                int state = entry.FindPropertyRelative(_k_stateField).intValue;
                SerializedProperty targets = entry.FindPropertyRelative(_k_targetsField);
                UnityEngine.Object[] objs = new UnityEngine.Object[targets.arraySize];
                for (int j = 0; j < targets.arraySize; j++)
                    objs[j] = targets.GetArrayElementAtIndex(j).objectReferenceValue;
                snapshot[state] = objs;   // duplicate states: last one wins (canonical form has none anyway)
            }

            // Resize to enum cardinality and rebuild each entry.
            _entries.arraySize = _enumValues.Length;
            for (int i = 0; i < _enumValues.Length; i++)
            {
                int stateInt = Convert.ToInt32(_enumValues.GetValue(i));
                SerializedProperty entry = _entries.GetArrayElementAtIndex(i);
                entry.FindPropertyRelative(_k_stateField).intValue = stateInt;

                SerializedProperty targets = entry.FindPropertyRelative(_k_targetsField);
                if (snapshot.TryGetValue(stateInt, out UnityEngine.Object[] objs))
                {
                    targets.arraySize = objs.Length;
                    for (int j = 0; j < objs.Length; j++)
                        targets.GetArrayElementAtIndex(j).objectReferenceValue = objs[j];
                }
                else
                {
                    targets.arraySize = 0;
                }
            }
        }
    }
}
