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
    public class LocalizedAssetEntry
    {
        public string key;
        public AssetIndex assetIndex;
    }
    /// <summary>
    /// A table config that maps string keys to AssetIndex values.
    /// Used for localized fonts, sprites, textures, voice clips, etc.
    /// </summary>
    public abstract class _ALocalizedAssetConfig : _ATableConfig<LocalizedAssetEntry>
    {
        [NotNull] private readonly Dictionary<string, AssetIndex> _m_indexDictionary;


        public _ALocalizedAssetConfig()
        {
            _m_indexDictionary = new Dictionary<string, AssetIndex>();
        }


        /// <summary>
        /// Returns the AssetIndex for the given key.
        /// Returns AssetIndex.Invalid if not found — check with isValid before use.
        /// </summary>
        public AssetIndex GetAssetIndex(string _key)
        {
            if (string.IsNullOrEmpty(_key))
            {
                Console.LogWarning(SystemNames.Localization, "Key cannot be null or empty");
                return AssetIndex.Invalid;
            }

            if (_m_indexDictionary.TryGetValue(_key, out AssetIndex index))
                return index;

            foreach (LocalizedAssetEntry entry in notNullDataList)
            {
                if (entry.key == _key)
                {
                    _m_indexDictionary[_key] = entry.assetIndex;
                    return entry.assetIndex;
                }
            }

            Console.LogWarning(SystemNames.Localization, $"Key '{_key}' not found");
            return AssetIndex.Invalid;
        }
    }
}
