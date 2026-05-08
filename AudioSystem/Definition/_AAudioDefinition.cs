// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using UnityEngine;
using UnityEngine.Audio;

namespace CodaGame
{
    /// <summary>
    /// Abstract base for a playable audio unit. Concrete subclasses (<see cref="SoundDefinition"/>,
    /// <see cref="BGMDefinition"/>) own their clip lifecycle via immediate acquisition
    /// (<see cref="GetClip"/> / <see cref="ReleaseClip"/>) or explicit preload sessions
    /// (<see cref="Preload"/> / <see cref="ReleasePreload"/>).
    /// </summary>
    /// <remarks>
    /// <para><strong>Immediate acquisition:</strong> <see cref="GetClip"/> loads one clip synchronously
    /// (random pick for multi-clip definitions); <see cref="ReleaseClip"/> releases that reference.
    /// Addressables manages ref-counting internally — no prior <see cref="Preload"/> is required.</para>
    /// <para><strong>Preload sessions:</strong> <see cref="Preload"/> asynchronously loads all clips and
    /// keeps them resident; <see cref="ReleasePreload"/> releases one session. Multiple calls to
    /// <see cref="Preload"/> are ref-counted and must each be balanced by a <see cref="ReleasePreload"/>.</para>
    /// <para><strong>Source configuration:</strong> subclasses override <see cref="ConfigureSource"/> to
    /// prepare the shared <see cref="AudioSource"/> (mixer group, volume, spatial blend, pitch, etc.).
    /// The base implementation sets mixer group and volume.</para>
    /// </remarks>
    public abstract class _AAudioDefinition : ScriptableObject
    {
        [SerializeField] private AudioMixerGroup _m_mixerGroup;
        [SerializeField, Range(0f, 1f)] private float _m_volume = 1f;
        [SerializeField] private bool _m_affectedByTimeScale;
        
        
        public bool affectedByTimeScale { get { return _m_affectedByTimeScale; } }

        
        /// <summary>
        /// Load a single clip synchronously. Safe to call without a prior <see cref="Preload"/>.
        /// </summary>
        /// <returns>The loaded audio clip, or null if loading fails.</returns>
        public abstract AudioClip GetClip();
        /// <summary>
        /// Release a single clip obtained from <see cref="GetClip"/>.
        /// </summary>
        /// <param name="_clip">The clip to release.</param>
        public abstract void ReleaseClip(AudioClip _clip);
        /// <summary>
        /// Keep all clips referenced by this definition resident. Must be balanced by <see cref="ReleasePreload"/>.
        /// Multiple calls nest (ref-counted).
        /// </summary>
        /// <param name="_complete">Callback invoked when preload completes.</param>
        public abstract void Preload(Action _complete);
        /// <summary>
        /// Release one <see cref="Preload"/> session. When all sessions are released, clips become unloadable.
        /// </summary>
        public abstract void ReleasePreload();
        
        
        public virtual void ConfigureSource(AudioSource _source)
        {
            if (_source == null)
                return;
            
            _source.outputAudioMixerGroup = _m_mixerGroup;
            _source.volume = _m_volume;
        }
    }
}
