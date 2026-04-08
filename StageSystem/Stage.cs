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
    /// <para>Load/Unload are reference counted: multiple Load calls for the same scene increment a counter,
    /// and the scene is only unloaded when the counter drops to zero via matching Unload calls.</para>
    /// </remarks>
    public static class Stage
    {
        [NotNull] private static StageManager manager { get { return StageManager.instance; } }


        /// <summary>
        /// Load a scene and retrieve its <see cref="_AStage"/> component.
        /// </summary>
        /// <param name="_sceneAsset">The AssetIndex of the scene to load</param>
        /// <param name="_complete">Callback invoked when the load completes. Receives null on failure.</param>
        public static void Load(AssetIndex _sceneAsset, Action<_AStage> _complete = null)
        {
            manager.Load(_sceneAsset, _complete);
        }
        /// <summary>
        /// Load a scene and retrieve its <see cref="_AStage"/> component cast to a specific subclass.
        /// </summary>
        /// <remarks>
        /// <para>If the loaded Stage is not of type <typeparamref name="T"/>, the scene is unloaded and the callback receives null.</para>
        /// </remarks>
        /// <param name="_sceneAsset">The AssetIndex of the scene to load</param>
        /// <param name="_complete">Callback invoked when the load completes. Receives null on failure or type mismatch.</param>
        public static void Load<T>(AssetIndex _sceneAsset, Action<T> _complete = null) where T : _AStage
        {
            manager.Load<T>(_sceneAsset, _complete);
        }
        /// <summary>
        /// Unload a previously loaded stage. Decrements the reference count; the scene is only unloaded when the count reaches zero.
        /// </summary>
        public static void Unload(_AStage _stage)
        {
            manager.Unload(_stage);
        }
    }
}
