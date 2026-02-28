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
        protected virtual void OnEnable()
        {
            Localization.onLanguageChanged += OnLanguageChanged;
            Refresh();
        }
        protected virtual void OnDisable()
        {
            Localization.onLanguageChanged -= OnLanguageChanged;
        }
        /// <summary>
        /// Called when the component is enabled or when the language changes.
        /// Implement this to update the component's content.
        /// </summary>
        protected abstract void Refresh();
        

        private void OnLanguageChanged(int _language)
        {
            Refresh();
        }
    }
}
