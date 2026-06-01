// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A playable audio unit. Authored as a ScriptableObject asset and passed to <see cref="Audio.Play(SoundDefinition, AudioScope)"/>.
    /// </summary>
    /// <remarks>
    /// <para>If <see cref="clips"/> contains more than one entry, one is chosen at random per playback.</para>
    /// <para>Volume and pitch are randomized within the configured ranges per playback.</para>
    /// </remarks>
    [CreateAssetMenu(menuName = "CodaGame/Audio/Sound Definition")]
    public class SoundDefinition : _AAudioDefinition
    {
        [SerializeField] private InspectorList<AssetIndex> _m_clips;

        [SerializeField] private FloatRange _m_volumeMultiplierRange = new FloatRange(1, 1);
        [SerializeField] private FloatRange _m_pitchRange = new FloatRange(1, 1);

        [SerializeField] private bool _m_loop;

        [SerializeField] private bool _m_is3D;
        [SerializeField, ShowIf(nameof(_m_is3D))] private float _m_minDistance = 1f;
        [SerializeField, ShowIf(nameof(_m_is3D))] private float _m_maxDistance = 50f;
        [SerializeField, ShowIf(nameof(_m_is3D))] private AudioRolloffMode _m_rolloffMode = AudioRolloffMode.Logarithmic;

        [NonSerialized] private SoundClipOperation _m_clipOperation;


        public List<AssetIndex> clips { get { return _m_clips; } }
        public FloatRange volumeMultiplierRange { get { return _m_volumeMultiplierRange; } }
        public FloatRange pitchRange { get { return _m_pitchRange; } }
        public bool is3D { get { return _m_is3D; } }
        public float minDistance { get { return _m_minDistance; } }
        public float maxDistance { get { return _m_maxDistance; } }
        public AudioRolloffMode rolloffMode { get { return _m_rolloffMode; } }
        public bool loop { get { return _m_loop; } }


        public override AudioClip GetClip()
        {
            if (_m_clips == null || _m_clips.Count == 0)
            {
                Console.LogError(SystemNames.Audio, "SoundDefinition '" + name + "' has no clips defined.");
                return null;
            }

            AssetIndex clipIndex = _m_clips.GetRandomElement();
            AudioClip audioClip = AssetLoader.LoadSync<AudioClip>(clipIndex);

            if (audioClip == null)
                Console.LogError(SystemNames.Audio, "SoundDefinition '" + name + "' failed to load clip from index: " + clipIndex);

            return audioClip;
        }
        public override void ReleaseClip(AudioClip _clip)
        {
            if (_clip != null)
                AssetLoader.Release(_clip);
        }
        public override void Preload(Action _complete)
        {
            if (_m_clips == null || _m_clips.Count == 0)
            {
                Console.LogError(SystemNames.Audio, "SoundDefinition '" + name + "' has no clips to preload.");
                _complete?.Invoke();
                return;
            }

            _m_clipOperation ??= new SoundClipOperation(_m_clips, name);
            _m_clipOperation.Start(_complete);
        }
        public override void ReleasePreload()
        {
            _m_clipOperation?.End();
        }
        public override void ConfigureSource(AudioSource _source)
        {
            base.ConfigureSource(_source);
            if (_source == null)
                return;

            // Per-play randomization: volume multiplier and pitch within configured ranges.
            _source.volume = Mathf.Clamp01(_source.volume * _m_volumeMultiplierRange.RandomValue());
            float pitch = _m_pitchRange.RandomValue();
            if (pitch < 0.05f)
            {
                Console.LogWarning(SystemNames.Audio, "SoundDefinition '" + name + "' pitch resolved to " + pitch + " which is near zero. Clamping to 0.05. Check pitchRange configuration.");
                pitch = 0.05f;
            }
            _source.pitch = pitch;
            _source.loop = _m_loop;

            if (_m_is3D)
            {
                _source.spatialBlend = 1f;
                _source.minDistance = _m_minDistance;
                _source.maxDistance = _m_maxDistance;
                _source.rolloffMode = _m_rolloffMode;
            }
            else
            {
                _source.spatialBlend = 0f;
            }
        }
    }
}
