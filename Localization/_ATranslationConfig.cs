// Copyright (c) 2026 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame
{
    [Serializable]
    public class TranslationEntry
    {
        public string key;
        public string value;
    }
    public abstract class _ATranslationConfig : _ATableConfig<TranslationEntry>
    {
        [NotNull] private readonly Dictionary<string, string> _m_translationDictionary;
        private bool _m_dictionaryBuilt;


        public _ATranslationConfig()
        {
            _m_translationDictionary = new Dictionary<string, string>();
        }


        public string GetTranslation(string _key)
        {
            if (string.IsNullOrEmpty(_key))
            {
                Console.LogWarning(SystemNames.Localization, "Translation key cannot be null or empty");
                return null;
            }

            if (!_m_dictionaryBuilt)
            {
                foreach (TranslationEntry entry in notNullDataList)
                {
                    if (!string.IsNullOrEmpty(entry.key))
                        _m_translationDictionary[entry.key] = entry.value;
                }
                _m_dictionaryBuilt = true;
            }

            if (_m_translationDictionary.TryGetValue(_key, out string value))
                return value;

            // Cache miss to suppress repeated warnings on subsequent lookups.
            Console.LogWarning(SystemNames.Localization, $"Translation key '{_key}' not found");
            _m_translationDictionary[_key] = null;
            return null;
        }
    }
}