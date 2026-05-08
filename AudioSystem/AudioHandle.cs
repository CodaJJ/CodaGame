// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// Lightweight value-type handle for a playing sound effect.
    /// </summary>
    /// <remarks>
    /// <para>The handle stays valid until the underlying playback ends (sound completes, is stopped, or its scope is disposed).
    /// All mutation methods are silent no-ops on invalid handles; they never throw.</para>
    /// <para>BGM does not return a handle — use <c>Audio.StopBGM(layer)</c> for lifecycle control.</para>
    /// </remarks>
    public readonly struct AudioHandle
    {
        public static readonly AudioHandle Invalid = default;


        private readonly Playback _m_playback;


        internal AudioHandle(Playback _playback)
        {
            _m_playback = _playback;
        }


        public bool isValid { get { return _m_playback is { isAlive: true }; } }


        public void Stop(float _fadeOut = 0f)
        {
            if (_m_playback is not { isAlive: true }) return;
            _m_playback.Stop(_fadeOut);
        }
        public void SetVolume(float _volume)
        {
            if (_m_playback is not { isAlive: true }) return;
            _m_playback.source.volume = Mathf.Clamp01(_volume);
        }
    }
}
