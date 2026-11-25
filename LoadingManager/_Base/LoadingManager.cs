// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

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
        /// Return the shared loading process for this show.
        /// </summary>
        public LoadingProcess GetOrCreateProcess(_ILoadingShow _loadingShow)
        {
            if (_loadingShow == null)
                return _m_emptyShowLoadingProcess ??= new LoadingProcess(null);

            if (!_m_loadingShowDict.TryGetValue(_loadingShow, out LoadingProcess loadingProcess))
            {
                loadingProcess = new LoadingProcess(_loadingShow);
                _m_loadingShowDict[_loadingShow] = loadingProcess;
            }
            return loadingProcess;
        }
    }
}
