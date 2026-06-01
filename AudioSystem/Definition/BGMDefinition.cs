// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using CodaGame.Base;
using UnityEngine;

namespace CodaGame
{
    /// <summary>
    /// A background music unit. Authored as a ScriptableObject asset and passed to
    /// <see cref="Audio.PlayBGM(BGMDefinition, int, float)"/>.
    /// </summary>
    [CreateAssetMenu(menuName = "CodaGame/Audio/BGM Definition")]
    public class BGMDefinition : _AAudioDefinition
    {
        [SerializeField] private AssetIndex _m_clipIndex;
        [NonSerialized] private SoundClipOperation _m_clipOperation;


        public AssetIndex clipIndex { get { return _m_clipIndex; } }


        public override AudioClip GetClip()
        {
            if (!_m_clipIndex.isValid)
            {
                Console.LogError(SystemNames.Audio, "BGMDefinition '" + name + "' has no valid clip.");
                return null;
            }

            AudioClip audioClip = AssetLoader.LoadSync<AudioClip>(_m_clipIndex);
            if (audioClip == null)
                Console.LogError(SystemNames.Audio, "BGMDefinition '" + name + "' failed to load clip.");

            return audioClip;
        }
        public override void ReleaseClip(AudioClip _clip)
        {
            if (_clip != null)
                AssetLoader.Release(_clip);
        }
        public override void Preload(Action _complete)
        {
            if (!_m_clipIndex.isValid)
            {
                Console.LogError(SystemNames.Audio, "BGMDefinition '" + name + "' has no valid clip to preload.");
                _complete?.Invoke();
                return;
            }

            _m_clipOperation ??= new SoundClipOperation(_m_clipIndex, name);
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

            _source.spatialBlend = 0f;
            _source.loop = true;
            _source.pitch = 1f;
        }
    }
}
