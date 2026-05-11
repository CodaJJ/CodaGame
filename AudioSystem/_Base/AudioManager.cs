// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Audio;
using Object = UnityEngine.Object;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace CodaGame.Base
{
    /// <summary>
    /// Internal manager for the Audio system. Owns the source pool and BGM layers.
    /// The <see cref="AudioMixer"/> reference is held by <see cref="_AGameMain"/>.
    /// </summary>
    /// <remarks>
    /// Per-playback lifecycle (play/stop/fade/recycle) lives on <see cref="Playback"/>.
    /// Per-layer orchestration (current BGM, cross-fade) lives on <see cref="BGMLayer"/>.
    /// Clip reference-counting is delegated to the <see cref="_AAudioDefinition"/> itself
    /// (via Addressables + optional <see cref="SoundClipOperation"/> for preload sessions).
    /// </remarks>
    internal class AudioManager
    {
        [NotNull] public static AudioManager instance { get { return _g_instance ??= new AudioManager(); } }
        private static AudioManager _g_instance;


        private readonly bool _m_initFailed;
        private readonly AudioMixer _m_mixer;
        private readonly AudioSourcePool _m_pool;

        [NotNull] private readonly Dictionary<int, BGMLayer> _m_bgmLayers;


        private AudioManager()
        {
            _m_bgmLayers = new Dictionary<int, BGMLayer>();

            _m_mixer = _AGameMain.instance.audioMixer;
            if (_m_mixer == null)
            {
                _m_initFailed = true;
                Console.LogError(SystemNames.Audio, "Audio system failed to initialize: GameMain has no AudioMixer assigned.");
                return;
            }

            GameObject poolRoot = new GameObject("Audio_Pool");
            Object.DontDestroyOnLoad(poolRoot);
            _m_pool = new AudioSourcePool(poolRoot.transform);

            Console.LogVerbose(SystemNames.Audio, "Audio system initialized.");
        }


        // ==================================================================
        // Mixer volume (string-keyed exposed parameters)
        // ==================================================================
        public void SetMixerVolume(string _exposedParam, float _linearVolume)
        {
            if (!CheckInitialized("SetMixerVolume")) return;
            if (string.IsNullOrEmpty(_exposedParam))
            {
                Console.LogError(SystemNames.Audio, "Audio.SetMixerVolume called with null or empty parameter name.");
                return;
            }
            _m_mixer.SetFloat(_exposedParam, ConvertUtility.LinearToDb(Mathf.Clamp01(_linearVolume)));
        }
        public float GetMixerVolume(string _exposedParam)
        {
            if (!CheckInitialized("GetMixerVolume")) return 1f;
            if (string.IsNullOrEmpty(_exposedParam))
            {
                Console.LogError(SystemNames.Audio, "Audio.GetMixerVolume called with null or empty parameter name.");
                return 1f;
            }
            if (_m_mixer.GetFloat(_exposedParam, out float db))
                return ConvertUtility.DbToLinear(db);

            Console.LogWarning(SystemNames.Audio, "Audio.GetMixerVolume: exposed parameter '" + _exposedParam + "' not found on mixer.");
            return 1f;
        }


        // ==================================================================
        // Preload / Release (SFX)
        // ==================================================================
        public void Preload(_AAudioDefinition _def, Action _complete)
        {
            if (!CheckInitialized("Preload")) { _complete?.Invoke(); return; }
            if (_def == null)
            {
                Console.LogError(SystemNames.Audio, "Audio.Preload called with null AudioDefinition.");
                _complete?.Invoke();
                return;
            }
            _def.Preload(_complete);
        }
        public void Release(_AAudioDefinition _def)
        {
            if (!CheckInitialized("Release")) return;
            if (_def == null)
            {
                Console.LogError(SystemNames.Audio, "Audio.Release called with null AudioDefinition.");
                return;
            }
            _def.ReleasePreload();
        }


        // ==================================================================
        // Play (SFX)
        // ==================================================================
        public AudioHandle Play(SoundDefinition _def, AudioScope _scope)
        {
            if (!ValidateSFXPlay(_def, "Play")) return AudioHandle.Invalid;

            Playback playback = new Playback(_def, _m_pool, _scope);
            if (!playback.isAlive) return AudioHandle.Invalid;

            playback.Play(0f);
            return new AudioHandle(playback);
        }
        public AudioHandle Play(SoundDefinition _def, Vector3 _position, AudioScope _scope)
        {
            if (!ValidateSFXPlay(_def, "Play")) return AudioHandle.Invalid;

#if UNITY_EDITOR
            if (!_def.is3D)
                Console.LogWarning(SystemNames.Audio, "Audio.Play: SoundDefinition '" + _def.name + "' is not 3D but a position was provided. Position will be ignored.");
#endif

            PositionedPlayback playback = new PositionedPlayback(_def, _m_pool, _scope, _position);
            if (!playback.isAlive) return AudioHandle.Invalid;

            playback.Play(0f);
            return new AudioHandle(playback);
        }
        public AudioHandle Play(SoundDefinition _def, Transform _follower, AudioScope _scope)
        {
            if (!ValidateSFXPlay(_def, "Play")) return AudioHandle.Invalid;
            if (_follower == null)
            {
                Console.LogError(SystemNames.Audio, "Audio.Play called with a null follower; use the 2D overload instead.");
                return AudioHandle.Invalid;
            }

#if UNITY_EDITOR
            if (!_def.is3D)
                Console.LogWarning(SystemNames.Audio, "Audio.Play: SoundDefinition '" + _def.name + "' is not 3D but a follower was provided. Position will be ignored.");
#endif

            FollowerPlayback playback = new FollowerPlayback(_def, _m_pool, _scope, _follower);
            if (!playback.isAlive) return AudioHandle.Invalid;

            playback.Play(0f);
            return new AudioHandle(playback);
        }


        // ==================================================================
        // BGM
        // ==================================================================
        public void PlayBGM(BGMDefinition _def, int _layer, float _fadeIn)
        {
            if (!CheckInitialized("PlayBGM")) return;
            if (_def == null)
            {
                Console.LogError(SystemNames.Audio, "Audio.PlayBGM called with null BGMDefinition.");
                return;
            }
            if (!_def.clipIndex.isValid)
            {
                Console.LogError(SystemNames.Audio, "Audio.PlayBGM: BGMDefinition '" + _def.name + "' has no valid clip.");
                return;
            }

            Playback newPlayback = new Playback(_def, _m_pool, _scope: null);
            if (!newPlayback.isAlive) return;

            BGMLayer layer = GetOrCreateLayer(_layer);
            layer.Switch(newPlayback, _fadeIn);
        }
        public void StopBGM(int _layer, float _fadeOut)
        {
            if (!CheckInitialized("StopBGM")) return;
            if (!_m_bgmLayers.TryGetValue(_layer, out BGMLayer layer)) return;

            layer.Clear(_fadeOut);
            _m_bgmLayers.Remove(_layer);
        }
        public void StopAllBGM(float _fadeOut)
        {
            if (!CheckInitialized("StopAllBGM")) return;

            foreach (BGMLayer layer in _m_bgmLayers.Values)
                layer.Clear(_fadeOut);
            _m_bgmLayers.Clear();
        }


        // ==================================================================
        // Snapshot
        // ==================================================================
        public void TransitionSnapshot(AudioMixerSnapshot _snapshot, float _duration)
        {
            if (!CheckInitialized("TransitionSnapshot")) return;
            if (_snapshot == null)
            {
                Console.LogError(SystemNames.Audio, "Audio.TransitionSnapshot called with null AudioMixerSnapshot.");
                return;
            }

            _snapshot.TransitionTo(Mathf.Max(0f, _duration));
        }


        // ==================================================================
        // Helpers
        // ==================================================================
        [MemberNotNullWhen(true, "_m_mixer", "_m_pool")]
        private bool ValidateSFXPlay(SoundDefinition _def, string _caller)
        {
            if (!CheckInitialized(_caller)) return false;
            if (_def == null)
            {
                Console.LogError(SystemNames.Audio, "Audio." + _caller + " called with null SoundDefinition.");
                return false;
            }
            if (_def.clips == null || _def.clips.Count == 0)
            {
                Console.LogError(SystemNames.Audio, "Audio." + _caller + ": SoundDefinition '" + _def.name + "' has no clips.");
                return false;
            }
            return true;
        }
        [NotNull]
        private BGMLayer GetOrCreateLayer(int _layerIndex)
        {
            if (!_m_bgmLayers.TryGetValue(_layerIndex, out BGMLayer layer))
            {
                layer = new BGMLayer();
                _m_bgmLayers.Add(_layerIndex, layer);
            }
            return layer;
        }
        [MemberNotNullWhen(true, "_m_mixer", "_m_pool")]
        private bool CheckInitialized(string _caller)
        {
            if (_m_initFailed)
            {
                Console.LogError(SystemNames.Audio, "Audio." + _caller + ": Audio system is not initialized.");
                return false;
            }
            return true;
        }
    }
}
