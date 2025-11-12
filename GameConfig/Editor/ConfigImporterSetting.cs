// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEditor;

namespace CodaGame.Editor
{
    internal class ConfigImporterSetting : _AGameSetting
    {
        public AssetPath outputFolderPath = "Assets/Configs";
        public List<ExcelSheetMapping> excelSheetMappings;
        
        
        public void CleanupExcelSheetMappings(List<string> _configTypeNames)
        {
            if (_configTypeNames == null || excelSheetMappings == null)
                return;
            
            excelSheetMappings.RemoveDuplicates(_mapping => _mapping.typeName);
            excelSheetMappings.RemoveAll(_mapping => string.IsNullOrEmpty(_mapping.typeName) || !_configTypeNames.Contains(_mapping.typeName));
            UnityEditor.EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
        
        
        [Serializable]
        internal class ExcelSheetMapping
        {
            public string typeName;
            public AssetPath filePath;
            public string sheetName;
        }
    }
}