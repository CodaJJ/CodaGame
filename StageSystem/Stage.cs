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
    /// Public facade for the Stage system. Manages loading and unloading of Unity Scenes as typed Stages.
    /// </summary>
    /// <remarks>
    /// <para>Two flavors of Stage:</para>
    /// <list type="bullet">
    /// <item><b>Main</b> (<see cref="_AMainStage"/>): at most one is loaded at a time. Becomes the Active Scene.
    /// Loading a new Main automatically unloads the previous one (after the new one finishes loading, no gap).</item>
    /// <item><b>Overlay</b> (<see cref="_AOverlayStage"/>): any number can coexist. Does not affect the Active Scene
    /// or other stages. Participates in ref counting.</item>
    /// </list>
    /// <para>Only Overlay stages can be unloaded by user code via <see cref="Unload"/>. Main stage lifetime is
    /// owned entirely by the framework: to leave a Main, load a different Main (e.g. a main-menu stage). There
    /// is no API to enter a "no Main" state once a Main has been loaded.</para>
    /// <para><b>Last-call-wins for overlapping LoadMain:</b> Successive <c>LoadMain</c> calls invalidate any
    /// in-flight Main load — only the most recent target ever becomes <see cref="currentMain"/>, regardless
    /// of load completion order. Superseded loads cancel themselves (scene unloaded, callback receives null).
    /// Wrap transitions in <c>Loading.Load</c> when user-visible loading feedback matters.</para>
    /// </remarks>
    public static class Stage
    {
        [NotNull] private static StageManager manager { get { return StageManager.instance; } }


        /// <summary>
        /// The currently loaded Main stage, or null if none. There is at most one Main stage at a time.
        /// </summary>
        public static _AMainStage currentMain { get { return manager.currentMain; } }


        /// <summary>
        /// Load a scene as the Main stage. Loading a new Main loads the new scene first, then atomically
        /// replaces the previous one (Active Scene + <see cref="currentMain"/>) and unloads it.
        /// </summary>
        /// <remarks>
        /// <para>On failure (load failure or the loaded stage is not an <see cref="_AMainStage"/>), the
        /// newly loaded scene is unloaded and the existing Main is preserved unchanged.</para>
        /// </remarks>
        /// <param name="_sceneAsset">The AssetIndex of the scene to load. The scene's root must carry an <see cref="_AMainStage"/> subclass.</param>
        /// <param name="_complete">Callback invoked when the operation completes. Receives null on failure.</param>
        public static void LoadMain(AssetIndex _sceneAsset, Action<_AMainStage> _complete = null)
        {
            manager.LoadMain(_sceneAsset, _complete);
        }
        /// <summary>
        /// Load a scene as the Main stage, casting to a specific subclass.
        /// </summary>
        /// <remarks>
        /// <para>If the loaded Stage is not of type <typeparamref name="T"/>, the newly loaded scene is
        /// unloaded, the existing Main is preserved unchanged, and the callback receives null.</para>
        /// </remarks>
        public static void LoadMain<T>(AssetIndex _sceneAsset, Action<T> _complete = null) where T : _AMainStage
        {
            manager.LoadMain<T>(_sceneAsset, _complete);
        }
        /// <summary>
        /// Load a scene as an Overlay stage. Multiple Overlay loads coexist; ref-counted per scene asset.
        /// Does not affect the Active Scene or any Main stage.
        /// </summary>
        /// <param name="_sceneAsset">The AssetIndex of the scene to load. The scene's root must carry an <see cref="_AOverlayStage"/> subclass.</param>
        /// <param name="_complete">Callback invoked when the load completes. Receives null on failure or type mismatch.</param>
        public static void LoadOverlay(AssetIndex _sceneAsset, Action<_AOverlayStage> _complete = null)
        {
            manager.LoadOverlay(_sceneAsset, _complete);
        }
        /// <summary>
        /// Load a scene as an Overlay stage, casting to a specific subclass.
        /// </summary>
        public static void LoadOverlay<T>(AssetIndex _sceneAsset, Action<T> _complete = null) where T : _AOverlayStage
        {
            manager.LoadOverlay<T>(_sceneAsset, _complete);
        }
        /// <summary>
        /// Unload an Overlay stage. Decrements the ref count; the scene only unloads when the count reaches zero.
        /// </summary>
        /// <remarks>
        /// Main stages cannot be unloaded directly — the framework owns their lifecycle. To leave a Main, load a
        /// different Main with <see cref="LoadMain(AssetIndex, Action{_AMainStage})"/>.
        /// </remarks>
        public static void Unload(_AOverlayStage _stage)
        {
            manager.Unload(_stage);
        }
    }
}
