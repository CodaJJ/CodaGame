// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// A reference count based asset loader.
    /// </summary>
    public class ReferenceCountLoader<T_ASSET> : _AReferenceCountAsyncOperation
    {
        // The index of the asset to load.
        private readonly _AAssetIndex _m_assetIndex;
        // Callbacks for load and release completion.
        private readonly Action<T_ASSET> _m_loadComplete;
        private readonly Action _m_releaseComplete;
        
        // The loaded asset.
        private T_ASSET _m_asset;
        
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="_assetIndex">The index of the asset to load.</param>
        /// <param name="_loadComplete">Callback for load completion.</param>
        /// <param name="_releaseComplete">Callback for release completion.</param>
        public ReferenceCountLoader(_AAssetIndex _assetIndex, Action<T_ASSET> _loadComplete = null, Action _releaseComplete = null)
        {
            _m_assetIndex = _assetIndex;
            _m_loadComplete = _loadComplete;
            _m_releaseComplete = _releaseComplete;
        }
        
        
        /// <summary>
        /// Called when the load operation starts.
        /// </summary>
        protected override void OnOperationStart(Action _complete)
        {
            if (_m_assetIndex == null)
            {
                Console.LogWarning(SystemNames.Operation, "Loader is null.");
                _complete.Invoke();
                return;
            }
            
            AssetLoader.LoadAsync<T_ASSET>(_m_assetIndex, _asset =>
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
            if (_m_assetIndex == null)
            {
                Console.LogWarning(SystemNames.Operation, "Loader is null.");
                _complete.Invoke();
                return;
            }

            AssetLoader.Release(_m_asset);
            _m_releaseComplete?.Invoke();
            _complete.Invoke();
        }
    }
}