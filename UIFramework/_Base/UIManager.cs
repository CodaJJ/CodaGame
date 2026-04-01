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
        /// Create a widget instance from the given asset index, parented to a custom transform.
        /// The widget auto-registers with its nearest owner (_AUIWidget or _AUIPanel) in the hierarchy.
        /// </summary>
        public _AUIPrefab CreatePrefab(AssetIndex _widgetAsset, Transform _parent)
        {
            if (!_widgetAsset.isValid)
            {
                Console.LogWarning(SystemNames.UI, "Invalid asset index for widget prefab.");
                return null;
            }

            if (_parent == null)
            {
                Console.LogWarning(SystemNames.UI, "Cannot create widget prefab with null parent.");
                return null;
            }

            GameObject go = AssetLoader.InstantiateSync(_widgetAsset);
            if (go == null)
            {
                Console.LogError(SystemNames.UI, "Failed to instantiate widget prefab.");
                return null;
            }

            _AUIPrefab widget = go.GetComponent<_AUIPrefab>();
            if (widget == null)
            {
                Console.LogError(SystemNames.UI, "Instantiated prefab does not have an _AUIPrefab component.");
                AssetLoader.ReleaseInstance(go);
                return null;
            }

            // Parent first, then search for owner
            go.transform.SetParent(_parent, false);

            // Auto-register with nearest owner
            _AUIComponent owner = null;
            go.transform.TraverseParents(parent =>
            {
                owner = parent.GetComponent<_AUIComponent>();
                return owner != null ? ETraverseOp.Stop : ETraverseOp.Continue;
            });

            // Init first, then register (Register may TriggerShow if owner is active)
            widget.Init();

            if (owner != null)
            {
                owner.RegisterChildWidget(widget);
            }
            else
            {
                Console.LogWarning(SystemNames.UI, widget.widgetName,
                    "Widget prefab has no owner (_AUIPanel or _AUIWidget) in its parent hierarchy. Lifecycle events will not propagate.");
            }

            Console.LogVerbose(SystemNames.UI, widget.widgetName, "Widget prefab created.");
            return widget;
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
