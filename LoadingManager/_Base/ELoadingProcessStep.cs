// Copyright (c) 2024 Coda
// 
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

namespace CodaGame.Base
{
    public enum ELoadingProcessStep
    {
        /// <summary>
        /// Idle state. Waiting for load functions.
        /// </summary>
        Idle,
        /// <summary>
        /// Start loading show. The load functions will be started after the loading show done.
        /// </summary>
        LoadingShowStart,
        /// <summary>
        /// Loading state. Loading the load functions.
        /// </summary>
        Loading,
        /// <summary>
        /// End loading show. The loading show will be ended after the load functions done.
        /// </summary>
        LoadingShowEnd
    }
}