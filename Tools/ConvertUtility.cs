// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame
{
    public static class ConvertUtility
    {
        // Separators for parsing arrays/lists at different depths
        [NotNull] private static readonly char[] _k_separators = new char[] { ';', '|', ':' };
        
        
        /// <summary>
        /// Convert string to specified type T
        /// </summary>
        /// <remarks>
        /// <para>Still boxes/unboxes for value types.</para>
        /// </remarks>
        public static T ConvertStringToType<T>(string _value, string _contextName = null)
        {
            object result = ConvertStringToType(_value, typeof(T), _contextName);
            if (result is T typedResult)
                return typedResult;

            return default;
        }
        /// <summary>
        /// Convert string to specified type
        /// </summary>
        public static object ConvertStringToType(string _value, Type _type, string _contextName = null, int _depth = 0)
        {
            if (string.IsNullOrEmpty(_value) || _type == null)
                return null;
            
            _contextName ??= "Value";
            if (_depth >= _k_separators.Length)
            {
                Console.LogWarning(SystemNames.Utility, $"Exceeded maximum depth for parsing '{_contextName}' of type '{_type.Name}'");
                return null;
            }

            // Handle different field types
            if (_type.GetMethod(
                    "TryParse",
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new[] { typeof(string), _type.MakeByRefType() },
                    null) is MethodInfo tryParseMethod && tryParseMethod.ReturnType == typeof(bool))
            {
                object[] parameters = new object[] { _value, null };
                bool success = (bool)tryParseMethod.Invoke(null, parameters);

                if (success)
                    return parameters[1];

                Console.LogWarning(SystemNames.Utility, $"Failed to parse {_type.Name} value '{_value}' for '{_contextName}' using TryParse");
            }
            else if (_type == typeof(string))
            {
                return _value;
            }
            else if (_type == typeof(Vector2))
            {
                if (GeometryUtility.TryParse(_value, out Vector2 vector2Value))
                    return vector2Value;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse Vector2 value '{_value}' for '{_contextName}', expected format: (x,y)");
            }
            else if (_type == typeof(Vector3))
            {
                if (GeometryUtility.TryParse(_value, out Vector3 vector3Value))
                    return vector3Value;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse Vector3 value '{_value}' for '{_contextName}', expected format: (x,y,z)");
            }
            else if (_type == typeof(Vector4))
            {
                if (GeometryUtility.TryParse(_value, out Vector4 vector4Value))
                    return vector4Value;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse Vector4 value '{_value}' for '{_contextName}', expected format: (x,y,z,w)");
            }
            else if (_type == typeof(Vector2Int))
            {
                if (GeometryUtility.TryParse(_value, out Vector2Int vector2IntValue))
                    return vector2IntValue;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse Vector2Int value '{_value}' for '{_contextName}', expected format: (x,y)");
            }
            else if (_type == typeof(Vector3Int))
            {
                if (GeometryUtility.TryParse(_value, out Vector3Int vector3IntValue))
                    return vector3IntValue;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse Vector3Int value '{_value}' for '{_contextName}', expected format: (x,y,z)");
            }
            else if (_type == typeof(Rect))
            {
                if (GeometryUtility.TryParse(_value, out Rect rectValue))
                    return rectValue;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse Rect value '{_value}' for '{_contextName}', expected format: (x,y,width,height)");
            }
            else if (_type == typeof(Bounds))
            {
                if (GeometryUtility.TryParse(_value, out Bounds boundsValue))
                    return boundsValue;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse Bounds value '{_value}' for '{_contextName}', expected format: (centerX,centerY,centerZ),(sizeX,sizeY,sizeZ)");
            }
            else if (_type == typeof(Color))
            {
                if (ColorUtility.TryParseHtmlString(_value, out Color colorValue))
                    return colorValue;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse Color value '{_value}' for '{_contextName}', expected format: #RRGGBB or #RRGGBBAA");
            }
            else if (_type == typeof(Color32))
            {
                if (ColorUtility.TryParseHtmlString(_value, out Color color32Value))
                    return color32Value;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse Color32 value '{_value}' for '{_contextName}', expected format: #RRGGBB or #RRGGBBAA");
            }
            else if (_type.IsEnum)
            {
                if (Enum.TryParse(_type, _value, out object enumValue))
                    return enumValue;

                Console.LogWarning(SystemNames.Utility, $"Failed to parse enum value '{_value}' for '{_contextName}' of type '{_type.Name}'");
            }
            else if (_type.IsArray)
            {
                Type elementType = _type.GetElementType();
                string[] stringValues = _value.Split(new[] { _k_separators[_depth] }, StringSplitOptions.RemoveEmptyEntries);
                Array array = Array.CreateInstance(elementType, stringValues.Length);

                for (int i = 0; i < stringValues.Length; i++)
                {
                    string strVal = stringValues[i].Trim();

                    // Convert string to element type (pass depth + 1 to use next separator)
                    object elementValue = ConvertStringToType(strVal, elementType, $"{_contextName}[{i}]", _depth + 1);

                    if (elementValue != null)
                    {
                        array.SetValue(elementValue, i);
                    }
                }

                return array;
            }
            else if (_type.IsGenericType && _type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = _type.GetGenericArguments()[0];
                string[] stringValues = _value.Split(new[] { _k_separators[_depth] }, StringSplitOptions.RemoveEmptyEntries);

                // Create List<T> instance
                IList list = (IList)Activator.CreateInstance(_type, true);

                for (int i = 0; i < stringValues.Length; i++)
                {
                    string strVal = stringValues[i].Trim();

                    // Convert string to element type (pass depth + 1 to use next separator)
                    object elementValue = ConvertStringToType(strVal, elementType, $"{_contextName}[{i}]", _depth + 1);

                    if (elementValue != null)
                    {
                        list.Add(elementValue);
                    }
                }

                return list;
            }
            else
            {
                try
                {
                    return Convert.ChangeType(_value, _type);
                }
                catch
                {
                    Console.LogWarning(SystemNames.Utility, $"Unsupported type '{_type.Name}' for '{_contextName}'");
                }
            }

            return null;
        }
        
    }
}