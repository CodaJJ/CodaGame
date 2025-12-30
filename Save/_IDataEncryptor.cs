// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// Interface for encrypting and decrypting save data.
    /// Implement this interface to provide custom encryption for save files.
    /// </summary>
    public interface _IDataEncryptor
    {
        /// <summary>
        /// Encrypts the given JSON string.
        /// </summary>
        /// <param name="_jsonData">The JSON string to encrypt</param>
        /// <returns>The encrypted string, or null if encryption fails</returns>        
        string Encrypt(string _jsonData);
        /// <summary>
        /// Decrypts the given encrypted string back to JSON.
        /// </summary>
        /// <param name="_encryptedData">The encrypted string to decrypt</param>
        /// <returns>The decrypted JSON string, or null if decryption fails</returns>
        string Decrypt(string _encryptedData);
    }
}
