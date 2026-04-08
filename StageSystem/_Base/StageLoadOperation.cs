// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace CodaGame.Base
{
    /// <summary>
    /// Internal ref-counted async operation that manages a single scene's load/unload cycle.
    /// </summary>
    /// <remarks>
    /// <para><see cref="_AReferenceCountAsyncOperation.Start"/> triggers scene load when ref goes 0 → 1.</para>
    /// <para><see cref="_AReferenceCountAsyncOperation.End"/> triggers scene unload when ref drops to 0.</para>
    /// <para><b>Failure contract:</b> If <see cref="stage"/> is null after Start's callback fires, the load failed.
    /// The caller MUST call <see cref="_AReferenceCountAsyncOperation.End"/> to balance the ref count incremented
    /// by the failed Start; otherwise the operation leaks and cannot be cleaned up. <c>StageManager</c> handles
    /// this automatically in its <c>Load</c> wrapper.</para>
    /// </remarks>
    internal class StageLoadOperation : _AReferenceCountAsyncOperation
    {
        private readonly AssetIndex _m_assetIndex;

        private SceneInstance _m_sceneInstance;
        private _AStage _m_stage;


        public StageLoadOperation(AssetIndex _assetIndex)
        {
            _m_assetIndex = _assetIndex;
        }


        public AssetIndex assetIndex { get { return _m_assetIndex; } }
        /// <summary>
        /// The Stage component found in the loaded scene. Valid after <see cref="_AReferenceCountAsyncOperation.isStarted"/> is true. Null if load failed.
        /// </summary>
        public _AStage stage { get { return _m_stage; } }


        protected override void OnOperationStart(Action _complete)
        {
            AssetLoader.LoadSceneAsync(_m_assetIndex, _sceneInstance =>
            {
                if (!_sceneInstance.Scene.IsValid())
                {
                    Console.LogError(SystemNames.Stage, "Failed to load scene for AssetIndex " + _m_assetIndex);
                    _m_stage = null;
                    _complete.Invoke();
                    return;
                }

                _m_sceneInstance = _sceneInstance;

                // Find _AStage component on a root GameObject of the loaded scene.
                GameObject[] roots = _sceneInstance.Scene.GetRootGameObjects();
                _AStage found = null;
                if (roots != null)
                {
                    foreach (GameObject go in roots)
                    {
                        if (go == null) 
                            continue;
                        
                        _AStage comp = go.GetComponent<_AStage>();
                        if (comp != null)
                        {
                            found = comp;
                            break;
                        }
                    }
                }

                _m_stage = found;
                if (_m_stage != null)
                {
                    _m_stage.SetAssetIndex(_m_assetIndex);
                    _m_stage.TriggerLoaded();
                }
                _complete.Invoke();
            });
        }

        protected override void OnOperationEnd(Action _complete)
        {
            if (_m_stage != null)
            {
                _m_stage.TriggerUnloaded();
                _m_stage = null;
            }

            if (!_m_sceneInstance.Scene.IsValid())
            {
                _complete.Invoke();
                return;
            }

            SceneInstance toUnload = _m_sceneInstance;
            _m_sceneInstance = default;
            AssetLoader.UnloadSceneAsync(toUnload, () =>
            {
                _complete.Invoke();
            });
        }
    }
}
