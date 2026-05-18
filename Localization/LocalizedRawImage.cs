// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace CodaGame
{
    /// <summary>
    /// Automatically updates RawImage texture when language changes.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class LocalizedRawImage : LocalizedMonoBehaviour
    {
        [SerializeField] private string _m_textureKey;

        private RawImage _m_rawImage;


        private void Awake()
        {
            _m_rawImage = GetComponent<RawImage>();
        }

        protected override void Refresh()
        {
            if (!string.IsNullOrEmpty(_m_textureKey))
            {
                Texture texture = Localization.GetTexture(_m_textureKey);
                if (texture != null)
                    _m_rawImage.texture = texture;
            }
        }
    }
}
