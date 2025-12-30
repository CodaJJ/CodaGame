// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// Result of a save operation.
    /// </summary>
    public struct SaveResult
    {
        /// <summary>
        /// Indicates whether the save operation was successful.
        /// </summary>
        public bool success;
        /// <summary>
        /// Type of error if the save operation failed.
        /// </summary>
        public SaveToSlotErrorType errorType;
        

        internal static SaveResult Success()
        {
            return new SaveResult
            {
                success = true,
                errorType = SaveToSlotErrorType.None
            };
        }
        internal static SaveResult Failure(SaveToSlotErrorType _errorType)
        {
            return new SaveResult
            {
                success = false,
                errorType = _errorType
            };
        }
    }
}
