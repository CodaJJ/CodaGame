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
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizedText : LocalizedMonoBehaviour
    {
        [SerializeField] private string _textKey;
        [SerializeField] private string _fontKey;

        private TMP_Text _m_text;


        protected override void OnEnable()
        {
            _m_text = GetComponent<TMP_Text>();
            base.OnEnable();
        }

        protected override void Refresh()
        {
            if (_m_text == null)
                return;

            // Update text
            if (!string.IsNullOrEmpty(_textKey))
            {
                _m_text.text = Localization.GetText(_textKey);
            }

            // Update font
            if (!string.IsNullOrEmpty(_fontKey))
            {
                TMP_FontAsset font = Localization.GetFont(_fontKey);
                if (font != null)
                    _m_text.font = font;
            }
        }
    }
}
