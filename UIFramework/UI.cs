// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Public API for the UI panel system.
    /// </summary>
    /// <remarks>
    /// <para>Create panel instances via <see cref="Create"/>. Each call creates a new instance.</para>
    /// <para>Panel lifecycle (Open/Hide) is managed on the panel instance itself.</para>
    /// <para>Destroy panels via <see cref="Destroy"/> or <see cref="_AUIPanel.Destroy(Action)"/>.</para>
    /// <para>Layer root transforms are configured on <see cref="GameMain"/>.</para>
    /// </remarks>
    [PublicAPI]
    public static class UI
    {
        [NotNull] private static UIManager manager { get { return UIManager.instance; } }


        /// <summary>
        /// Create a new panel instance from the given asset index, in Hidden state.
        /// </summary>
        /// <remarks>
        /// <para>The panel's <see cref="_AUIPanel.OnPanelInit"/> is called after creation.</para>
        /// <para>Use <see cref="_AUIPanel.Open"/> to show the panel.</para>
        /// </remarks>
        /// <param name="_panelAsset">The asset index identifying the panel prefab.</param>
        /// <returns>The panel instance, or null on failure.</returns>
        public static _AUIPanel Create(AssetIndex _panelAsset)
        {
            return manager.Create(_panelAsset);
        }
        /// <summary>
        /// Create a new panel instance with a typed return value, in Hidden state.
        /// </summary>
        /// <remarks>
        /// <para>The panel's <see cref="_AUIPanel.OnPanelInit"/> is called after creation.</para>
        /// <para>Use <see cref="_AUIPanel.Open"/> to show the panel.</para>
        /// </remarks>
        /// <typeparam name="T">The expected panel type.</typeparam>
        /// <param name="_panelAsset">The asset index identifying the panel prefab.</param>
        /// <returns>The typed panel instance, or null on failure or type mismatch.</returns>
        public static T Create<T>(AssetIndex _panelAsset) where T : _AUIPanel
        {
            _AUIPanel panel = manager.Create(_panelAsset);
            if (panel is T typed)
                return typed;

            if (panel != null)
            {
                Console.LogError(SystemNames.UI, $"Panel type mismatch. Expected {typeof(T).Name}, got {panel.GetType().Name}.");
                panel.DestroyImmediate();
            }
            return null;
        }
        /// <summary>
        /// Destroy a panel and release its assets.
        /// </summary>
        /// <remarks>
        /// <para>If the panel is active, it will be auto-hidden first.</para>
        /// <para>Equivalent to calling <see cref="_AUIPanel.Destroy(Action)"/> on the panel instance.</para>
        /// </remarks>
        /// <param name="_panel">The panel instance to destroy.</param>
        /// <param name="_complete">Callback invoked when the operation completes.</param>
        public static void Destroy(_AUIPanel _panel, Action _complete = null)
        {
            if (_panel == null)
            {
                Console.LogWarning(SystemNames.UI, "Cannot destroy a null panel.");
                _complete?.Invoke();
                return;
            }
            _panel.Destroy(_complete);
        }
        /// <summary>
        /// Destroy a panel immediately, skipping any hide animation. Pending callbacks are still invoked.
        /// </summary>
        /// <remarks>
        /// <para>Use as a safety net for error recovery or scene transitions.</para>
        /// </remarks>
        /// <param name="_panel">The panel instance to destroy immediately.</param>
        public static void DestroyImmediate(_AUIPanel _panel)
        {
            manager.DestroyImmediate(_panel);
        }
        /// <summary>
        /// Destroy all currently tracked panels immediately. Pending callbacks are still invoked.
        /// </summary>
        /// <remarks>
        /// <para>Use as a safety net for error recovery or scene transitions.</para>
        /// </remarks>
        public static void DestroyAllImmediate()
        {
            manager.DestroyAllImmediate();
        }
    }
}
