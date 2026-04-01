// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// Abstract base class for dynamically loaded UI widgets.
    /// </summary>
    /// <remarks>
    /// <para>Created via <see cref="UI.CreatePrefab"/>. Auto-registers with the nearest owner in the hierarchy.</para>
    /// <para>Unlike static <see cref="_AUIWidget"/>, prefab widgets can be individually destroyed via <see cref="Destroy"/>.</para>
    /// </remarks>
    [PublicAPI]
    public abstract class _AUIPrefab : _AUIWidget
    {
        private _AUIComponent _m_owner;


        /// <summary>
        /// Destroy this prefab widget: unregister from owner, trigger lifecycle, and release asset.
        /// </summary>
        public void Destroy()
        {
            if (isDestroyed)
                return;

            Console.LogVerbose(SystemNames.UI, widgetName, "Destroy requested.");

            // Unregister from owner
            if (_m_owner != null)
            {
                _m_owner.UnregisterChildWidget(this);
                _m_owner = null;
            }

            // Full destroy lifecycle
            TriggerDestroy();
        }


        /// <summary>
        /// Override to release asset after destroy lifecycle.
        /// </summary>
        internal override void TriggerDestroy()
        {
            if (isDestroyed)
                return;

            base.TriggerDestroy();

            // Release dynamically loaded asset
            AssetLoader.ReleaseInstance(gameObject);
        }

        /// <summary>
        /// Set the owner of this prefab widget.
        /// </summary>
        internal void SetOwner(_AUIComponent _owner)
        {
            _m_owner = _owner;
        }
    }
}
