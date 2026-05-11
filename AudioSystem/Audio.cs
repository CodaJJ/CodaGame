// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using CodaGame.Base;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;

namespace CodaGame
{
    /// <summary>
    /// Public facade for the Audio system. Auto-initializes on first use; requires
    /// <see cref="_AGameMain"/> to have an <see cref="AudioMixer"/> assigned.
    /// </summary>
    public static class Audio
    {
        [NotNull] private static AudioManager manager { get { return AudioManager.instance; } }


        // ==================================================================
        // SFX — one-shot or looping sound effects
        // ==================================================================

        /// <summary>
        /// Play a 2D sound effect.
        /// </summary>
        /// <param name="_def">The sound to play. A random clip is picked if multiple clips are assigned.</param>
        /// <param name="_scope">Optional scope that owns this sound. Disposing the scope stops all sounds in it.</param>
        /// <returns>A handle to control the playing sound. Returns <see cref="AudioHandle.Invalid"/> on failure.</returns>
        public static AudioHandle Play(SoundDefinition _def, AudioScope _scope = null)
        {
            return manager.Play(_def, _scope);
        }
        /// <summary>
        /// Play a 3D sound effect at a fixed world position.
        /// </summary>
        /// <param name="_def">The sound to play. <see cref="SoundDefinition.is3D"/> should be true for spatial audio;
        /// a warning is logged in the Editor if it is not.</param>
        /// <param name="_position">World-space position of the audio source.</param>
        /// <param name="_scope">Optional scope that owns this sound.</param>
        public static AudioHandle Play(SoundDefinition _def, Vector3 _position, AudioScope _scope = null)
        {
            return manager.Play(_def, _position, _scope);
        }
        /// <summary>
        /// Play a 3D sound effect that follows a <see cref="Transform"/>.
        /// The source position is synced every frame. If the follower is destroyed, the sound stops automatically.
        /// </summary>
        /// <param name="_def">The sound to play.</param>
        /// <param name="_follower">Transform to follow. Must not be null.</param>
        /// <param name="_scope">Optional scope that owns this sound.</param>
        public static AudioHandle Play(SoundDefinition _def, Transform _follower, AudioScope _scope = null)
        {
            return manager.Play(_def, _follower, _scope);
        }


        // ==================================================================
        // BGM — looping background music with cross-fade
        // ==================================================================

        /// <summary>
        /// Play background music. If the layer already has a BGM playing, the old one cross-fades out
        /// while the new one fades in over <paramref name="_fadeIn"/> seconds.
        /// </summary>
        /// <param name="_def">The BGM to play. Unlike <see cref="SoundDefinition"/>, this always loops
        /// and holds a single clip with no randomization or 3D settings.</param>
        /// <param name="_layer">BGM layer index (0 or 1). Independent layers allow overlapping BGMs
        /// (e.g. main music on layer 0, ambient on layer 1).</param>
        /// <param name="_fadeIn">Cross-fade duration in seconds. Pass 0 for an instant switch.</param>
        public static void PlayBGM(BGMDefinition _def, int _layer = 0, float _fadeIn = 1.5f)
        {
            manager.PlayBGM(_def, _layer, _fadeIn);
        }
        /// <summary>
        /// Stop the BGM on the specified layer.
        /// </summary>
        /// <param name="_layer">BGM layer index to stop.</param>
        /// <param name="_fadeOut">Fade-out duration in seconds. Pass 0 for an instant stop.</param>
        public static void StopBGM(int _layer = 0, float _fadeOut = 1.0f)
        {
            manager.StopBGM(_layer, _fadeOut);
        }
        /// <summary>
        /// Stop all BGM layers.
        /// </summary>
        /// <param name="_fadeOut">Fade-out duration in seconds. Pass 0 for an instant stop.</param>
        public static void StopAllBGM(float _fadeOut = 1.0f)
        {
            manager.StopAllBGM(_fadeOut);
        }


        // ==================================================================
        // Mixer — volume and snapshot control
        // ==================================================================

        /// <summary>
        /// Set a mixer exposed parameter to the given linear volume (0..1).
        /// The conversion to dB is handled internally.
        /// </summary>
        /// <remarks>
        /// The parameter name must be "exposed" in the Unity AudioMixer (right-click a parameter → Expose to script).
        /// The framework does not persist volume settings — projects should handle their own save/load.
        /// </remarks>
        public static void SetMixerVolume(string _exposedParam, float _linearVolume)
        {
            manager.SetMixerVolume(_exposedParam, _linearVolume);
        }
        /// <summary>
        /// Read a mixer exposed parameter and return it as linear volume (0..1).
        /// </summary>
        public static float GetMixerVolume(string _exposedParam)
        {
            return manager.GetMixerVolume(_exposedParam);
        }
        /// <summary>
        /// Smoothly transition to the target <see cref="AudioMixerSnapshot"/> over <paramref name="_duration"/> seconds.
        /// A snapshot captures the full state of the mixer (volumes, effects, etc.).
        /// Pass 0 for an instant switch.
        /// </summary>
        public static void TransitionSnapshot(AudioMixerSnapshot _snapshot, float _duration)
        {
            manager.TransitionSnapshot(_snapshot, _duration);
        }


        // ==================================================================
        // Preload / Release — manual clip lifetime management
        // ==================================================================

        /// <summary>
        /// Pre-load all clips in the <see cref="SoundDefinition"/> into memory (reference counted).
        /// Each <see cref="Preload(SoundDefinition, Action)"/> call must be balanced by a matching
        /// <see cref="Release(SoundDefinition)"/> call.
        /// <para>
        /// Clips played via <see cref="Play(SoundDefinition, AudioScope)"/> without a prior Preload are loaded on-demand and
        /// released automatically when playback ends — no manual Release is needed in that case.
        /// </para>
        /// </summary>
        /// <param name="_def">The sound definition whose clips to preload.</param>
        /// <param name="_complete">Optional callback invoked when all clips are ready.</param>
        public static void Preload(SoundDefinition _def, Action _complete = null)
        {
            manager.Preload(_def, _complete);
        }
        /// <summary>
        /// Batch preload for <see cref="SoundDefinition"/>. The callback fires once when all
        /// definitions have finished loading. Each definition must be individually released.
        /// </summary>
        public static void Preload(List<SoundDefinition> _defs, Action _complete = null)
        {
            PreloadBatch(_defs, _complete);
        }
        /// <summary>
        /// Release a previously preloaded <see cref="SoundDefinition"/>. When the reference count
        /// reaches zero, the underlying <see cref="AudioClip"/> assets are unloaded.
        /// Only call this to balance a prior <see cref="Preload(SoundDefinition, Action)"/> — clips loaded
        /// implicitly by <see cref="Play(SoundDefinition, AudioScope)"/> are managed automatically.
        /// </summary>
        public static void Release(SoundDefinition _def)
        {
            manager.Release(_def);
        }

        /// <summary>
        /// Pre-load the <see cref="BGMDefinition"/>'s clip into memory (reference counted).
        /// Recommended before <see cref="PlayBGM"/> to avoid load hitches on the first play —
        /// BGM clips are typically large enough that the synchronous load inside <c>PlayBGM</c>
        /// can cause a visible stall.
        /// Each <see cref="Preload(BGMDefinition, Action)"/> must be balanced by a matching
        /// <see cref="Release(BGMDefinition)"/>.
        /// </summary>
        public static void Preload(BGMDefinition _def, Action _complete = null)
        {
            manager.Preload(_def, _complete);
        }
        /// <summary>
        /// Batch preload for <see cref="BGMDefinition"/>. The callback fires once when all
        /// definitions have finished loading. Each definition must be individually released.
        /// </summary>
        public static void Preload(List<BGMDefinition> _defs, Action _complete = null)
        {
            PreloadBatch(_defs, _complete);
        }
        /// <summary>
        /// Release a previously preloaded <see cref="BGMDefinition"/>. When the reference count
        /// reaches zero, the underlying <see cref="AudioClip"/> is unloaded.
        /// </summary>
        public static void Release(BGMDefinition _def)
        {
            manager.Release(_def);
        }


        private static void PreloadBatch<T>(List<T> _defs, Action _complete) where T : _AAudioDefinition
        {
            if (_defs == null)
            {
                _complete?.Invoke();
                return;
            }

            Async.Parallel(_defs, (_def, _loadDone) =>
            {
                if (_def == null)
                {
                    _loadDone.Invoke();
                    return;
                }
                
                manager.Preload(_def, _loadDone);
            }, _complete);
        }
    }
}
