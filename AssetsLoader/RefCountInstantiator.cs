// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A reference count based asset loader.
    /// </summary>
    public class RefCountInstantiator : _AReferenceCountAsyncOperation
    {
        // The path of the asset to load.
        private readonly string _m_assetPath;
        // Callbacks for load and release completion.
        private readonly Action<GameObject> _m_loadComplete;
        private readonly Action _m_releaseComplete;
        
        // The loaded asset.
        private GameObject _m_asset;
        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load.</param>
        /// <param name="_loadComplete">Callback for load completion.</param>
        /// <param name="_releaseComplete">Callback for release completion.</param>
        public RefCountInstantiator(_AAssetIndex _assetIndex, Action<GameObject> _loadComplete = null, Action _releaseComplete = null)
            : this(_assetIndex?.ToAddressableKey(), _loadComplete, _releaseComplete)
        {
        }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_assetPath">The path of the asset to load.</param>
        /// <param name="_loadComplete">Callback for load completion.</param>
        /// <param name="_releaseComplete">Callback for release completion.</param>
        public RefCountInstantiator(string _assetPath, Action<GameObject> _loadComplete = null, Action _releaseComplete = null)
        {
            _m_assetPath = _assetPath;
            _m_loadComplete = _loadComplete;
            _m_releaseComplete = _releaseComplete;
        }
        
        
        /// <summary>
        /// Called when the load operation starts.
        /// </summary>
        protected override void OnOperationStart(Action _complete)
        {
            AssetLoader.InstantiateAsync(_m_assetPath, _asset =>
            {
                _m_asset = _asset;
                _m_loadComplete?.Invoke(_asset);
                _complete.Invoke();
            });
        }
        /// <summary>
        /// Called when the load operation ends, releasing the asset.
        /// </summary>
        protected override void OnOperationEnd(Action _complete)
        {
            AssetLoader.Release(_m_asset);
            _m_releaseComplete?.Invoke();
            _complete.Invoke();
        }
    }
}