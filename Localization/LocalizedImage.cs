// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace CodaGame
{
    /// <summary>
    /// Automatically updates Image sprite when language changes.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class LocalizedImage : LocalizedMonoBehaviour
    {
        [SerializeField] private string _m_spriteKey;

        private Image _m_image;


        private void Awake()
        {
            _m_image = GetComponent<Image>();
        }

        protected override void Refresh()
        {
            if (!string.IsNullOrEmpty(_m_spriteKey))
            {
                Sprite sprite = Localization.GetSprite(_m_spriteKey);
                if (sprite != null)
                    _m_image.sprite = sprite;
            }
        }
    }
}
