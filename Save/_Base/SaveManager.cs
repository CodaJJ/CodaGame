// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// Manages saving and loading game data to/from disk with support for multiple save slots and optional encryption.
    /// </summary>
    internal class SaveManager
    {
        [NotNull] public static SaveManager instance { get { return _g_instance ??= new SaveManager(); } }
        private static SaveManager _g_instance;
        
        public const string SAVE_FILE_PREFIX = "save_slot_";
        public const string SAVE_FILE_EXTENSION = ".sav";
        public const string TEMP_FILE_EXTENSION = ".tmp";
        public const string BACKUP_FILE_EXTENSION = ".bak";

        
        // Locks for thread safety
        [NotNull] private readonly Dictionary<int, object> _m_slotLocks;
        // Cache save directory to avoid accessing Unity API from background thread
        private readonly string _m_saveDirectory;
        
        // Encryptor for save data
        private _IDataEncryptor _m_encryptor;


        private SaveManager()
        {
            _m_slotLocks = new Dictionary<int, object>();
            _m_saveDirectory = Application.persistentDataPath;
        }


        /// <summary>
        /// The directory where save files are stored.
        /// </summary>
        public string saveDirectory { get { return _m_saveDirectory; } }


        /// <summary>
        /// Sets the encryptor to use for encrypting/decrypting save data.
        /// Set to null to disable encryption.
        /// </summary>
        public void SetEncryptor(_IDataEncryptor _encryptor)
        {
            _m_encryptor = _encryptor;
            Console.LogSystem(SystemNames.Save, "Save data encryptor set: " + (_encryptor != null ? _encryptor.GetType().Name : "None"));
        }
        /// <summary>
        /// Saves data to the specified slot asynchronously (on a background thread).
        /// </summary>
        /// <remarks>
        /// <para>The completion callback is invoked on the main thread.</para>
        /// </remarks>
        public void SaveToSlot<T_DATA>(T_DATA _data, int _slot, Action<SaveResult> _complete)
            where T_DATA : _AGameData
        {
            if (_data == null)
            {
                _complete?.Invoke(SaveResult.Failure(SaveToSlotErrorType.SerializationError));
                return;
            }
            if (!typeof(T_DATA).IsSerializable)
            {
                Console.LogError(SystemNames.Save, $"Failed to save to slot {_slot}: Data type {typeof(T_DATA).Name} is not serializable.");
                _complete?.Invoke(SaveResult.Failure(SaveToSlotErrorType.SerializationError));
                return;
            }

            Console.LogSystem(SystemNames.Save, $"Saving game data to slot {_slot}...");
            _IDataEncryptor encryptor = _m_encryptor;
            object slotLock = _m_slotLocks.GetValueDefinitely(_slot);
            _data.UpdateMetadata();
            string dataJson = JsonUtility.ToJson(_data, true);
            Task.RunThread(() =>
            {
                string tempFilePath = null;
                lock (slotLock)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(dataJson))
                            return SaveResult.Failure(SaveToSlotErrorType.SerializationError);

                        string dataToWrite = dataJson;
                        if (encryptor != null)
                        {
                            dataToWrite = encryptor.Encrypt(dataJson);
                            if (string.IsNullOrEmpty(dataToWrite))
                                return SaveResult.Failure(SaveToSlotErrorType.SerializationError);
                        }

                        string filePath = GetSlotFilePath(_slot, SAVE_FILE_EXTENSION);
                        string backupFilePath = GetSlotFilePath(_slot, BACKUP_FILE_EXTENSION);
                        tempFilePath = GetSlotFilePath(_slot, TEMP_FILE_EXTENSION);

                        string directory = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        File.WriteAllText(tempFilePath, dataToWrite);

                        if (File.Exists(filePath))
                        {
                            if (File.Exists(backupFilePath))
                                File.Delete(backupFilePath);
                            File.Move(filePath, backupFilePath);
                        }

                        File.Move(tempFilePath, filePath);
                        Console.LogSystem(SystemNames.Save, $"Game data saved to slot {_slot} successfully.");
                        return SaveResult.Success();
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.LogError(SystemNames.Save, $"Failed to save to slot {_slot}: Access denied.");
                        return SaveResult.Failure(SaveToSlotErrorType.AccessDenied);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        Console.LogError(SystemNames.Save, $"Failed to save to slot {_slot}: Directory not found ({ex.Message}).");
                        return SaveResult.Failure(SaveToSlotErrorType.PathError);
                    }
                    catch (PathTooLongException)
                    {
                        Console.LogError(SystemNames.Save, $"Failed to save to slot {_slot}: Path too long.");
                        return SaveResult.Failure(SaveToSlotErrorType.PathError);
                    }
                    catch (IOException ex)
                    {
                        Console.LogError(SystemNames.Save, $"Failed to save to slot {_slot}: IO error ({ex.Message}).");
                        return SaveResult.Failure(SaveToSlotErrorType.IOError);
                    }
                    catch (Exception ex)
                    {
                        Console.LogError(SystemNames.Save, $"Failed to save to slot {_slot}: Unknown error ({ex.Message}).");
                        return SaveResult.Failure(SaveToSlotErrorType.Unknown);
                    }
                    finally
                    {
                        FileUtility.DeleteFileIfExists(tempFilePath);
                    }
                }
            }, _complete);
        }
        /// <summary>
        /// Loads data from the specified slot synchronously.
        /// </summary>
        public LoadResult<T_DATA> LoadFromSlot<T_DATA>(int _slot)
            where T_DATA : _AGameData, new()
        {
            if (!typeof(T_DATA).IsSerializable)
            {
                Console.LogError(SystemNames.Save, $"Failed to load from slot {_slot}: Data type {typeof(T_DATA).Name} is not serializable.");
                return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.SerializationError);
            }
            
            object slotLock = _m_slotLocks.GetValueDefinitely(_slot);
            _IDataEncryptor encryptor = _m_encryptor;
            lock (slotLock)
            {
                string filePath = GetSlotFilePath(_slot, SAVE_FILE_EXTENSION);
                LoadResult<T_DATA> result = LoadFromFile<T_DATA>(filePath, encryptor);
                if (result.success)
                {
                    Console.LogSystem(SystemNames.Save, $"Game data loaded from slot {_slot} successfully.");
                    return result;
                }

                Console.LogWarning(SystemNames.Save, $"Failed to load from slot {_slot}, attempting to load from backup...");
                string backupFilePath = GetSlotFilePath(_slot, BACKUP_FILE_EXTENSION);
                LoadResult<T_DATA> backupResult = LoadFromFile<T_DATA>(backupFilePath, encryptor);
                return backupResult;
            }
        }
        /// <summary>
        /// Deletes the save file at the specified slot.
        /// </summary>
        /// <returns>True if the file was deleted, false if it didn't exist</returns>
        public bool DeleteSlot(int _slot)
        {
            string filePath = GetSlotFilePath(_slot, SAVE_FILE_EXTENSION);
            string backupFilePath = GetSlotFilePath(_slot, BACKUP_FILE_EXTENSION);
            bool deleted = false;
            deleted |= FileUtility.DeleteFileIfExists(filePath);
            deleted |= FileUtility.DeleteFileIfExists(backupFilePath);

            if (deleted)
                Console.LogSystem(SystemNames.Save, $"Save slot {_slot} deleted successfully.");
            else
                Console.LogWarning(SystemNames.Save, $"Save slot {_slot} does not exist.");

            return deleted;
        }



        private string GetSlotFilePath(int _slot, string _saveFileExtension)
        {
            string fileName = SAVE_FILE_PREFIX + _slot + _saveFileExtension;
            return Path.Combine(saveDirectory, fileName);
        }
        private LoadResult<T_DATA> LoadFromFile<T_DATA>(string _filePath, _IDataEncryptor _encryptor)
            where T_DATA : _AGameData, new()
        {
            try
            {
                if (!File.Exists(_filePath))
                    return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.FileNotFound);

                string fileContent = File.ReadAllText(_filePath);
                if (string.IsNullOrEmpty(fileContent))
                {
                    Console.LogError(SystemNames.Save, $"Save file is empty: {_filePath}");
                    return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.FileCorrupted);
                }

                string jsonData = fileContent;
                if (_encryptor != null)
                {
                    jsonData = _encryptor.Decrypt(fileContent);
                    if (string.IsNullOrEmpty(jsonData))
                    {
                        Console.LogError(SystemNames.Save, $"Failed to decrypt save file: {_filePath}");
                        return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.FileCorrupted);
                    }
                }

                T_DATA data = JsonUtility.FromJson<T_DATA>(jsonData);
                if (data == null)
                {
                    Console.LogError(SystemNames.Save, $"Failed to deserialize save file: {_filePath}");
                    return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.FileCorrupted);
                }

                if (!data.OnVersionCheck(data.gameVersion))
                {
                    Console.LogError(SystemNames.Save, $"Save file version incompatible: {_filePath} (version: {data.gameVersion})");
                    return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.FileCorrupted);
                }

                return LoadResult<T_DATA>.Success(data);
            }
            catch (ArgumentException ex)
            {
                Console.LogError(SystemNames.Save, $"Save file corrupted (invalid JSON format): {_filePath} ({ex.Message})");
                return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.FileCorrupted);
            }
            catch (UnauthorizedAccessException)
            {
                Console.LogError(SystemNames.Save, $"Access denied when loading: {_filePath}");
                return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.AccessDenied);
            }
            catch (DirectoryNotFoundException)
            {
                Console.LogError(SystemNames.Save, $"Directory not found when loading: {_filePath}");
                return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.PathError);
            }
            catch (PathTooLongException)
            {
                Console.LogError(SystemNames.Save, $"Path too long when loading: {_filePath}");
                return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.PathError);
            }
            catch (FileNotFoundException)
            {
                // This is expected when file doesn't exist, no error log needed
                return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.FileNotFound);
            }
            catch (Exception ex)
            {
                Console.LogError(SystemNames.Save, $"Unknown error when loading: {_filePath} ({ex.Message})");
                return LoadResult<T_DATA>.Failure(LoadFromSlotErrorType.Unknown);
            }
        }
    }
}