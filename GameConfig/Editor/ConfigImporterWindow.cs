// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace CodaGame.Editor
{
    public class ConfigImporterWindow : EditorWindow
    {
        private ConfigImporterSetting _m_setting;
        
        private string _m_searchPattern = string.Empty;
        [ItemNotNull, NotNull] private List<_AConfigImporterItem> _m_allConfigItems;
        
        
        private ConfigImporterWindow()
        {
            _m_allConfigItems = new List<_AConfigImporterItem>();
        }
        
        
        [MenuItem("Tools/Config Importer")]
        private static void Open()
        {
            ConfigImporterWindow window = GetWindow<ConfigImporterWindow>("Config Importer");
            if (window == null)
                throw new InvalidOperationException("Failed to open Config Importer window.");
            
            window.minSize = new Vector2(400, 300);
            window._m_setting = AssetDatabase.LoadAssetAtPath<ConfigImporterSetting>("Assets/Scripts/CodaGame/GameConfig/Editor/ConfigImporterSetting.asset")!;
            if (window._m_setting == null)
            {
                window._m_setting = CreateInstance<ConfigImporterSetting>()!;
                AssetDatabase.CreateAsset(window._m_setting, "Assets/Scripts/CodaGame/GameConfig/Editor/ConfigImporterSetting.asset");
                AssetDatabase.SaveAssets();
            }
            window.CollectAllConfigTypes();
        }
        private void OnGUI()
        {
            if (_m_setting == null)
            {
                EditorUtility.DrawHelpBox("Config Importer Setting asset not found, Reopen the window to create one.", MessageType.Error);
                return;
            }
            
            // Line 1: Output Folder Path
            _m_setting.outputFolderPath = EditorUtility.DrawFolderPathField("Output Folder Path", _m_setting.outputFolderPath, _m_setting.outputFolderPath.absolutePath);
            
            // Line 2: Recollect All Config Types Button
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Recollect All Config Types");
                if (GUILayout.Button("Recollect", GUILayout.MinWidth(150)))
                    CollectAllConfigTypes();
            }
            EditorGUILayout.EndHorizontal();
            
            // Line 3: Cleanup Excel Sheet Mappings Button
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Cleanup Excel Sheet Mappings");
                if (GUILayout.Button("Remove Missing Types", GUILayout.MinWidth(150)))
                {
                    if (UnityEditor.EditorUtility.DisplayDialog(
                            "Confirm Cleanup",
                            "Are you sure you want to cleanup Excel Sheet Mappings? " +
                            "This will remove any mappings that do not correspond to existing config types.",
                            "Yes", "No"))
                        _m_setting.CleanupExcelSheetMappings(_m_allConfigItems.ConvertAll(_item => _item.type.FullName));
                }
            }
            EditorGUILayout.EndHorizontal();
            
            // Separator ===============================================================
            EditorUtility.DrawSeparator();
            
            // Line 1: Search Field For Config Types
            _m_searchPattern = EditorUtility.DrawSearchField("Search Config", _m_searchPattern, "Type to search config files...");
            
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Import All Configs Below");
                if (GUILayout.Button("Import All", GUILayout.MinWidth(150)))
                    ImportAllConfigs(_m_searchPattern);
            }
            EditorGUILayout.EndHorizontal();
            
            // Line 3+: Config Items List
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foreach (_AConfigImporterItem item in _m_allConfigItems)
            {
                if (!string.IsNullOrEmpty(_m_searchPattern) && !item.type.FullName.Contains(_m_searchPattern, StringComparison.OrdinalIgnoreCase))
                    continue;
                
                DrawImporterItem(item);
            }
            EditorGUILayout.EndVertical();
        }

        private void CollectAllConfigTypes()
        {
            Type[] constantsTypes = ReflectionUtility.GetAllSubclasses(typeof(_AConstantsConfig), _type =>
            {
                // Skip types marked with [IgnoreConfigImporter]
                if (_type.GetCustomAttribute<IgnoreConfigImporterAttribute>() != null)
                    return false;

                return !_type.IsAbstract && !_type.IsGenericTypeDefinition;
            });
            Type[] tableTypes = ReflectionUtility.GetAllSubclasses(typeof(_ATableConfig<>), _type =>
            {
                // Skip types marked with [IgnoreConfigImporter]
                if (_type.GetCustomAttribute<IgnoreConfigImporterAttribute>() != null)
                    return false;

                if (_type.IsAbstract || _type.IsGenericTypeDefinition)
                    return false;

                FieldInfo dataListField = _type.GetField("dataList");
                if (dataListField == null)
                {
                    Console.LogError(SystemNames.Config, $"Config table type '{_type.FullName}' does not have a 'dataList' field.");
                    return false;
                }

                Type fieldType = dataListField.FieldType;
                if (!fieldType.IsGenericType)
                {
                    Console.LogError(SystemNames.Config, $"Field 'dataList' in config table type '{_type.FullName}' is not a generic type.");
                    return false;
                }

                Type elementType = fieldType.GetGenericArguments()[0];
                if (!ReflectionUtility.CheckTypeIsSerializable(elementType))
                {
                    Console.LogWarning(SystemNames.Config, $"Element type '{elementType.FullName}' of 'dataList' in config table type '{_type.FullName}' is not serializable.");
                    return false;
                }

                return true;
            });
            _m_allConfigItems = new List<_AConfigImporterItem>(constantsTypes.Length + tableTypes.Length);
            foreach (Type type in constantsTypes)
            {
                ConfigImporterSetting.ExcelSheetMapping excelData = null;
                if (_m_setting != null && _m_setting.excelSheetMappings != null)
                    excelData = _m_setting.excelSheetMappings.Find(_data => _data.typeName == type.FullName);
                
                _m_allConfigItems.Add(new ConstantsConfigImporterItem(type, excelData));
            }
            foreach (Type type in tableTypes)
            {
                ConfigImporterSetting.ExcelSheetMapping excelData = null;
                if (_m_setting != null && _m_setting.excelSheetMappings != null)
                    excelData = _m_setting.excelSheetMappings.Find(_data => _data.typeName == type.FullName);
                
                _m_allConfigItems.Add(new TableConfigImporterItem(type, excelData));
            }
        }
        private void DrawImporterItem([NotNull] _AConfigImporterItem _item)
        {
            _item.foldout = EditorGUILayout.BeginFoldoutHeaderGroup(_item.foldout, _item.type.FullName);
            EditorGUILayout.EndFoldoutHeaderGroup();
            if (!_item.foldout)
                return;

            _item.excelData.filePath = EditorUtility.DrawFilePathField("Excel File Path", _item.excelData.filePath, "xlsx,xls", _item.excelData.filePath.absolutePath);
            _item.excelData.sheetName = EditorGUILayout.TextField("Sheet Name", _item.excelData.sheetName);
            if (GUILayout.Button("Import This Config"))
                _item.ImportConfig(_m_setting);
        }
        private void ImportAllConfigs(string _searchPattern)
        {
            foreach (_AConfigImporterItem item in _m_allConfigItems)
            {
                if (!string.IsNullOrEmpty(_searchPattern) && !item.type.FullName.Contains(_searchPattern, StringComparison.OrdinalIgnoreCase))
                    continue;

                item.ImportConfig(_m_setting);
            }
        }
        
        
        private abstract class _AConfigImporterItem
        {
            [NotNull] private readonly Type _m_type;
            [NotNull] private readonly ConfigImporterSetting.ExcelSheetMapping _m_excelData;
            private readonly bool _m_hasExcelData;

            private bool _m_foldout;


            protected _AConfigImporterItem([NotNull] Type _type, ConfigImporterSetting.ExcelSheetMapping _excelData)
            {
                _m_type = _type;
                _m_excelData = _excelData ?? new ConfigImporterSetting.ExcelSheetMapping()
                {
                    typeName = _m_type.FullName
                };
                _m_hasExcelData = _excelData != null;
            }
            
            
            [NotNull] public Type type { get { return _m_type; } }
            [NotNull] public ConfigImporterSetting.ExcelSheetMapping excelData { get { return _m_excelData; } }
            public bool foldout { get { return _m_foldout; } set { _m_foldout = value; } }


            public void ImportConfig(ConfigImporterSetting _setting)
            {
                // Validate excel data
                if (!File.Exists(excelData.filePath.absolutePath))
                {
                    Console.LogError(SystemNames.Config, $"Excel file path is not set, or file not exists for config type '{type.FullName}'.");
                    return;
                }
                if (string.IsNullOrEmpty(excelData.sheetName))
                {
                    Console.LogError(SystemNames.Config, $"Sheet name is not set for config type '{type.FullName}'.");
                    return;
                }

                if (_setting == null)
                {
                    Console.LogError(SystemNames.Config, "Config import failed, the importer setting is null.");
                    return;
                }
                

                ScriptableObject configAsset = GetOrCreateConfig(_setting.outputFolderPath);
                ImportDataFromExcel(excelData.filePath.absolutePath, excelData.sheetName, configAsset);
                if (!_m_hasExcelData)
                {
                    // Add new mapping to setting
                    _setting.excelSheetMappings.Add(excelData);
                    UnityEditor.EditorUtility.SetDirty(_setting);
                    AssetDatabase.SaveAssets();
                }
            }


            protected abstract void ImportDataFromExcel(string _filePath, string _sheetName, ScriptableObject _configAsset);
            

            private ScriptableObject GetOrCreateConfig(AssetPath _outputFolderPath)
            {
                // Ensure output folder exists
                if (!Directory.Exists(_outputFolderPath.absolutePath))
                {
                    Directory.CreateDirectory(_outputFolderPath.absolutePath);
                    AssetDatabase.Refresh();
                }

                // Generate asset path
                string assetPath = Path.Combine(_outputFolderPath.unityPath, $"{_m_type.FullName}.asset");
                // Try to load existing asset
                ScriptableObject config = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (config != null && config.GetType() != _m_type)
                {
                    Console.LogWarning(SystemNames.Config, $"Config asset at '{assetPath}' has type '{config.GetType().FullName}' which does not match expected type '{_m_type.FullName}'. Recreating asset.");
                    // Type mismatch - delete the old asset and create new one
                    AssetDatabase.DeleteAsset(assetPath);
                    config = null;
                }

                // Create new
                if (config == null)
                {
                    config = CreateInstance(_m_type);
                    if (config == null)
                    {
                        Console.LogError(SystemNames.Config, $"Failed to create config instance for type '{_m_type.FullName}'.");
                        return null;
                    }

                    AssetDatabase.CreateAsset(config, assetPath);
                    Console.LogSystem(SystemNames.Config, $"Created new config asset at '{assetPath}'.");
                }

                return config;
            }
        }
        private class ConstantsConfigImporterItem : _AConfigImporterItem
        {
            public ConstantsConfigImporterItem([NotNull] Type _type, ConfigImporterSetting.ExcelSheetMapping _excelData)
                : base(_type, _excelData)
            {
            }
            
            
            protected override void ImportDataFromExcel(string _filePath, string _sheetName, ScriptableObject _configAsset)
            {
                // Read from Excel using reflection to call generic method
                try
                {
                    MethodInfo readMethod = typeof(ExcelUtility).GetMethod("ReadConstantsFromExcel", BindingFlags.Public | BindingFlags.Static);
                    MethodInfo genericMethod = readMethod.MakeGenericMethod(type);
                    genericMethod.Invoke(null, new object[] { _filePath, _sheetName, _configAsset, 1 });

                    UnityEditor.EditorUtility.SetDirty(_configAsset);
                    AssetDatabase.SaveAssets();
                    Console.LogSystem(SystemNames.Config, $"Successfully imported constants config '{type.FullName}' from '{_filePath}'.");
                }
                catch (Exception e)
                {
                    Console.LogError(SystemNames.Config, $"Failed to import constants config '{type.FullName}': {e.Message}");
                }
            }
        }
        private class TableConfigImporterItem : _AConfigImporterItem
        {
            public TableConfigImporterItem([NotNull] Type _type, ConfigImporterSetting.ExcelSheetMapping _excelData)
                : base(_type, _excelData)
            {
            }
            

            protected override void ImportDataFromExcel(string _filePath, string _sheetName, ScriptableObject _configAsset)
            {
                // Read from Excel using reflection to call generic method
                try
                {
                    MethodInfo readMethod = typeof(ExcelUtility).GetMethod("ReadTableFromExcel", BindingFlags.Public | BindingFlags.Static);
                    Type dataType = (Type)type.GetMethod("GetDataType").Invoke(_configAsset, null);
                    MethodInfo genericMethod = readMethod.MakeGenericMethod(type, dataType);
                    genericMethod.Invoke(null, new object[] { _filePath, _sheetName, _configAsset, 1 });

                    UnityEditor.EditorUtility.SetDirty(_configAsset);
                    AssetDatabase.SaveAssets();
                    Console.LogSystem(SystemNames.Config, $"Successfully imported table config '{type.FullName}' from '{_filePath}'.");
                }
                catch (Exception e)
                {
                    Console.LogError(SystemNames.Config, $"Failed to import table config '{type.FullName}': {e.Message}");
                }
            }
        }
    }
}