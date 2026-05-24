// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using TMPro;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Automatically updates TMP_Text content and font when language changes.
    /// </summary>
    public abstract class _ALocalizedText : LocalizedMonoBehaviour
    {
        [SerializeField] private string _m_textKey;
        [SerializeField] private string _m_fontKey;

        private TMP_Text _m_text;


        private void Awake()
        {
            _m_text = GetComponent<TMP_Text>();
        }

        protected override void Refresh()
        {
            // Update text
            if (!string.IsNullOrEmpty(_m_textKey))
            {
                _m_text.text = Localization.GetText(_m_textKey);
            }

            // Update font
            if (!string.IsNullOrEmpty(_m_fontKey))
            {
                TMP_FontAsset font = Localization.GetFont(_m_fontKey);
                if (font != null)
                    _m_text.font = font;
            }
        }
    }
}
