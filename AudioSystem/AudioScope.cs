// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using CodaGame.Base;
using JetBrains.Annotations;

namespace CodaGame
{
    /// <summary>
    /// A disposable bag of playing sounds. <see cref="Dispose()"/> stops every sound that was played
    /// into this scope. Use one per logical owner (e.g., per Stage subclass) for batch cleanup.
    /// </summary>
    public sealed class AudioScope : IDisposable
    {
        [NotNull] private readonly List<Playback> _m_playbacks;
        private bool _m_disposed;


        public AudioScope()
        {
            _m_playbacks = new List<Playback>();
            _m_disposed = false;
        }


        public bool isDisposed { get { return _m_disposed; } }


        public void Dispose()
        {
            Dispose(0f);
        }
        public void Dispose(float _fadeOut)
        {
            if (_m_disposed)
                return;

            _m_disposed = true;

            for (int i = 0; i < _m_playbacks.Count; ++i)
                _m_playbacks[i].Stop(_fadeOut);
            _m_playbacks.Clear();
        }


        // Called by AudioManager when a sound is played into this scope.
        internal void Add(Playback _playback)
        {
            if (_m_disposed)
            {
                Console.LogWarning(SystemNames.Audio, "Attempted to add a sound to a disposed AudioScope; the sound was stopped immediately.");
                _playback.Stop(0f);
                return;
            }
            _m_playbacks.Add(_playback);
        }
        // Called by Playback when it is recycled (natural end or external stop).
        // List.Remove is O(n); switch to HashSet<Playback> if scope sizes grow large.
        internal void Remove(Playback _playback)
        {
            if (_m_disposed)
                return;

            _m_playbacks.Remove(_playback);
        }
    }
}
