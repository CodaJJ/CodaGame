// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Abstract base class for a Stage — a typed entry point for a Unity Scene managed by the Stage system.
    /// </summary>
    /// <remarks>
    /// <para>Attach a concrete subclass to exactly one root GameObject of a scene.</para>
    /// <para>After the scene is loaded, <see cref="Base.StageManager"/> finds the component and sets <see cref="assetIndex"/>.</para>
    /// <para>Use <see cref="OnLoaded"/> and <see cref="OnUnloaded"/> for one-time initialization and cleanup.</para>
    /// </remarks>
    [DisallowMultipleComponent]
    public abstract class _AStage : MonoBehaviour
    {
        // Set by StageManager after the scene is loaded. Used to look up the owning operation on Unload.
        private AssetIndex _m_assetIndex = AssetIndex.Invalid;


        /// <summary>
        /// The AssetIndex of the scene this Stage belongs to. Set by <c>StageManager</c> after load.
        /// </summary>
        internal AssetIndex assetIndex { get { return _m_assetIndex; } }


        /// <summary>
        /// Called once after the scene is loaded and the Stage is discovered (ref 0 → 1).
        /// </summary>
        protected virtual void OnLoaded() { }
        /// <summary>
        /// Called once when the reference count drops to zero, immediately before the scene is unloaded.
        /// </summary>
        protected virtual void OnUnloaded() { }


        // Set the asset index. Called by StageLoadOperation after scene discovery.
        internal void SetAssetIndex(AssetIndex _assetIndex)
        {
            _m_assetIndex = _assetIndex;
        }
        // Invoke OnLoaded. Called by StageLoadOperation.
        internal void TriggerLoaded()
        {
            OnLoaded();
        }
        // Invoke OnUnloaded. Called by StageLoadOperation.
        internal void TriggerUnloaded()
        {
            OnUnloaded();
        }
    }
}
