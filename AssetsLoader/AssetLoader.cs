// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CodaGame
{
    /// <summary>
    /// Asset Loader using Unity Addressables
    /// </summary>
    public static class AssetLoader
    {
        /// <summary>
        /// Load Asset Synchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        public static T_ASSET LoadSync<T_ASSET>(_AAssetIndex _assetIndex)
        {
            if (_assetIndex == null)
                return default;

            try
            {
                AsyncOperationHandle<T_ASSET> handle = Addressables.LoadAssetAsync<T_ASSET>(_assetIndex.ToAddressableKey());
                handle.WaitForCompletion();
                return handle.Result;
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables load failed.");
                return default;
            }
        }
        /// <summary>
        /// Load Asset Asynchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        /// <param name="_complete">Callback when load is complete</param>
        public static void LoadAsync<T_ASSET>(_AAssetIndex _assetIndex, Action<T_ASSET> _complete)
        {
            if (_complete == null)
                return;
            
            if (_assetIndex == null)
            {
                _complete.Invoke(default);
                return;
            }

            try
            {
                Addressables.LoadAssetAsync<T_ASSET>(_assetIndex.ToAddressableKey()).Completed += _handle =>
                {
                    _complete.Invoke(_handle.Result);
                };
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables load failed.");
                _complete.Invoke(default);
            }
        }
        /// <summary>
        /// Load Asset Asynchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        public static AsyncOperationHandle<T_ASSET> LoadAsync<T_ASSET>(_AAssetIndex _assetIndex)
        {
            if (_assetIndex == null)
                return default;

            try
            {
                return Addressables.LoadAssetAsync<T_ASSET>(_assetIndex.ToAddressableKey());
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables load failed.");
                return default;
            }
        }
        /// <summary>
        /// Load Asset Synchronously
        /// </summary>
        /// <param name="_assetPath">The path of the asset to load</param>
        public static T_ASSET LoadSync<T_ASSET>(string _assetPath)
        {
            if (string.IsNullOrEmpty(_assetPath))
                return default;

            try
            {
                AsyncOperationHandle<T_ASSET> handle = Addressables.LoadAssetAsync<T_ASSET>(_assetPath);
                handle.WaitForCompletion();
                return handle.Result;
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables load failed.");
                return default;
            }
        }
        /// <summary>
        /// Load Asset Asynchronously
        /// </summary>
        /// <param name="_assetPath">The path of the asset to load</param>
        /// <param name="_complete">Callback when load is complete</param>
        public static void LoadAsync<T_ASSET>(string _assetPath, Action<T_ASSET> _complete)
        {
            if (_complete == null)
                return;
            
            if (string.IsNullOrEmpty(_assetPath))
            {
                _complete.Invoke(default);
                return;
            }

            try
            {
                Addressables.LoadAssetAsync<T_ASSET>(_assetPath).Completed += _handle =>
                {
                    _complete.Invoke(_handle.Result);
                };
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables load failed.");
                _complete.Invoke(default);
            }
        }
        /// <summary>
        /// Load Asset Asynchronously
        /// </summary>
        /// <param name="_assetPath">The path of the asset to load</param>
        public static AsyncOperationHandle<T_ASSET> LoadAsync<T_ASSET>(string _assetPath)
        {
            if (string.IsNullOrEmpty(_assetPath))
                return default;

            try
            {
                return Addressables.LoadAssetAsync<T_ASSET>(_assetPath);
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables load failed.");
                return default;
            }
        }
        /// <summary>
        /// Release the loaded asset
        /// </summary>
        public static void Release<T_ASSET>(AsyncOperationHandle<T_ASSET> _handle)
        {
            Addressables.Release(_handle);
        }
        /// <summary>
        /// Release the loaded asset
        /// </summary>
        public static void Release<T_ASSET>(T_ASSET _asset)
        {
            Addressables.Release(_asset);
        }
    }
}