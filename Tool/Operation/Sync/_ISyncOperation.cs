// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// An interface for synchronous operations with start and end methods.
    /// </summary>
    public interface _ISyncOperation
    {
        public void Start();
        public void End();
    }
}
