// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace CodaGame.Base
{
    /// <summary>
    /// Internal manager for the Stage system. Maintains per-AssetIndex ref-counted load operations,
    /// owns Main-stage lifecycle, and manages the Unity Active Scene around Main stage swaps.
    /// </summary>
    /// <remarks>
    /// <para><b>Main lifecycle:</b> The framework owns Main stage lifecycle entirely. Public API only allows
    /// <c>LoadMain</c> (loading or switching). To "exit" a Main, load a different Main (e.g. a main-menu stage).
    /// There is no public way to unload a Main into a "no Main" state.</para>
    /// <para><b>Main swap (load-then-swap):</b> When <c>LoadMain</c> is called with a different asset, the new
    /// scene is loaded first. If it loads successfully and validates as <see cref="_AMainStage"/> (and the
    /// requested subclass when called via the generic overload), the swap happens atomically: the new scene
    /// becomes the Active Scene, <c>currentMain</c> is replaced, and the old Main is then unloaded. There is no
    /// transient no-Main state. If validation fails, the newly loaded scene is unloaded and the existing Main
    /// remains unchanged.</para>
    /// <para><b>Last-call-wins for overlapping LoadMain:</b> If multiple <c>LoadMain</c> calls overlap in
    /// flight, only the most recently issued target ever becomes <c>currentMain</c>, regardless of which load
    /// finishes first. Superseded loads cancel themselves (unload the scene, invoke their callback with null)
    /// via a serialize captured on every <c>LoadMain</c> entry. Note: this protects ordering but is not a
    /// substitute for wrapping transitions in <c>Loading.Load</c> when user-visible loading feedback matters
    /// — multiple superseded loads still spend time on the disc.</para>
    /// </remarks>
    internal class StageManager
    {
        [NotNull] public static StageManager instance { get { return _g_instance ??= new StageManager(); } }
        private static StageManager _g_instance;


        [NotNull] private readonly Dictionary<AssetIndex, StageLoadOperation> _m_operations;
        // The current Main stage. Null only before the first LoadMain succeeds (i.e. the boot state).
        private _AMainStage _m_currentMain;
        // Refreshed on every LoadMain call (including fast-path same-asset returns) via Serialize.Next().
        // Each in-flight load captures the serialize value when it started; if a later LoadMain refreshes
        // the field before that load's callback fires, the in-flight load knows it has been superseded and
        // cancels itself. Makes out-of-order completion safe: only the most recent LoadMain target ever
        // becomes currentMain.
        private uint _m_mainLoadSerialize;


        private StageManager()
        {
            _m_operations = new Dictionary<AssetIndex, StageLoadOperation>();
        }


        /// <summary>
        /// The current Main stage, or null if none has been loaded yet (boot state). After the first successful
        /// <c>LoadMain</c>, this is never null again under normal use — switching Mains replaces atomically.
        /// </summary>
        public _AMainStage currentMain { get { return _m_currentMain; } }
        

        // Load-and-swap path for Main stages. On success: new Main becomes Active Scene and replaces currentMain,
        // then the old Main is unloaded. On failure (load failure or type mismatch): the newly loaded scene is
        // unloaded (if any) and currentMain remains untouched. If a later LoadMain call supersedes this one
        // before its callback fires, this load cancels itself (unload + callback null).
        public void LoadMain<T>(AssetIndex _assetIndex, Action<T> _complete) where T : _AMainStage
        {
            // Refresh on every LoadMain entry — including fast-path returns — so any in-flight load started
            // before this call is invalidated.
            uint mySerialize = _m_mainLoadSerialize = Serialize.Next();

            if (!_assetIndex.isValid)
            {
                Console.LogWarning(SystemNames.Stage, $"Stage.LoadMain<{typeof(T).Name}> called with invalid AssetIndex.");
                _complete?.Invoke(null);
                return;
            }

            // Re-loading the same asset → no-op when the existing currentMain is compatible with T.
            if (_m_currentMain != null && _m_currentMain.assetIndex.Equals(_assetIndex))
            {
                if (_m_currentMain is T cached)
                {
                    _complete?.Invoke(cached);
                    return;
                }
                Console.LogError(SystemNames.Stage, $"Stage.LoadMain<{typeof(T).Name}>: currentMain is the same asset but its type is {_m_currentMain.GetType().Name}.");
                _complete?.Invoke(null);
                return;
            }

            LoadInternal(_assetIndex, _stage =>
            {
                // If a newer LoadMain call has been issued, abandon this one regardless of how it would
                // have resolved. Scenes still get unloaded cleanly via the ref-counted op.
                if (mySerialize != _m_mainLoadSerialize)
                {
                    if (_stage != null)
                        UnloadByAssetIndex(_assetIndex);
                    _complete?.Invoke(null);
                    return;
                }

                if (_stage == null)
                {
                    _complete?.Invoke(null);
                    return;
                }

                if (_stage is not T typed)
                {
                    Console.LogError(SystemNames.Stage, $"Stage.LoadMain<{typeof(T).Name}>: loaded stage is {_stage.GetType().Name}. Rejecting new load; existing Main is unchanged.");
                    UnloadByAssetIndex(_assetIndex);
                    _complete?.Invoke(null);
                    return;
                }

                _AMainStage previous = _m_currentMain;
                _m_currentMain = typed;
                SceneManager.SetActiveScene(typed.gameObject.scene);
                if (previous != null)
                    UnloadByAssetIndex(previous.assetIndex);

                _complete?.Invoke(typed);
            });
        }
        // Overlay load impl. On failure (load failure or type mismatch): the newly loaded scene is unloaded.
        // Never interacts with Main state.
        public void LoadOverlay<T>(AssetIndex _assetIndex, Action<T> _complete) where T : _AOverlayStage
        {
            if (!_assetIndex.isValid)
            {
                Console.LogWarning(SystemNames.Stage, $"Stage.LoadOverlay<{typeof(T).Name}> called with invalid AssetIndex.");
                _complete?.Invoke(null);
                return;
            }

            LoadInternal(_assetIndex, _stage =>
            {
                if (_stage == null)
                {
                    _complete?.Invoke(null);
                    return;
                }

                if (_stage is not T typed)
                {
                    Console.LogError(SystemNames.Stage, $"Stage.LoadOverlay<{typeof(T).Name}>: loaded stage is {_stage.GetType().Name}. Rejecting new load.");
                    UnloadByAssetIndex(_assetIndex);
                    _complete?.Invoke(null);
                    return;
                }

                _complete?.Invoke(typed);
            });
        }
        /// <summary>
        /// Unload an Overlay stage. Decrements ref count; the scene only unloads when the count reaches zero.
        /// </summary>
        public void Unload(_AOverlayStage _stage)
        {
            if (_stage == null)
            {
                Console.LogWarning(SystemNames.Stage, "Stage.Unload called with null stage.");
                return;
            }

            UnloadByAssetIndex(_stage.assetIndex);
        }


        // Shared core load: ref-counted scene load producing an _AStage. Used by both Main and Overlay paths.
        private void LoadInternal(AssetIndex _assetIndex, Action<_AStage> _complete)
        {
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
                    Console.LogError(SystemNames.Stage, $"Scene loaded but no _AStage component was found on any root GameObject. The ref count will be automatically decremented. AssetIndex: {_assetIndex}");
                    UnloadByAssetIndex(_assetIndex);
                    _complete?.Invoke(null);
                    return;
                }

                _complete?.Invoke(stage);
            });
        }
        // Decrement ref count; remove op when zero.
        private void UnloadByAssetIndex(AssetIndex _assetIndex)
        {
            if (!_assetIndex.isValid)
            {
                Console.LogWarning(SystemNames.Stage, "Stage unload called with invalid AssetIndex. Was the stage loaded via the Stage facade?");
                return;
            }

            if (!_m_operations.TryGetValue(_assetIndex, out StageLoadOperation op))
            {
                Console.LogWarning(SystemNames.Stage, $"Stage unload: no active operation for AssetIndex {_assetIndex}. Already unloaded?");
                return;
            }

            op.End();
            if (op.referenceCount <= 0)
                _m_operations.Remove(_assetIndex);
        }
    }
}
