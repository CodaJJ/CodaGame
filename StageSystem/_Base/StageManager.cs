// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace CodaGame.Base
{
    /// <summary>
    /// Internal manager for the Stage system. Maintains per-AssetIndex ref-counted load operations.
    /// </summary>
    internal class StageManager
    {
        [NotNull] public static StageManager instance { get { return _g_instance ??= new StageManager(); } }
        private static StageManager _g_instance;


        [NotNull] private readonly Dictionary<AssetIndex, StageLoadOperation> _m_operations;


        private StageManager()
        {
            _m_operations = new Dictionary<AssetIndex, StageLoadOperation>();
        }


        /// <summary>
        /// Load a scene. Increments ref count; triggers async load if this is the first reference.
        /// </summary>
        public void Load(AssetIndex _assetIndex, Action<_AStage> _complete)
        {
            if (!_assetIndex.isValid)
            {
                Console.LogWarning(SystemNames.Stage, "Stage.Load called with invalid AssetIndex.");
                _complete?.Invoke(null);
                return;
            }

            if (!_m_operations.TryGetValue(_assetIndex, out StageLoadOperation op))
            {
                op = new StageLoadOperation(_assetIndex);
                _m_operations.Add(_assetIndex, op);
            }

            op.Start(() =>
            {
                _AStage stage = op.stage;
                if (stage == null)
                {
                    Console.LogError(SystemNames.Stage, "Scene loaded but no _AStage component found on any root GameObject, " +
                                                        "The ref count will be automatically decremented. AssetIndex: " + _assetIndex);
                    Unload(_assetIndex);
                    _complete?.Invoke(null);
                    return;
                }

                _complete?.Invoke(stage);
            });
        }
        /// <summary>
        /// Load a scene and cast the Stage to a specific subclass. On type mismatch, unloads the scene and invokes the callback with null.
        /// </summary>
        public void Load<T>(AssetIndex _assetIndex, Action<T> _complete) where T : _AStage
        {
            Load(_assetIndex, _stage =>
            {
                if (_stage == null)
                {
                    _complete?.Invoke(null);
                    return;
                }

                if (_stage is T typed)
                {
                    _complete?.Invoke(typed);
                    return;
                }

                Console.LogError(SystemNames.Stage, "Stage type mismatch: loaded " + _stage.GetType().Name + " but expected " + typeof(T).Name + ". Rolling back.");
                Unload(_stage);
                _complete?.Invoke(null);
            });
        }
        /// <summary>
        /// Unload a stage. Decrements ref count; triggers async unload when ref drops to zero.
        /// </summary>
        public void Unload(_AStage _stage)
        {
            if (_stage == null)
            {
                Console.LogWarning(SystemNames.Stage, "Stage.Unload called with null stage.");
                return;
            }

            Unload(_stage.assetIndex);
        }
        /// <summary>
        /// Unload a stage. Decrements ref count; triggers async unload when ref drops to zero.
        /// </summary>
        public void Unload(AssetIndex _assetIndex)
        {
            if (!_assetIndex.isValid)
            {
                Console.LogWarning(SystemNames.Stage, "Stage.Unload called with stage that has invalid AssetIndex. Was it loaded via Stage.Load?");
                return;
            }

            if (!_m_operations.TryGetValue(_assetIndex, out StageLoadOperation op))
            {
                Console.LogWarning(SystemNames.Stage, "Stage.Unload: no active operation for AssetIndex " + _assetIndex + ". Already unloaded?");
                return;
            }

            op.End();
            if (op.referenceCount <= 0)
                _m_operations.Remove(_assetIndex);
        }
    }
}
