// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame.Base
{
    /// <summary>
    /// A internal manager for loading module.
    /// </summary>
    internal class LoadingManager
    {
        [NotNull] public static LoadingManager instance { get { return _g_instance ??= new LoadingManager(); } }
        private static LoadingManager _g_instance;


        [NotNull] private readonly Dictionary<_ILoadingShow, LoadingProcess> _m_loadingShowDict;
        private LoadingProcess _m_emptyShowLoadingProcess;


        private LoadingManager()
        {
            _m_loadingShowDict = new Dictionary<_ILoadingShow, LoadingProcess>();
        }


        /// <summary>
        /// Loads the specified loading show with the provided load function and end callback.
        /// </summary>
        /// <param name="_loadingShow">The loading show interface.</param>
        /// <param name="_loadFunction">The asynchronous load function.</param>
        /// <param name="_loadingShowEndCallback">The callback to be invoked when the loading show ends.</param>
        public void Load(_ILoadingShow _loadingShow, AsyncFunction _loadFunction, Action _loadingShowEndCallback)
        {
            // Determine the appropriate loading process based on the loading show
            LoadingProcess loadingProcess;
            if (_loadingShow == null)
            {
                _m_emptyShowLoadingProcess ??= new LoadingProcess(null);
                loadingProcess = _m_emptyShowLoadingProcess;
            }
            else if (!_m_loadingShowDict.TryGetValue(_loadingShow, out loadingProcess) || loadingProcess == null)
            {
                loadingProcess = new LoadingProcess(_loadingShow);
                _m_loadingShowDict[_loadingShow] = loadingProcess;
            }
            
            // Add the load function and end callback to the loading process
            loadingProcess.AddLoadFunction(_loadFunction, _loadingShowEndCallback);
        }
    }
}