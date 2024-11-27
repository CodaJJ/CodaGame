// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;

namespace CodaGame
{
    /// <summary>
    /// The interface for loading show.
    /// </summary>
    /// <remarks>
    /// <para>Register this interface with the Loading module, and let the module handle the loading process with the loading show.</para>
    /// <para>You don't need to worry about whether 'Hide' is called in 'Show' or 'Show' is called in 'Hide'; the Loading module will handle it.</para>
    /// </remarks>
    public interface _ILoadingShow
    {
        /// <summary>
        /// The name of the loading show.
        /// </summary>
        string name { get; }
        /// <summary>
        /// Play the show before the loading function starts running.
        /// </summary>
        /// <remarks>
        /// <para>The loading function will run right after the '_complete' action is called.</para>
        /// </remarks>
        void Show(Action _complete);
        /// <summary>
        /// Hide the show after the loading function completes.
        /// </summary>
        void Hide(Action _complete);
    }
}