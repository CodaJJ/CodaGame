// Copyright (c) 2026 Coda
//
// This file is part of CodaGame, licensed under the MIT License.
// See the LICENSE file in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using JetBrains.Annotations;
using NotNull = JetBrains.Annotations.NotNullAttribute;

namespace CodaGame.Base
{
    /// <summary>
    /// Single audio playback context. Owns one <see cref="AudioSource"/> drawn from the shared pool
    /// and drives its full lifecycle: fade-in / fade-out, time-scale pitch sync, end-of-clip recycle,
    /// scope tracking, and clip release.
    /// </summary>
    /// <remarks>
    /// <para>Used directly for BGM and 2D SFX. Subclasses (<see cref="PositionedPlayback"/>,
    /// <see cref="FollowerPlayback"/>) add 3D position handling via the <see cref="OnConfigured"/>
    /// and <see cref="OnPlayStarted"/> hooks.</para>
    /// <para>The clip is loaded synchronously via <see cref="_AAudioDefinition.GetClip"/> at
    /// construction time. Callers that care about load-time hitches should <see cref="Audio.Preload"/>
    /// the definition first so the sync get becomes a hot-cache fetch.</para>
    /// </remarks>
    internal class Playback
    {
        [NotNull] private readonly _AAudioDefinition _m_definition;
        [NotNull] private readonly AudioSourcePool _m_sourcePool;
        private readonly AudioScope _m_scope;
        private readonly bool _m_useUnscaledTime;

        private bool _m_isAlive;
        private AudioSource _m_source;
        private AudioClip _m_clip;
        private float _m_targetVolume;
        private float _m_basePitch;

        private TaskHandle _m_fadeTask;
        private TaskHandle _m_fadeEndTask;
        private TaskHandle _m_pitchSyncTask;
        private TaskHandle _m_completionTask;


        public Playback([NotNull] _AAudioDefinition _definition, [NotNull] AudioSourcePool _sourcePool, AudioScope _scope)
        {
            _m_definition = _definition;
            _m_sourcePool = _sourcePool;
            _m_scope = _scope;
            _m_useUnscaledTime = !_m_definition.affectedByTimeScale;

            _m_source = _sourcePool.Get();
            if (_m_source == null)
                return;
            
            _m_clip = _m_definition.GetClip();
            if (_m_clip == null)
            {
                // Load failed — return the source immediately; Play() will be a silent no-op.
                _sourcePool.Release(_m_source);
                _m_source = null;
                return;
            }

            _m_source.clip = _m_clip;
            _m_definition.ConfigureSource(_m_source);
            _m_targetVolume = _m_source.volume;
            _m_basePitch = _m_source.pitch;

            _m_isAlive = true;
        }


        [MemberNotNullWhen(true, "_m_source", "source", "_m_clip")]
        public bool isAlive { get { return _m_isAlive; } }
        public AudioSource source { get { return _m_source; } }

        protected bool useUnscaledTime { get { return _m_useUnscaledTime; } }


        public void Play(float _fadeIn)
        {
            if (!isAlive) return;

            _m_scope?.Add(this);
            _m_source.Play();

            if (_fadeIn > 0f)
            {
                _m_source.volume = 0f;
                StartFade(0f, _m_targetVolume, _fadeIn, null);
            }

            if (!_m_useUnscaledTime)
                _m_pitchSyncTask = Task.RunContinuousActionTask(_dt => _m_source.pitch = _m_basePitch * Time.timeScale, _duration: -1f);

            if (!_m_source.loop)
            {
                float duration = _m_clip.length / Mathf.Max(0.01f, _m_basePitch);
                _m_completionTask = Task.RunDelayActionTask(() =>
                {
                    if (!_m_isAlive) return;
                    _m_isAlive = false;
                    _m_scope?.Remove(this);
                    Recycle();
                }, duration, UpdateType.Update, _m_useUnscaledTime);
            }
            
            OnPlayStarted();
        }
        public void Stop(float _fadeOut)
        {
            if (!_m_isAlive) return;
            _m_isAlive = false;

            _m_scope?.Remove(this);

            if (_fadeOut <= 0f)
            {
                Recycle();
                return;
            }

            StartFade(_m_source.volume, 0f, _fadeOut, Recycle);
        }


        /// <summary>Hook fired immediately after <see cref="AudioSource.Play"/> and the framework tasks are started.</summary>
        protected virtual void OnPlayStarted() { }
        /// <summary>Hook fired during recycle, before the source is returned to the pool and the clip is released.</summary>
        protected virtual void OnRecycle() { }


        private void StartFade(float _from, float _to, float _duration, Action _onComplete)
        {
            _m_fadeTask.StopTask();
            _m_fadeEndTask.StopTask();

            float elapsed = 0f;
            _m_fadeTask = Task.RunContinuousActionTask(_dt =>
            {
                elapsed += _dt;
                if (_m_source != null)
                    _m_source.volume = Mathf.Lerp(_from, _to, elapsed / _duration);
            }, _duration, UpdateType.Update, _m_useUnscaledTime);
            _m_fadeEndTask = Task.RunDelayActionTask(() =>
            {
                if (_m_source != null)
                    _m_source.volume = _to;
                _onComplete?.Invoke();
            }, _duration, UpdateType.Update, _m_useUnscaledTime);
        }
        private void Recycle()
        {
            _m_fadeTask.StopTask();
            _m_fadeEndTask.StopTask();
            _m_pitchSyncTask.StopTask();
            _m_completionTask.StopTask();

            OnRecycle();

            if (_m_clip != null)
            {
                _m_definition.ReleaseClip(_m_clip);
                _m_clip = null;
            }

            if (_m_source != null)
            {
                _m_sourcePool.Release(_m_source);
                _m_source = null;
            }
        }
    }
}
