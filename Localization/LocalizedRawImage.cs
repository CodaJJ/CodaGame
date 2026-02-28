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
        [SerializeField] private string _textureKey;

        private RawImage _m_rawImage;


        protected override void OnEnable()
        {
            _m_rawImage = GetComponent<RawImage>();
            base.OnEnable();
        }

        protected override void Refresh()
        {
            if (_m_rawImage == null)
                return;

            if (!string.IsNullOrEmpty(_textureKey))
            {
                Texture texture = Localization.GetTexture(_textureKey);
                if (texture != null)
                    _m_rawImage.texture = texture;
            }
        }
    }
}
