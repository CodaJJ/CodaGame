// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using UnityEngine;

namespace CodaGame.Base
{
    /// <summary>
    /// Object pool for <see cref="AudioSource"/> components. Creates a new <see cref="GameObject"/>
    /// with an <see cref="AudioSource"/> when the cache is empty, and resets the source state on release.
    /// </summary>
    internal class AudioSourcePool : _AUnsafeSyncObjectPool<AudioSource>
    {
        private readonly Transform _m_poolRoot;
        private int _m_count;


        internal AudioSourcePool(Transform _poolRoot)
            : base("AudioSourcePool", 8)
        {
            _m_poolRoot = _poolRoot;
        }


        protected override AudioSource LoadObject()
        {
            GameObject go = new GameObject("AudioSource_" + _m_count++);
            go.transform.SetParent(_m_poolRoot, false);

            AudioSource src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            return src;
        }
        protected override void ResetObject(AudioSource _object)
        {
            if (_object == null)
                return;
            
            _object.Stop();
            _object.clip = null;
            _object.outputAudioMixerGroup = null;
            _object.loop = false;
            _object.spatialBlend = 0f;
            _object.pitch = 1f;
            _object.volume = 1f;
            _object.transform.localPosition = Vector3.zero;
        }
        protected override void DestroyObject(AudioSource _object)
        {
            if (_object == null)
                return;
            
            Object.Destroy(_object.gameObject);
        }
    }
}
