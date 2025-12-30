// Copyright (c) 2025 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    public enum LoadFromSlotErrorType
    {
        None,
        
        SerializationError,
        
        FileNotFound,
        FileCorrupted,
        
        AccessDenied,
        PathError,
        
        Unknown,
    }
}