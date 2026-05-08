// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using JetBrains.Annotations;

namespace CodaGame.Base
{
    /// <summary>
    /// Thin orchestrator for a single BGM channel. Owns a pointer to the currently-playing
    /// playback and delegates all lifecycle (fade-in, fade-out, cross-fade) to the playbacks themselves.
    /// </summary>
    /// <remarks>
    /// A cross-fade is simply: stop the outgoing playback with a fade-out and play the incoming
    /// playback with an equal fade-in. Both playbacks run their own fade tasks concurrently.
    /// </remarks>
    internal class BGMLayer
    {
        private Playback _m_current;


        public bool isEmpty { get { return _m_current == null; } }


        /// <summary>
        /// Replace the current playback with <paramref name="_newPlayback"/>. If a previous
        /// playback exists, it is stopped with <paramref name="_crossFade"/>; the new playback is
        /// started with the same fade-in.
        /// </summary>
        public void Switch([NotNull] Playback _newPlayback, float _crossFade)
        {
            _m_current?.Stop(_crossFade);
            _m_current = _newPlayback;
            _newPlayback.Play(_crossFade);
        }
        /// <summary>
        /// Stop the current playback (if any) with the given fade-out.
        /// </summary>
        public void Clear(float _fadeOut)
        {
            _m_current?.Stop(_fadeOut);
            _m_current = null;
        }
    }
}
