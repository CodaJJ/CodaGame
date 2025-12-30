// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Public API for the save system.
    /// Provides methods to save, load, and manage save files.
    /// </summary>
    public static class Save
    {
        [NotNull] private static SaveManager manager { get { return SaveManager.instance; } }
        
        /// <summary>
        /// Gets the directory where save files are stored.
        /// </summary>
        public static string saveDirectory { get { return manager.saveDirectory; } }


        /// <summary>
        /// Sets the encryptor to use for encrypting/decrypting save data.
        /// Set to null to disable encryption.
        /// </summary>
        /// <param name="_encryptor">The encryptor implementation, or null to disable encryption</param>
        public static void SetEncryptor(_IDataEncryptor _encryptor)
        {
            manager.SetEncryptor(_encryptor);
        }
        /// <summary>
        /// Saves game data to the specified slot asynchronously.
        /// The save operation runs on a background thread.
        /// </summary>
        /// <typeparam name="T_DATA">Type of game data, must inherit from _AGameData, and be serializable</typeparam>
        /// <param name="_data">The game data to save</param>
        /// <param name="_slot">The slot number to save to</param>
        /// <param name="_onComplete">Callback invoked when save completes (on main thread)</param>
        public static void SaveToSlot<T_DATA>(T_DATA _data, int _slot, Action<SaveResult> _onComplete = null)
            where T_DATA : _AGameData
        {
            manager.SaveToSlot(_data, _slot, _onComplete);
        }
        /// <summary>
        /// Loads game data from the specified slot synchronously.
        /// </summary>
        /// <typeparam name="T_DATA">Type of game data, must inherit from _AGameData, and be serializable</typeparam>
        /// <param name="_slot">The slot number to load from</param>
        /// <returns>LoadResult containing the loaded data and status</returns>
        public static LoadResult<T_DATA> LoadFromSlot<T_DATA>(int _slot)
            where T_DATA : _AGameData, new()
        {
            return manager.LoadFromSlot<T_DATA>(_slot);
        }
        /// <summary>
        /// Deletes the save file at the specified slot.
        /// </summary>
        /// <param name="_slot">The slot number to delete</param>
        /// <returns>True if the file was deleted, false if it didn't exist</returns>
        public static bool DeleteSlot(int _slot)
        {
            return manager.DeleteSlot(_slot);
        }
    }
}