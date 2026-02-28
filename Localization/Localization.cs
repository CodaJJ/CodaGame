// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Public API for the localization system.
    /// </summary>
    public static class Localization
    {
        /// <summary>
        /// Current language id. Returns -1 if not set.
        /// </summary>
        public static int currentLanguage { get { return LocalizationManager.instance.currentLanguage; } }
        /// <summary>
        /// Fired when language changes. Parameter is the new language id.
        /// </summary>
        public static event Action<int> onLanguageChanged
        {
            add { LocalizationManager.instance.onLanguageChanged += value; }
            remove { LocalizationManager.instance.onLanguageChanged -= value; }
        }


        /// <summary>
        /// Sets the current language, clears all caches, and fires onLanguageChanged.
        /// </summary>
        public static void SetLanguage(int _language)
        {
            LocalizationManager.instance.SetLanguage(_language);
        }


        #region Text

        /// <summary>
        /// Registers a translation config for a specific language.
        /// </summary>
        public static void RegisterTextConfig(int _language, AssetIndex _assetIndex)
        {
            LocalizationManager.instance.RegisterTextConfig(_language, _assetIndex);
        }
        /// <summary>
        /// Gets the translated and parsed text for the given key.
        /// </summary>
        public static string GetText(string _key, params string[] _args)
        {
            return LocalizationManager.instance.GetText(_key, _args);
        }
        /// <summary>
        /// Registers a custom function for the template parser.
        /// </summary>
        public static void RegisterFunction(string _name, NotNullFunc<string[], string> _func)
        {
            LocalizationManager.instance.RegisterFunction(_name, _func);
        }

        #endregion


        #region Voice

        /// <summary>
        /// Registers a voice config for a specific language.
        /// </summary>
        public static void RegisterVoiceConfig(int _language, AssetIndex _assetIndex)
        {
            LocalizationManager.instance.RegisterVoiceConfig(_language, _assetIndex);
        }
        /// <summary>
        /// Returns the AssetIndex for the given voice key in the current language.
        /// Returns AssetIndex.Invalid if not found — check with isValid before use.
        /// Use AssetLoader to load the audio asset.
        /// </summary>
        public static AssetIndex GetVoiceAssetIndex(string _key)
        {
            return LocalizationManager.instance.GetVoiceAssetIndex(_key);
        }

        #endregion


        #region Font

        /// <summary>
        /// Registers a font config for a specific language.
        /// </summary>
        public static void RegisterFontConfig(int _language, AssetIndex _assetIndex)
        {
            LocalizationManager.instance.RegisterFontConfig(_language, _assetIndex);
        }
        /// <summary>
        /// Gets the font for the given key in the current language.
        /// </summary>
        public static TMP_FontAsset GetFont(string _key)
        {
            return LocalizationManager.instance.GetFont(_key);
        }

        #endregion


        #region Legacy Font

        /// <summary>
        /// Registers a legacy font config for a specific language.
        /// </summary>
        public static void RegisterLegacyFontConfig(int _language, AssetIndex _assetIndex)
        {
            LocalizationManager.instance.RegisterLegacyFontConfig(_language, _assetIndex);
        }
        /// <summary>
        /// Gets the legacy font for the given key in the current language.
        /// </summary>
        public static Font GetLegacyFont(string _key)
        {
            return LocalizationManager.instance.GetLegacyFont(_key);
        }

        #endregion


        #region Sprite

        /// <summary>
        /// Registers a sprite config for a specific language.
        /// </summary>
        public static void RegisterSpriteConfig(int _language, AssetIndex _assetIndex)
        {
            LocalizationManager.instance.RegisterSpriteConfig(_language, _assetIndex);
        }
        /// <summary>
        /// Gets the sprite for the given key in the current language.
        /// </summary>
        public static Sprite GetSprite(string _key)
        {
            return LocalizationManager.instance.GetSprite(_key);
        }

        #endregion


        #region Texture

        /// <summary>
        /// Registers a texture config for a specific language.
        /// </summary>
        public static void RegisterTextureConfig(int _language, AssetIndex _assetIndex)
        {
            LocalizationManager.instance.RegisterTextureConfig(_language, _assetIndex);
        }
        /// <summary>
        /// Gets the texture for the given key in the current language.
        /// </summary>
        public static Texture GetTexture(string _key)
        {
            return LocalizationManager.instance.GetTexture(_key);
        }

        #endregion
    }
}
