// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Base class for components that need to update when language changes.
    /// </summary>
    public abstract class LocalizedMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// Called after the component is enabled, before the first Refresh.
        /// Override to do one-shot setup on enable.
        /// </summary>
        protected virtual void OnEnabled() { }
        /// <summary>
        /// Called after the component is disabled.
        /// Override to do cleanup on disable.
        /// </summary>
        protected virtual void OnDisabled() { }
        /// <summary>
        /// Called when the component is enabled or when the language changes.
        /// Implement this to update the component's content.
        /// </summary>
        protected abstract void Refresh();


        private void OnEnable()
        {
            Localization.onLanguageChanged += OnLanguageChanged;
            OnEnabled();
            Refresh();
        }
        private void OnDisable()
        {
            Localization.onLanguageChanged -= OnLanguageChanged;
            OnDisabled();
        }
        private void OnLanguageChanged(int _language)
        {
            Refresh();
        }
    }
}
