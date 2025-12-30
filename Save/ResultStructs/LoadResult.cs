// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Result of a load operation.
    /// </summary>
    public struct LoadResult<T_DATA> where T_DATA : _AGameData
    {
        /// <summary>
        /// Indicates whether the load operation was successful.
        /// </summary>
        public bool success;
        /// <summary>
        /// Type of error if the load operation failed.
        /// </summary>
        public LoadFromSlotErrorType errorType;
        /// <summary>
        /// The loaded data if the operation was successful; otherwise, null.
        /// </summary>
        public T_DATA data;
        

        internal static LoadResult<T_DATA> Success([NotNull] T_DATA _data)
        {
            return new LoadResult<T_DATA>
            {
                success = true,
                errorType = LoadFromSlotErrorType.None,
                data = _data
            };
        }
        internal static LoadResult<T_DATA> Failure(LoadFromSlotErrorType _errorType)
        {
            return new LoadResult<T_DATA>
            {
                success = false,
                errorType = _errorType,
                data = null
            };
        }
    }
}
