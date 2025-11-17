// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// An interface for asynchronous operations with start and end methods that accept a completion callback.
    /// </summary>
    public interface _IAsyncOperation
    {
        public void Start(Action _complete);
        public void End(Action _complete);
    }
}