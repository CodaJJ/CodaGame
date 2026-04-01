// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Public API for the Flow system.
    /// </summary>
    /// <remarks>
    /// <para>Enter flows via <see cref="Enter"/>. Each flow can contain game worlds, UI panels, or both.</para>
    /// <para>Exit the topmost flow via <see cref="Exit()"/>, with optional name or reference validation.</para>
    /// <para>Flow lifecycle is managed on the flow instance itself (see <see cref="_AFlow"/>).</para>
    /// </remarks>
    [PublicAPI]
    public static class Flow
    {
        [NotNull] private static FlowManager manager { get { return FlowManager.instance; } }

        /// <summary>
        /// The topmost active flow, or null if no flows are active.
        /// </summary>
        public static _AFlow current { get { return manager.current; } }


        /// <summary>
        /// Enter a new flow with the specified mode and optional loading screen.
        /// </summary>
        /// <remarks>
        /// <para>If a transition is in progress, the operation is queued and executed after the current transition completes.</para>
        /// <para>
        /// When a loading show is provided, the flow system shows it before executing exits/pauses,
        /// then hides it after the new flow's OnEnter completes.
        /// </para>
        /// </remarks>
        /// <param name="_flow">The flow instance to enter.</param>
        /// <param name="_mode">The entry mode determining how existing flows are affected.</param>
        /// <param name="_loadingShow">Optional loading screen to cover the transition.</param>
        public static void Enter(_AFlow _flow, EFlowMode _mode, _ILoadingShow _loadingShow = null)
        {
            if (_flow == null)
            {
                Console.LogWarning(SystemNames.Flow, "Cannot enter a null flow.");
                return;
            }
            manager.Enter(_flow, _mode, _loadingShow);
        }
        /// <summary>
        /// Exit the topmost active flow without validation.
        /// </summary>
        /// <remarks>
        /// <para>If a transition is in progress, the operation is queued.</para>
        /// </remarks>
        public static void Exit()
        {
            manager.Exit();
        }
        /// <summary>
        /// Exit the topmost active flow with name validation.
        /// </summary>
        /// <remarks>
        /// <para>If the topmost flow's <see cref="_AFlow.flowName"/> does not match, an error is logged and no exit occurs.</para>
        /// <para>If a transition is in progress, the operation is queued.</para>
        /// </remarks>
        /// <param name="_name">The expected name of the topmost flow.</param>
        public static void Exit(string _name)
        {
            if (string.IsNullOrEmpty(_name))
            {
                Console.LogWarning(SystemNames.Flow, "Cannot exit with a null or empty name.");
                return;
            }
            manager.Exit(_name);
        }
        /// <summary>
        /// Exit the topmost active flow with reference validation.
        /// </summary>
        /// <remarks>
        /// <para>If the provided flow is not the topmost, an error is logged and no exit occurs.</para>
        /// <para>If a transition is in progress, the operation is queued.</para>
        /// </remarks>
        /// <param name="_flow">The expected topmost flow instance.</param>
        public static void Exit(_AFlow _flow)
        {
            if (_flow == null)
            {
                Console.LogWarning(SystemNames.Flow, "Cannot exit a null flow.");
                return;
            }
            manager.Exit(_flow);
        }
    }
}
