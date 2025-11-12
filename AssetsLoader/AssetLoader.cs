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

            return LoadSync<T_ASSET>(_assetIndex.ToAddressableKey());
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

            LoadAsync(_assetIndex.ToAddressableKey(), _complete);
        }
        /// <summary>
        /// Load Asset Asynchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        public static AsyncOperationHandle<T_ASSET> LoadAsync<T_ASSET>(_AAssetIndex _assetIndex)
        {
            if (_assetIndex == null)
                return default;

            return LoadAsync<T_ASSET>(_assetIndex.ToAddressableKey());
        }
        /// <summary>
        /// Instantiate GameObject Synchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        public static GameObject InstantiateSync(_AAssetIndex _assetIndex)
        {
            if (_assetIndex == null)
                return null;

            return InstantiateSync(_assetIndex.ToAddressableKey());
        }
        /// <summary>
        /// Instantiate GameObject Asynchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        /// <param name="_complete">Callback when load is complete</param>
        public static void InstantiateAsync(_AAssetIndex _assetIndex, Action<GameObject> _complete)
        {
            if (_complete == null)
                return;
            
            if (_assetIndex == null)
            {
                _complete.Invoke(null);
                return;
            }

            InstantiateAsync(_assetIndex.ToAddressableKey(), _complete);
        }
        /// <summary>
        /// Instantiate GameObject Asynchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        public static AsyncOperationHandle<GameObject> InstantiateAsync(_AAssetIndex _assetIndex)
        {
            if (_assetIndex == null)
                return default;

            return InstantiateAsync(_assetIndex.ToAddressableKey());
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
        /// Instantiate GameObject Synchronously
        /// </summary>
        /// <param name="_assetPath">The path of the asset to load</param>
        public static GameObject InstantiateSync(string _assetPath)
        {
            if (string.IsNullOrEmpty(_assetPath))
                return null;

            try
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(_assetPath);
                handle.WaitForCompletion();
                return handle.Result;
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables instantiate failed.");
                return null;
            }
        }
        /// <summary>
        /// Instantiate GameObject Asynchronously
        /// </summary>
        /// <param name="_assetPath">The path of the asset to load</param>
        /// <param name="_complete">Callback when load is complete</param>
        public static void InstantiateAsync(string _assetPath, Action<GameObject> _complete)
        {
            if (_complete == null)
                return;
            
            if (string.IsNullOrEmpty(_assetPath))
            {
                _complete.Invoke(null);
                return;
            }

            try
            {
                Addressables.InstantiateAsync(_assetPath).Completed += _handle =>
                {
                    _complete.Invoke(_handle.Result);
                };
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables instantiate failed.");
                _complete.Invoke(null);
            }
        }
        /// <summary>
        /// Instantiate GameObject Asynchronously
        /// </summary>
        /// <param name="_assetPath">The path of the asset to load</param>
        public static AsyncOperationHandle<GameObject> InstantiateAsync(string _assetPath)
        {
            if (string.IsNullOrEmpty(_assetPath))
                return default;
            
            try
            {
                return Addressables.InstantiateAsync(_assetPath);
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables instantiate failed.");
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