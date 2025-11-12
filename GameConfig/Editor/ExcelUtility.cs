// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ExcelDataReader;
using JetBrains.Annotations;

namespace CodaGame.Editor
{
    public static class ExcelUtility
    {
        /// <summary>
        /// Read constants config from an Excel sheet.
        /// </summary>
        /// <param name="_excelFilePath">The path to the Excel file.</param>
        /// <param name="_sheetName">The name of the sheet to read.</param>
        /// <param name="_config">The config instance to populate.</param>
        /// <param name="_skipRows">Number of rows to skip after the header.</param>
        public static void ReadConstantsFromExcel<T_CONFIG>(string _excelFilePath, string _sheetName, T_CONFIG _config, int _skipRows = 0)
            where T_CONFIG : _AConstantsConfig
        {
            if (_config == null)
            {
                Console.LogError(SystemNames.Config, "Excel read failed, the config instance is null.");
                return;
            }
            
            if (string.IsNullOrEmpty(_excelFilePath))
            {
                Console.LogError(SystemNames.Config, "Excel read failed, the file path is null or empty.");  
                return;
            }
            
            if (!File.Exists(_excelFilePath))
            {
                Console.LogError(SystemNames.Config, $"Excel file not found: {_excelFilePath}");
                return;
            }
            
            string tempFilePath = FileUtility.CreateTempCopy(_excelFilePath);

            try
            {
                using FileStream stream = File.Open(tempFilePath, FileMode.Open, FileAccess.Read);
                using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);

                if (reader == null)
                {
                    Console.LogError(SystemNames.Config, $"Excel read failed, unable to create reader for file: {_excelFilePath}");  
                    return;
                }
                
                do
                {
                    if (reader.Name != _sheetName)
                        continue;

                    ReadConstantsFromSheet(reader, _config, _skipRows);
                    return;
                } while (reader.NextResult());

                Console.LogError(SystemNames.Config, $"Sheet '{_sheetName}' not found in {_excelFilePath}");
            }
            catch (Exception e)
            {
                Console.LogError(SystemNames.Config, $"Excel read failed for file '{_excelFilePath}': {e.Message}");
            }
            finally
            {
                FileUtility.DeleteTempCopy(tempFilePath);
            }
        }
        /// <summary>
        /// Read table config from an Excel sheet.
        /// </summary>
        /// <param name="_excelFilePath">The path to the Excel file.</param>
        /// <param name="_sheetName">The name of the sheet to read.</param>
        /// <param name="_config">The config instance to populate.</param>
        /// <param name="_skipRows">Number of rows to skip after the header.</param>
        public static void ReadTableFromExcel<T_CONFIG, T_DATA>(string _excelFilePath, string _sheetName, T_CONFIG _config, int _skipRows = 0)
            where T_CONFIG : _ATableConfig<T_DATA>
        {
            if (_config == null)
            {
                Console.LogError(SystemNames.Config, "Excel read failed, the config instance is null.");
                return;
            }
            
            if (string.IsNullOrEmpty(_excelFilePath))
            {
                Console.LogError(SystemNames.Config, "Excel read failed, the file path is null or empty.");  
                return;
            }
            
            if (!File.Exists(_excelFilePath))
            {
                Console.LogError(SystemNames.Config, $"Excel file not found: {_excelFilePath}");
                return;
            }
            
            string tempFilePath = FileUtility.CreateTempCopy(_excelFilePath);
        
            try
            {
                using FileStream stream = File.Open(tempFilePath, FileMode.Open, FileAccess.Read);
                using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream);
        
                if (reader == null)
                {
                    Console.LogError(SystemNames.Config, $"Excel read failed, unable to create reader for file: {_excelFilePath}");  
                    return;
                }
                
                do
                {
                    if (reader.Name != _sheetName)
                        continue;
        
                    ReadTableFromSheet(reader, _config, _skipRows);
                    return;
                } while (reader.NextResult());
        
                Console.LogError(SystemNames.Config, $"Sheet '{_sheetName}' not found in {_excelFilePath}");
            }
            finally
            {
                FileUtility.DeleteTempCopy(tempFilePath);
            }
        }
        

        // Read constants from the current sheet
        private static void ReadConstantsFromSheet<T_CONFIG>([NotNull] IExcelDataReader _reader, [NotNull] T_CONFIG _config, int _skipRows)
            where T_CONFIG : _AConstantsConfig
        {
            // Read header row
            if (!_reader.Read()) return;
            // Skip rows
            for (int i = 0; i < _skipRows; i++)
            {
                if (!_reader.Read()) return;
            }
            
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            do
            {
                string key = _reader.GetValue(0)?.ToString();
                string value = _reader.GetValue(1)?.ToString();
                if (string.IsNullOrEmpty(key))
                    continue;

                if (!keyValues.TryAdd(key, value))
                    Console.LogWarning(SystemNames.Config, "Duplicate key found in constants config: " + key);
            } while (_reader.Read());
            
            // Get all fields from the config type
            Type type = typeof(T_CONFIG);
            List<FieldInfo> fields = ReflectionUtility.GetSerializableFields(type);
            foreach (FieldInfo field in fields)
            {
                if (keyValues.TryGetValue(field.Name, out string valueStr))
                    field.SetValue(_config, ConvertUtility.ConvertStringToType(valueStr, field.FieldType, field.Name));
                else
                    Console.LogWarning(SystemNames.Config, "Key not found for field in constants config: " + field.Name);
            }
        }
        // Read table data from the current sheet
        private static void ReadTableFromSheet<T_DATA>(IExcelDataReader _reader, [NotNull] _ATableConfig<T_DATA> _config, int _skipRows)
        {
            // Read header row
            if (!_reader.Read()) return;
            int rowIndex = 0;
            // Skip rows
            for (; rowIndex < _skipRows; rowIndex++)
            {
                if (!_reader.Read()) return;
            }
        
            // Get field names from header
            List<string> fieldNames = new List<string>();
            for (int i = 0; i < _reader.FieldCount; i++)
            {
                string fieldName = _reader.GetValue(i)?.ToString();
                // Stop if an empty field name is encountered
                if (string.IsNullOrEmpty(fieldName))
                    break;
                
                fieldNames.Add(fieldName);
            }
        
            // Get all fields from the data type
            Type type = typeof(T_DATA);
            List<FieldInfo> fields = ReflectionUtility.GetSerializableFields(type);
            foreach (FieldInfo field in fields)
            {
                if (!fieldNames.Contains(field.Name))
                {
                    Console.LogWarning(SystemNames.Config, $"Field '{field.Name}' in data type '{type.Name}' does not have a corresponding column in the Excel sheet.");
                }
            }
        
            // Initialize the data list
            if (_config.dataList == null)
                _config.dataList = new List<T_DATA>();
            else
                _config.dataList.Clear();
        
            // Read data rows
            while (_reader.Read())
            {
                rowIndex++;
                if (IsRowEmpty(_reader))
                    continue;
                
                T_DATA dataInstance = Activator.CreateInstance<T_DATA>();
        
                // Map Excel columns to data fields
                for (int i = 0; i < fieldNames.Count; i++)
                {
                    string fieldName = fieldNames[i];
                    FieldInfo field = fields.Find(_f => _f.Name == fieldName);
                    if (field != null)
                    {
                        string dataStr = _reader.GetValue(i)?.ToString();
                        if (string.IsNullOrEmpty(dataStr))
                            continue;
                        
                        field.SetValue(dataInstance, ConvertUtility.ConvertStringToType(dataStr, field.FieldType, field.Name + " (Row " + rowIndex + ")"));
                    }
                }
        
                _config.dataList.Add(dataInstance);
            }
        }
        private static bool IsRowEmpty(IExcelDataReader _reader)
        {
            if (_reader == null)
                return true;
            
            for (int i = 0; i < _reader.FieldCount; i++)
            {
                string value = _reader.GetValue(i)?.ToString();
                if (!string.IsNullOrEmpty(value))
                    return false;
            }
            
            return true;
        }
    }
}