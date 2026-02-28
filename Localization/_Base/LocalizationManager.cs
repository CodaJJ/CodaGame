// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// Manager for localization, including text, voice, fonts, sprites, and textures.
    /// </summary>
    internal class LocalizationManager
    {
        [NotNull] public static LocalizationManager instance { get { return _g_instance ??= new LocalizationManager(); } }
        private static LocalizationManager _g_instance;


        private int _m_currentLanguage;

        [NotNull] private readonly StringTemplateParser _m_templateParser;

        // Config addresses: language -> config AssetIndex
        [NotNull] private readonly Dictionary<int, AssetIndex> _m_textConfigAddresses;
        [NotNull] private readonly Dictionary<int, AssetIndex> _m_voiceConfigAddresses;
        [NotNull] private readonly Dictionary<int, AssetIndex> _m_fontConfigAddresses;
        [NotNull] private readonly Dictionary<int, AssetIndex> _m_legacyFontConfigAddresses;
        [NotNull] private readonly Dictionary<int, AssetIndex> _m_spriteConfigAddresses;
        [NotNull] private readonly Dictionary<int, AssetIndex> _m_textureConfigAddresses;

        // Config caches: loaded config for current language
        private _ATranslationConfig _m_textConfigCache;
        private _ALocalizedAssetConfig _m_voiceConfigCache;
        private _ALocalizedAssetConfig _m_fontConfigCache;
        private _ALocalizedAssetConfig _m_legacyFontConfigCache;
        private _ALocalizedAssetConfig _m_spriteConfigCache;
        private _ALocalizedAssetConfig _m_textureConfigCache;

        // Asset caches: key -> loaded asset
        [NotNull] private readonly Dictionary<string, TMP_FontAsset> _m_fontCache;
        [NotNull] private readonly Dictionary<string, Font> _m_legacyFontCache;
        [NotNull] private readonly Dictionary<string, Sprite> _m_spriteCache;
        [NotNull] private readonly Dictionary<string, Texture> _m_textureCache;


        private LocalizationManager()
        {
            _m_templateParser = new StringTemplateParser();

            _m_textConfigAddresses = new Dictionary<int, AssetIndex>();
            _m_voiceConfigAddresses = new Dictionary<int, AssetIndex>();
            _m_fontConfigAddresses = new Dictionary<int, AssetIndex>();
            _m_legacyFontConfigAddresses = new Dictionary<int, AssetIndex>();
            _m_spriteConfigAddresses = new Dictionary<int, AssetIndex>();
            _m_textureConfigAddresses = new Dictionary<int, AssetIndex>();

            _m_fontCache = new Dictionary<string, TMP_FontAsset>();
            _m_legacyFontCache = new Dictionary<string, Font>();
            _m_spriteCache = new Dictionary<string, Sprite>();
            _m_textureCache = new Dictionary<string, Texture>();

            _m_currentLanguage = -1;
        }


        /// <summary>
        /// Current language id.
        /// </summary>
        public int currentLanguage { get { return _m_currentLanguage; } }
        /// <summary>
        /// Fired when language changes. Parameter is the new language id.
        /// </summary>
        public event Action<int> onLanguageChanged;


        /// <summary>
        /// Sets the current language, clears cache, and fires the onLanguageChanged event.
        /// </summary>
        public void SetLanguage(int _language)
        {
            if (_language < 0)
            {
                Console.LogWarning(SystemNames.Localization, "Language id cannot be negative");
                return;
            }

            if (_m_currentLanguage == _language)
                return;

            ClearAllCaches();
            _m_currentLanguage = _language;
            onLanguageChanged?.Invoke(_language);
        }


        #region Text

        /// <summary>
        /// Registers a translation config for a specific language.
        /// </summary>
        public void RegisterTextConfig(int _language, AssetIndex _assetIndex)
        {
            RegisterConfig(_m_textConfigAddresses, _language, _assetIndex);
        }
        /// <summary>
        /// Gets the translated and parsed text for the given key.
        /// </summary>
        public string GetText(string _key, params string[] _args)
        {
            if (string.IsNullOrEmpty(_key))
                return string.Empty;

            if (_m_currentLanguage < 0)
            {
                Console.LogWarning(SystemNames.Localization, "Language not set");
                return _key;
            }

            if (_m_textConfigCache == null)
            {
                if (!_m_textConfigAddresses.TryGetValue(_m_currentLanguage, out AssetIndex assetIndex))
                {
                    Console.LogWarning(SystemNames.Localization, $"No text config for language '{_m_currentLanguage}'");
                    return _key;
                }

                _m_textConfigCache = AssetLoader.LoadSync<_ATranslationConfig>(assetIndex);
                if (_m_textConfigCache == null)
                {
                    Console.LogWarning(SystemNames.Localization, "Failed to load text config");
                    return _key;
                }
            }

            string template = _m_textConfigCache.GetTranslation(_key);
            if (template == null)
                return _key;

            return _m_templateParser.Parse(template, _args);
        }
        /// <summary>
        /// Registers a custom function for the template parser.
        /// </summary>
        public void RegisterFunction(string _name, NotNullFunc<string[], string> _func)
        {
            _m_templateParser.RegisterFunction(_name, _func);
        }

        #endregion


        #region Voice

        /// <summary>
        /// Registers a voice config for a specific language.
        /// </summary>
        public void RegisterVoiceConfig(int _language, AssetIndex _assetIndex)
        {
            RegisterConfig(_m_voiceConfigAddresses, _language, _assetIndex);
        }
        /// <summary>
        /// Returns the AssetIndex for the given voice key in the current language.
        /// Returns AssetIndex.Invalid if not found — check with isValid before use.
        /// The caller is responsible for loading the audio asset.
        /// </summary>
        public AssetIndex GetVoiceAssetIndex(string _key)
        {
            if (string.IsNullOrEmpty(_key))
                return AssetIndex.Invalid;

            if (_m_currentLanguage < 0)
            {
                Console.LogWarning(SystemNames.Localization, "Language not set");
                return AssetIndex.Invalid;
            }

            if (_m_voiceConfigCache == null)
            {
                if (!_m_voiceConfigAddresses.TryGetValue(_m_currentLanguage, out AssetIndex configIndex))
                {
                    Console.LogWarning(SystemNames.Localization, $"No voice config for language '{_m_currentLanguage}'");
                    return AssetIndex.Invalid;
                }

                _m_voiceConfigCache = AssetLoader.LoadSync<_ALocalizedAssetConfig>(configIndex);
                if (_m_voiceConfigCache == null)
                {
                    Console.LogWarning(SystemNames.Localization, "Failed to load voice config");
                    return AssetIndex.Invalid;
                }
            }

            return _m_voiceConfigCache.GetAssetIndex(_key);
        }

        #endregion


        #region Font

        /// <summary>
        /// Registers a font config for a specific language.
        /// </summary>
        public void RegisterFontConfig(int _language, AssetIndex _assetIndex)
        {
            RegisterConfig(_m_fontConfigAddresses, _language, _assetIndex);
        }
        /// <summary>
        /// Gets the font for the given key in the current language.
        /// </summary>
        public TMP_FontAsset GetFont(string _key)
        {
            return GetLocAsset(_m_fontConfigAddresses, ref _m_fontConfigCache, _m_fontCache, _key, "font");
        }

        #endregion


        #region Legacy Font

        /// <summary>
        /// Registers a legacy font config for a specific language.
        /// </summary>
        public void RegisterLegacyFontConfig(int _language, AssetIndex _assetIndex)
        {
            RegisterConfig(_m_legacyFontConfigAddresses, _language, _assetIndex);
        }
        /// <summary>
        /// Gets the legacy font for the given key in the current language.
        /// </summary>
        public Font GetLegacyFont(string _key)
        {
            return GetLocAsset(_m_legacyFontConfigAddresses, ref _m_legacyFontConfigCache, _m_legacyFontCache, _key, "legacy font");
        }

        #endregion


        #region Sprite

        /// <summary>
        /// Registers a sprite config for a specific language.
        /// </summary>
        public void RegisterSpriteConfig(int _language, AssetIndex _assetIndex)
        {
            RegisterConfig(_m_spriteConfigAddresses, _language, _assetIndex);
        }
        /// <summary>
        /// Gets the sprite for the given key in the current language.
        /// </summary>
        public Sprite GetSprite(string _key)
        {
            return GetLocAsset(_m_spriteConfigAddresses, ref _m_spriteConfigCache, _m_spriteCache, _key, "sprite");
        }

        #endregion


        #region Texture

        /// <summary>
        /// Registers a texture config for a specific language.
        /// </summary>
        public void RegisterTextureConfig(int _language, AssetIndex _assetIndex)
        {
            RegisterConfig(_m_textureConfigAddresses, _language, _assetIndex);
        }
        /// <summary>
        /// Gets the texture for the given key in the current language.
        /// </summary>
        public Texture GetTexture(string _key)
        {
            return GetLocAsset(_m_textureConfigAddresses, ref _m_textureConfigCache, _m_textureCache, _key, "texture");
        }

        #endregion


        #region Helpers

        private void RegisterConfig(
            [NotNull] Dictionary<int, AssetIndex> _configAddresses,
            int _language,
            AssetIndex _assetIndex)
        {
            if (_language < 0)
            {
                Console.LogWarning(SystemNames.Localization, "Language id cannot be negative");
                return;
            }
            _configAddresses[_language] = _assetIndex;
        }
        private T GetLocAsset<T>(
            [NotNull] Dictionary<int, AssetIndex> _configAddresses,
            ref _ALocalizedAssetConfig _configCache,
            [NotNull] Dictionary<string, T> _assetCache,
            string _key,
            string _configTypeName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(_key))
                return null;

            if (_m_currentLanguage < 0)
            {
                Console.LogWarning(SystemNames.Localization, "Language not set");
                return null;
            }

            if (_assetCache.TryGetValue(_key, out T cached))
                return cached;

            if (_configCache == null)
            {
                if (!_configAddresses.TryGetValue(_m_currentLanguage, out AssetIndex configIndex))
                {
                    Console.LogWarning(SystemNames.Localization, $"No {_configTypeName} config for language '{_m_currentLanguage}'");
                    return null;
                }

                _configCache = AssetLoader.LoadSync<_ALocalizedAssetConfig>(configIndex);
                if (_configCache == null)
                {
                    Console.LogWarning(SystemNames.Localization, $"Failed to load {_configTypeName} config");
                    return null;
                }
            }

            AssetIndex assetIndex = _configCache.GetAssetIndex(_key);
            if (!assetIndex.isValid)
                return null;

            T asset = AssetLoader.LoadSync<T>(assetIndex);
            if (asset != null)
                _assetCache[_key] = asset;

            return asset;
        }
        private void ClearAllCaches()
        {
            if (_m_textConfigCache != null) { AssetLoader.Release(_m_textConfigCache); _m_textConfigCache = null; }
            if (_m_voiceConfigCache != null) { AssetLoader.Release(_m_voiceConfigCache); _m_voiceConfigCache = null; }
            if (_m_fontConfigCache != null) { AssetLoader.Release(_m_fontConfigCache); _m_fontConfigCache = null; }
            if (_m_legacyFontConfigCache != null) { AssetLoader.Release(_m_legacyFontConfigCache); _m_legacyFontConfigCache = null; }
            if (_m_spriteConfigCache != null) { AssetLoader.Release(_m_spriteConfigCache); _m_spriteConfigCache = null; }
            if (_m_textureConfigCache != null) { AssetLoader.Release(_m_textureConfigCache); _m_textureConfigCache = null; }

            foreach (TMP_FontAsset font in _m_fontCache.Values)
                AssetLoader.Release(font);
            _m_fontCache.Clear();

            foreach (Font font in _m_legacyFontCache.Values)
                AssetLoader.Release(font);
            _m_legacyFontCache.Clear();

            foreach (Sprite sprite in _m_spriteCache.Values)
                AssetLoader.Release(sprite);
            _m_spriteCache.Clear();

            foreach (Texture texture in _m_textureCache.Values)
                AssetLoader.Release(texture);
            _m_textureCache.Clear();
        }

        #endregion
    }
}
