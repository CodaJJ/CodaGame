// Copyright (c) 2025 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame
{
    /// <summary>
    /// Severity of an inspector message. Mirrors UnityEditor.MessageType
    /// values so editor code can cast directly when drawing.
    /// </summary>
    public enum EMessageType
    {
        None = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
    }
}
