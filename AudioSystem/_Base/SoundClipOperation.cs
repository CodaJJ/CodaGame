// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// Reference-counted async operation that loads and caches all <see cref="AudioClip"/>s
    /// for a single <see cref="SoundDefinition"/>. Replaces the manual refcount logic formerly in ClipRefEntry.
    /// </summary>
    /// <remarks>
    /// <para>First <see cref="_AReferenceCountAsyncOperation.Start"/> triggers parallel clip loading.
    /// When the last reference is released via <see cref="_AReferenceCountAsyncOperation.End"/>,
    /// all clips are unloaded.</para>
    /// </remarks>
    internal class SoundClipOperation : _AReferenceCountAsyncOperation
    {
        [NotNull] private readonly List<AssetIndex> _m_clipIndexes;
        private readonly string _m_definitionName;
        
        private List<AudioClip> _m_clips;


        internal SoundClipOperation([NotNull] List<AssetIndex> _indexes, string _definitionName)
        {
            _m_clipIndexes = new List<AssetIndex>(_indexes);
            _m_definitionName = _definitionName;
        }
        internal SoundClipOperation(AssetIndex _index, string _definitionName)
        {
            _m_clipIndexes = new List<AssetIndex>(1) { _index };
            _m_definitionName = _definitionName;
        }


        public List<AudioClip> clips { get { return _m_clips; } }


        protected override void OnOperationStart(Action _complete)
        {
            _m_clips = new List<AudioClip>(_m_clipIndexes.Count);
            Async.Parallel(_m_clipIndexes, (_clipIndex, _loadDone) =>
            {
                if (!_clipIndex.isValid)
                {
                    Console.LogError(SystemNames.Audio, "AudioDefinition '" + _m_definitionName + "' has invalid AssetIndex at clip.");
                    _loadDone.Invoke();
                    return;
                }
                
                AssetLoader.LoadAsync<AudioClip>(_clipIndex, _clip =>
                {
                    _m_clips.Add(_clip);
                    _loadDone.Invoke();
                });                
            }, _complete);
        }
        protected override void OnOperationEnd(Action _complete)
        {
            if (_m_clips != null)
            {
                for (int i = 0; i < _m_clips.Count; ++i)
                {
                    if (_m_clips[i] != null)
                        AssetLoader.Release(_m_clips[i]);
                }
                _m_clips = null;
            }
            _complete();
        }
    }
}
