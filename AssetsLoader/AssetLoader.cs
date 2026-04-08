// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

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
        public static T_ASSET LoadSync<T_ASSET>(AssetIndex _assetIndex)
        {
            return LoadSync<T_ASSET>(_assetIndex.ToAddressableKey());
        }
        /// <summary>
        /// Load Asset Asynchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        /// <param name="_complete">Callback when load is complete</param>
        public static void LoadAsync<T_ASSET>(AssetIndex _assetIndex, Action<T_ASSET> _complete)
        {
            LoadAsync(_assetIndex.ToAddressableKey(), _complete);
        }
        /// <summary>
        /// Load Asset Asynchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        public static AsyncOperationHandle<T_ASSET> LoadAsync<T_ASSET>(AssetIndex _assetIndex)
        {
            return LoadAsync<T_ASSET>(_assetIndex.ToAddressableKey());
        }
        /// <summary>
        /// Instantiate GameObject Synchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        public static GameObject InstantiateSync(AssetIndex _assetIndex)
        {
            return InstantiateSync(_assetIndex.ToAddressableKey());
        }
        /// <summary>
        /// Instantiate GameObject Asynchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        /// <param name="_complete">Callback when load is complete</param>
        public static void InstantiateAsync(AssetIndex _assetIndex, Action<GameObject> _complete)
        {
            InstantiateAsync(_assetIndex.ToAddressableKey(), _complete);
        }
        /// <summary>
        /// Instantiate GameObject Asynchronously
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load</param>
        public static AsyncOperationHandle<GameObject> InstantiateAsync(AssetIndex _assetIndex)
        {
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
                    if (_handle.Status != AsyncOperationStatus.Succeeded)
                    {
                        Console.LogError(SystemNames.Assets, "Unity Addressables load failed.");
                        Addressables.Release(_handle);
                        _complete.Invoke(default);
                        return;
                    }
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
                    if (_handle.Status != AsyncOperationStatus.Succeeded)
                    {
                        Console.LogError(SystemNames.Assets, "Unity Addressables instantiate failed.");
                        Addressables.Release(_handle);
                        _complete.Invoke(null);
                        return;
                    }
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
        /// Release an instantiated GameObject created by <see cref="InstantiateAsync(string, Action{GameObject})"/>.
        /// </summary>
        /// <remarks>
        /// <para>This destroys the GameObject and releases the Addressable reference.</para>
        /// <para>Returns false if the GameObject was not created via Addressables.</para>
        /// </remarks>
        public static bool ReleaseInstance(GameObject _instance)
        {
            if (_instance == null)
                return false;

            return Addressables.ReleaseInstance(_instance);
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
        /// <summary>
        /// Load a Unity Scene asynchronously in Additive mode.
        /// </summary>
        /// <param name="_assetIndex">The index of the scene to load</param>
        /// <param name="_complete">Callback when load is complete. Receives <c>default(SceneInstance)</c> on failure.</param>
        public static void LoadSceneAsync(AssetIndex _assetIndex, Action<SceneInstance> _complete)
        {
            LoadSceneAsync(_assetIndex.ToAddressableKey(), _complete);
        }
        /// <summary>
        /// Load a Unity Scene asynchronously in Additive mode.
        /// </summary>
        /// <param name="_assetIndex">The index of the scene to load</param>
        public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(AssetIndex _assetIndex)
        {
            return LoadSceneAsync(_assetIndex.ToAddressableKey());
        }
        /// <summary>
        /// Load a Unity Scene asynchronously in Additive mode.
        /// </summary>
        /// <param name="_assetPath">The path of the scene to load</param>
        /// <param name="_complete">Callback when load is complete. Receives <c>default(SceneInstance)</c> on failure.</param>
        public static void LoadSceneAsync(string _assetPath, Action<SceneInstance> _complete)
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
                Addressables.LoadSceneAsync(_assetPath, LoadSceneMode.Additive).Completed += _handle =>
                {
                    if (_handle.Status != AsyncOperationStatus.Succeeded)
                    {
                        Console.LogError(SystemNames.Assets, "Unity Addressables scene load failed.");
                        Addressables.Release(_handle);
                        _complete.Invoke(default);
                        return;
                    }
                    _complete.Invoke(_handle.Result);
                };
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables scene load failed.");
                _complete.Invoke(default);
            }
        }
        /// <summary>
        /// Load a Unity Scene asynchronously in Additive mode.
        /// </summary>
        /// <param name="_assetPath">The path of the scene to load</param>
        public static AsyncOperationHandle<SceneInstance> LoadSceneAsync(string _assetPath)
        {
            if (string.IsNullOrEmpty(_assetPath))
                return default;

            try
            {
                return Addressables.LoadSceneAsync(_assetPath, LoadSceneMode.Additive);
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables scene load failed.");
                return default;
            }
        }
        /// <summary>
        /// Unload a Unity Scene previously loaded via <see cref="LoadSceneAsync(string, Action{SceneInstance})"/>.
        /// </summary>
        /// <param name="_sceneInstance">The scene instance to unload</param>
        /// <param name="_complete">Callback when unload is complete</param>
        public static void UnloadSceneAsync(SceneInstance _sceneInstance, Action _complete)
        {
            if (!_sceneInstance.Scene.IsValid())
            {
                Console.LogWarning(SystemNames.Assets, "UnloadSceneAsync called with invalid SceneInstance.");
                _complete?.Invoke();
                return;
            }

            try
            {
                Addressables.UnloadSceneAsync(_sceneInstance).Completed += _handle =>
                {
                    if (_handle.Status != AsyncOperationStatus.Succeeded)
                        Console.LogError(SystemNames.Assets, "Unity Addressables scene unload failed.");
                    _complete?.Invoke();
                };
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables scene unload failed.");
                _complete?.Invoke();
            }
        }
        /// <summary>
        /// Unload a Unity Scene previously loaded via Addressables.
        /// </summary>
        public static AsyncOperationHandle<SceneInstance> UnloadSceneAsync(SceneInstance _sceneInstance)
        {
            if (!_sceneInstance.Scene.IsValid())
            {
                Console.LogWarning(SystemNames.Assets, "UnloadSceneAsync called with invalid SceneInstance.");
                return default;
            }

            try
            {
                return Addressables.UnloadSceneAsync(_sceneInstance);
            }
            catch
            {
                Console.LogError(SystemNames.Assets, "Unity Addressables scene unload failed.");
                return default;
            }
        }
    }
}
