// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Base class for game data to be saved/loaded.
    /// </summary>
    [Serializable]
    public abstract class _AGameData
    {
        // Metadata
        public string saveTime;
        public string gameVersion;


        /// <summary>
        /// Called after loading from file to handle version compatibility.
        /// Override this method to migrate data from older versions.
        /// </summary>
        /// <param name="_loadedVersion">The version string loaded from the save file</param>
        /// <returns>True if migration succeeded, false if the save is incompatible</returns>
        public abstract bool OnVersionCheck(string _loadedVersion);
        

        /// <summary>
        /// Updates the metadata (save time and game version).
        /// Called automatically before saving.
        /// </summary>
        internal void UpdateMetadata()
        {
            saveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            gameVersion = Application.version;
        }
    }
}