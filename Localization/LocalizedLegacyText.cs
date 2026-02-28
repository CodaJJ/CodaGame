// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace CodaGame
{
    /// <summary>
    /// Automatically updates legacy Text content and font when language changes.
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocalizedLegacyText : LocalizedMonoBehaviour
    {
        [SerializeField] private string _m_textKey;
        [SerializeField] private string _m_fontKey;

        private Text _m_text;


        protected override void OnEnable()
        {
            _m_text = GetComponent<Text>();
            base.OnEnable();
        }

        protected override void Refresh()
        {
            if (_m_text == null)
                return;

            // Update text
            if (!string.IsNullOrEmpty(_m_textKey))
            {
                _m_text.text = Localization.GetText(_m_textKey);
            }

            // Update font
            if (!string.IsNullOrEmpty(_m_fontKey))
            {
                Font font = Localization.GetLegacyFont(_m_fontKey);
                if (font != null)
                    _m_text.font = font;
            }
        }
    }
}
