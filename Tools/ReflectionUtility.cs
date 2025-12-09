// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    public class ReflectionUtility
    {
        /// <summary>
        /// Gets all fields that Unity's serialization system will serialize.
        /// </summary>
        /// <param name="_type">The type to get serializable fields from</param>
        /// <returns>List of fields that Unity will serialize</returns>
        [ItemNotNull, NotNull]
        public static List<FieldInfo> GetSerializableFields(Type _type)
        {
            if (_type == null)
                return new List<FieldInfo>(0);

            FieldInfo[] allFields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            List<FieldInfo> serializableFields = new List<FieldInfo>(allFields.Length);

            foreach (FieldInfo field in allFields)
            {
                // Exclude [NonSerialized] fields
                if (field.GetCustomAttribute<NonSerializedAttribute>() != null)
                    continue;

                // Exclude readonly fields (Unity doesn't serialize readonly)
                if (field.IsInitOnly)
                    continue;

                // Include if public OR has [SerializeField]
                if (field.IsPublic || field.GetCustomAttribute<SerializeField>() != null)
                {
                    // Verify the field's type is actually serializable by Unity
                    if (CheckTypeIsSerializable(field.FieldType))
                        serializableFields.Add(field);
                }
            }

            return serializableFields;
        }
        /// <summary>
        /// Checks if a type can be serialized by Unity's serialization system.
        /// </summary>
        /// <param name="_type">The type to check</param>
        /// <returns>True if the type is serializable by Unity</returns>
        public static bool CheckTypeIsSerializable(Type _type)
        {
            if (_type == null)
                return false;

            // Primitive types (int, float, bool, byte, etc.)
            if (_type.IsPrimitive)
                return true;

            // String
            if (_type == typeof(string))
                return true;

            // Enums (all enums are serializable)
            if (_type.IsEnum)
                return true;

            // UnityEngine.Object derivatives (GameObject, Component, ScriptableObject, etc.)
            if (typeof(UnityEngine.Object).IsAssignableFrom(_type))
                return true;

            // Unity built-in serializable types
            if (IsUnityBuiltInSerializableType(_type))
                return true;

            // Arrays of serializable types
            if (_type.IsArray)
            {
                Type elementType = _type.GetElementType();
                return CheckTypeIsSerializable(elementType);
            }

            // List<T> (the only generic collection Unity serializes natively)
            if (_type.IsGenericType && _type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type[] genericArgs = _type.GetGenericArguments();
                return genericArgs.Length == 1 && CheckTypeIsSerializable(genericArgs[0]);
            }

            // Custom classes and structs with [Serializable] attribute
            if (_type.GetCustomAttribute<SerializableAttribute>() != null)
            {
                // Structs with [Serializable] are serializable
                if (_type.IsValueType)
                    return true;

                // Classes must not be abstract or generic type definitions
                if (!_type.IsAbstract && !_type.IsGenericTypeDefinition)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Gets all subclasses of a parent type, including support for open generic types.
        /// </summary>
        /// <param name="_parentType">The parent type to find subclasses of (can be open generic like List&lt;&gt;)</param>
        /// <param name="_filter">Optional filter predicate</param>
        /// <returns>Array of all subclass types</returns>
        [ItemNotNull, NotNull, Pure]
        public static Type[] GetAllSubclasses(Type _parentType, NotNullPredicate<Type> _filter = null)
        {
            if (_parentType == null)
                return Array.Empty<Type>();

            bool isOpenGeneric = _parentType.IsGenericTypeDefinition;
            List<Type> result = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                if (assembly == null)
                    continue;

                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    // Skip assemblies that fail to load types
                    continue;
                }

                foreach (Type type in types)
                {
                    if (_parentType == type)
                        continue;

                    bool isSubclass = false;

                    // Handle open generic types
                    if (isOpenGeneric)
                    {
                        Type currentType = type;
                        while (currentType != null && currentType != typeof(object))
                        {
                            if (currentType.IsGenericType && currentType.GetGenericTypeDefinition() == _parentType)
                            {
                                isSubclass = true;
                                break;
                            }
                            currentType = currentType.BaseType;
                        }
                    }
                    else
                    {
                        // Standard subclass check for non-generic types
                        isSubclass = _parentType.IsAssignableFrom(type);
                    }

                    if (isSubclass)
                    {
                        if (_filter == null || _filter(type))
                            result.Add(type);
                    }
                }
            }

            return result.ToArray();
        }
        
        
        /// <summary>
        /// Checks if a type is a Unity built-in serializable type.
        /// </summary>
        private static bool IsUnityBuiltInSerializableType(Type _type)
        {
            return _type == typeof(Vector2) ||
                   _type == typeof(Vector3) ||
                   _type == typeof(Vector4) ||
                   _type == typeof(Vector2Int) ||
                   _type == typeof(Vector3Int) ||
                   _type == typeof(Quaternion) ||
                   _type == typeof(Matrix4x4) ||
                   _type == typeof(Color) ||
                   _type == typeof(Color32) ||
                   _type == typeof(LayerMask) ||
                   _type == typeof(Rect) ||
                   _type == typeof(RectInt) ||
                   _type == typeof(Bounds) ||
                   _type == typeof(BoundsInt) ||
                   _type == typeof(AnimationCurve) ||
                   _type == typeof(Gradient) ||
                   _type == typeof(RectOffset) ||
                   _type == typeof(GUIStyle);
        }
    }
}