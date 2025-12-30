// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.IO;
using JetBrains.Annotations;

namespace CodaGame
{
    public static class FileUtility
    {
        [NotNull]
        public static string CreateTempCopy([NotNull] string _originalFilePath)
        {
            try
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{Path.GetExtension(_originalFilePath)}");
                File.Copy(_originalFilePath, tempFilePath, true);
                return tempFilePath;
            }
            catch (Exception e)
            {
                Console.LogCrush(SystemNames.System, $"Failed to create temp copy of '{_originalFilePath}': {e.Message}");
                throw;
            }
        }
        public static void DeleteTempCopy([NotNull] string _tempFilePath)
        {
            try
            {
                if (File.Exists(_tempFilePath))
                    File.Delete(_tempFilePath);
            }
            catch (Exception e)
            {
                Console.LogError(SystemNames.System, $"Failed to delete temporary file '{_tempFilePath}': {e.Message}");
            }
        }
        public static bool DeleteFileIfExists(string _filePath)
        {
            if (string.IsNullOrEmpty(_filePath))
                return false;
            
            try
            {
                if (File.Exists(_filePath))
                {
                    File.Delete(_filePath);
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.LogError(SystemNames.System, $"Failed to delete file '{_filePath}': {e.Message}");
                return false;
            }
        }
    }
}