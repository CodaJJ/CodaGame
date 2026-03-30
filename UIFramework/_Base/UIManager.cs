// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// Internal manager for UI panel creation and tracking.
    /// </summary>
    internal class UIManager
    {
        [NotNull] public static UIManager instance { get { return _g_instance ??= new UIManager(); } }
        private static UIManager _g_instance;


        [ItemNotNull, NotNull] private readonly HashSet<_AUIPanel> _m_panels;


        private UIManager()
        {
            _m_panels = new HashSet<_AUIPanel>();
        }


        /// <summary>
        /// Create a panel instance from the given asset index.
        /// </summary>
        public _AUIPanel Create(AssetIndex _panelAsset)
        {
            if (!_panelAsset.isValid)
            {
                Console.LogWarning(SystemNames.UI, "Invalid asset index.");
                return null;
            }

            GameObject go = AssetLoader.InstantiateSync(_panelAsset);
            if (go == null)
            {
                Console.LogError(SystemNames.UI, "Failed to instantiate panel prefab.");
                return null;
            }

            _AUIPanel panel = go.GetComponent<_AUIPanel>();
            if (panel == null)
            {
                Console.LogError(SystemNames.UI, "Instantiated prefab does not have an _AUIPanel component.");
                AssetLoader.ReleaseInstance(go);
                return null;
            }

            // Setup
            go.transform.SetParent(GetLayerRoot(panel.layer), false);
            panel.Init(_panelAsset);

            _m_panels.Add(panel);

            Console.LogVerbose(SystemNames.UI, panel.panelName, "Panel created.");
            return panel;
        }
        /// <summary>
        /// Destroy a panel immediately, skipping any hide animation.
        /// </summary>
        public void DestroyImmediate(_AUIPanel _panel)
        {
            if (_panel == null || _panel.isDestroyed)
                return;
            
            _panel.DestroyImmediate();
        }
        /// <summary>
        /// Destroy all currently tracked panels immediately.
        /// </summary>
        public void DestroyAllImmediate()
        {
            if (_m_panels.Count == 0)
                return;

            List<_AUIPanel> panels = new List<_AUIPanel>(_m_panels);
            foreach (_AUIPanel panel in panels)
                panel.DestroyImmediate();
        }
        
        
        /// <summary>
        /// Destroy a panel: unregister from tracking and release the asset instance.
        /// </summary>
        internal void DestroyPanel(_AUIPanel _panel)
        {
            if (_panel == null)
                return;
            
            _m_panels.Remove(_panel);
            AssetLoader.ReleaseInstance(_panel.gameObject);
        }


        private static Transform GetLayerRoot(int _layer)
        {
            ReadOnlyList<Transform> layers = GameMain.instance.uiLayers;
            if (layers.Count == 0)
            {
                Console.LogError(SystemNames.UI, "GameMain.uiLayers is not configured.");
                return null;
            }

            if (_layer >= 0 && _layer < layers.Count)
                return layers[_layer];

            Console.LogWarning(SystemNames.UI, $"Layer {_layer} is out of range. Falling back to layer 0.");
            return layers[0];
        }
    }
}
